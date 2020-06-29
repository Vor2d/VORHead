using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace MeshSystem
{
    public static class MeshCutter
    {
        public static List<List<Transform>> get_lines_Acut(Vector3 CP1, Vector3 CP2, 
            Dictionary<MeshData,Transform> MeshDataPool, bool infinte, Texture2D texture2D,Transform par_TRANS)
        {
            List<List<Transform>> res_TRANSs = new List<List<Transform>>();
            Vector3 CP1_relative;
            Vector3 CP2_relative;
            Transform temp_TRANS;
            MeshLine cut_line;
            List<Transform> half_TRANSs;
            foreach (MeshData mesh_data in MeshDataPool.Keys.ToArray())
            {
                half_TRANSs = new List<Transform>();
                temp_TRANS = MeshDataPool[mesh_data];
                CP1_relative = CP1 - temp_TRANS.position;
                CP2_relative = CP2 - temp_TRANS.position;
                cut_line = new MeshLine();
                cut_line.line_cal(new MeshPoint(CP1_relative, true), new MeshPoint(CP2_relative, true));
                cut_line.infinite = infinte;
                if (cut(mesh_data, cut_line, texture2D, par_TRANS, MeshDataPool[mesh_data], out half_TRANSs))
                {
                    Debug.Log("!!!!! " + half_TRANSs.ToString());
                    res_TRANSs.Add(half_TRANSs);
                    mesh_data.clean_destroy(MeshDataPool);
                    Object.Destroy(temp_TRANS.gameObject);
                }
            }

            Debug.Log("================================");

            return res_TRANSs;
        }

        private static bool cut(MeshData mesh_data, MeshLine cut_line, Texture2D texture2D, Transform par_TRANS,
            Transform orig_TRANS, out List<Transform> res_TRANSs)
        {
            res_TRANSs = new List<Transform>();

            MeshPoint[] cut_points = mesh_data.line_inter_cal(cut_line);

            if (cut_points.Length < 2)
            {
                return false;
            }

            MeshData[] cut_meshs = mesh_data.cut(cut_points[0], cut_points[1]);
            Transform NM1_TRANS = MeshCreater.create_Unity_mesh(cut_meshs[0], false, texture2D, par_TRANS,
                            orig_TRANS.localPosition);
            Transform NM2_TRANS = MeshCreater.create_Unity_mesh(cut_meshs[1], false, texture2D, par_TRANS,
                            orig_TRANS.localPosition);
            //Debug.Log("mesh1 " + cut_meshs[0].VarToString());
            //Debug.Log("mesh2 " + cut_meshs[1].VarToString());
            res_TRANSs.Add(NM1_TRANS);
            res_TRANSs.Add(NM2_TRANS);
            return true;
        }

        private static void mesh_debug(MeshData mesh_data)
        {
            Debug.Log(" " + mesh_data.VarToString());
        }

        private static void mesh_debug(List<MeshPoint> list_MP, bool FHalf)
        {

        }
    }
}

