using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MeshSystem
{
    public static class MeshCreater
    {
        /// <summary>
        /// Create MeshData from position;
        /// </summary>
        /// <returns></returns>
        public static Transform create_mesh(Vector3[] poss, Vector3[] init_UV, Texture2D texture2D,
            Transform par_TRANS, Vector3 trans_pos = new Vector3(), string shader_str = "")
        {
            MeshData mesh_data = new MeshData();
            mesh_data.set_init_UVs(init_UV[0], init_UV[1]);

            List<MeshPoint> verts = new List<MeshPoint>();
            foreach (Vector3 pos in poss)
            {
                verts.Add(new MeshPoint(pos, false, mesh_data));
            }
            mesh_data.Verticies = verts;
            mesh_data.MD_regener();

            return create_Unity_mesh(mesh_data, true, texture2D, par_TRANS, trans_pos: trans_pos,
                shader_str: shader_str);
        }

        /// <summary>
        /// Create Unity Mesh;
        /// </summary>
        /// <returns>transform</returns>
        public static Transform create_Unity_mesh(MeshData mesh_data, bool cal_uv, Texture2D texture2D,
            Transform par_TRANS, Vector3 trans_pos = new Vector3(), string shader_str = "")
        {
            if (cal_uv)
            {
                mesh_data.uv_cal_all();
            }

            Transform NM_TRANS = new GameObject(FS_SD.FNewMesh_TRANS).transform;
            Mesh mesh = new Mesh();
            mesh_data.to_mesh(ref mesh);
            MeshFilter MF = NM_TRANS.gameObject.AddComponent<MeshFilter>();
            MF.mesh = mesh;
            MF.mesh.RecalculateNormals();
            MeshRenderer MR = NM_TRANS.gameObject.AddComponent<MeshRenderer>();
            MR.material = FS_RC.IS.FruitCommon_MTRL;
            MeshDataComp MDC = NM_TRANS.gameObject.AddComponent<MeshDataComp>();
            MDC.set_MD(mesh_data);
            MR.material.mainTexture = texture2D;
            if (shader_str != "") { MR.material.shader = Shader.Find(shader_str); }
            NM_TRANS.parent = par_TRANS;
            NM_TRANS.localPosition = trans_pos;
            FS_RC.IS.MeshDataPool.Add(mesh_data, NM_TRANS);

            return NM_TRANS;
        }
    }
}


