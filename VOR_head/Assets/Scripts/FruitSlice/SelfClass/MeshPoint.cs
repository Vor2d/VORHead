using UnityEngine;
using System.Collections.Generic;

namespace MeshSystem
{
    public class MeshPoint
    {
        public Vector2 pos { get; set; }
        public bool is_cut_point { get; set; }
        public Vector2 uv { get; set; } //UV relative positions to the center point;
        public float z_pos { get; set; }

        private MeshData mesh_data;

        public MeshPoint()
        {
            this.pos = new Vector2();
            this.is_cut_point = false;
            this.uv = new Vector2();
            this.z_pos = 0.0f;
            this.mesh_data = null;
        }

        public MeshPoint(MeshPoint MP)
        {
            this.pos = MP.pos;
            this.is_cut_point = MP.is_cut_point;
            this.uv = MP.uv;
            this.z_pos = MP.z_pos;
            this.mesh_data = MP.mesh_data;
        }

        public MeshPoint(Vector3 _pos, bool _ICP)
        {
            this.pos = new Vector2(_pos.x, _pos.y);
            this.is_cut_point = _ICP;
            this.uv = new Vector2();
            this.z_pos = _pos.z;
            this.mesh_data = null;
        }

        public MeshPoint(MeshData _mesh_data)
        {
            this.pos = new Vector2();
            this.is_cut_point = false;
            this.uv = new Vector2();
            this.z_pos = 0.0f;
            this.mesh_data = _mesh_data;
        }

        public MeshPoint(Vector3 _pos, bool _ICP,MeshData _mesh_data)
        {
            this.pos = new Vector2(_pos.x, _pos.y);
            this.is_cut_point = _ICP;
            this.uv = new Vector2();
            this.z_pos = _pos.z;
            this.mesh_data = _mesh_data;
        }

        /// <summary>
        /// Calculate the UV from the parent the MeshData
        /// </summary>
        /// <param name="MD"></param>
        public void uv_cal(MeshData MD)
        {
            float x = pos.x - MD.UV_poss[0].x;
            float y = pos.y - MD.UV_poss[0].y;
            float range_x = MD.UV_poss[1].x - MD.UV_poss[0].x;
            float range_y = MD.UV_poss[1].y - MD.UV_poss[0].y;
            float uv_x = x / range_x;
            float uv_y = y / range_y;
            uv = new Vector2(uv_x, uv_y);
        }

        public void uv_cal()
        {
            float x = pos.x - mesh_data.UV_poss[0].x;
            float y = pos.y - mesh_data.UV_poss[0].y;
            float range_x = mesh_data.UV_poss[1].x - mesh_data.UV_poss[0].x;
            float range_y = mesh_data.UV_poss[1].y - mesh_data.UV_poss[0].y;
            float uv_x = x / range_x;
            float uv_y = y / range_y;
            uv = new Vector2(uv_x, uv_y);
        }

        public MeshPoint clone()
        {
            MeshPoint MP = new MeshPoint(this);
            return MP;
        }

        public Vector3 to_v3()
        {
            return new Vector3(pos.x, pos.y, z_pos);
        }

        public string VarToString()
        {
            string result = "";
            result += " pos " + pos.ToString("F2");
            result += " is_cut_point " + is_cut_point.ToString();
            result += " uv " + uv.ToString("F2");
            result += " z_pos " + z_pos.ToString("F2");
            result += "\n";

            return result;
        }

        /// <summary>
        /// Sort verticies as clockwise from center; Works only for convex polygons;
        /// </summary>
        /// <param name="o_p"></param>
        /// <param name="center"></param>
        /// <returns></returns>
        public int CompareTo(MeshPoint o_p, Vector2 center)
        {
            if (this.pos.x - center.x >= 0 && o_p.pos.x - center.x < 0)
                return 1;
            if (this.pos.x - center.x < 0 && o_p.pos.x - center.x >= 0)
                return -1;
            if (this.pos.x - center.x == 0 && o_p.pos.x - center.x == 0)
            {
                if (this.pos.y - center.y >= 0 || o_p.pos.y - center.y >= 0)
                    return (this.pos.y > o_p.pos.y) ? 1 : -1;
                return (o_p.pos.y > this.pos.y) ? 1 : -1;
            }

            // compute the cross product of vectors (center -> a) x (center -> b)
            float det = (this.pos.x - center.x) * (o_p.pos.y - center.y) -
                            (o_p.pos.x - center.x) * (this.pos.y - center.y);
            if (det < 0)
                return -1;
            if (det > 0)
                return 1;

            // points a and b are on the same line from the center
            // check which point is closer to the center
            float d1 = (this.pos.x - center.x) * (this.pos.x - center.x) +
                        (this.pos.y - center.y) * (this.pos.y - center.y);
            float d2 = (o_p.pos.x - center.x) * (o_p.pos.x - center.x) +
                        (o_p.pos.y - center.y) * (o_p.pos.y - center.y);
            return (d1 > d2) ? 1 : -1;
        }


        #region:static

        public static List<MeshPoint> FromVec2(List<Vector2> list_v2)
        {
            List<MeshPoint> list_MP = new List<MeshPoint>();
            MeshPoint MP;
            foreach (Vector2 v2 in list_v2)
            {
                MP = new MeshPoint();
                MP.pos = v2;
                list_MP.Add(MP);
            }

            return list_MP;
        }

        public static List<MeshPoint> FromVec3(List<Vector3> list_v3)
        {
            List<MeshPoint> list_MP = new List<MeshPoint>();
            MeshPoint MP;
            foreach (Vector3 v3 in list_v3)
            {
                MP = new MeshPoint();
                MP.pos = new Vector2(v3.x, v3.y);
                MP.z_pos = v3.z;
                list_MP.Add(MP);
            }
            return list_MP;
        }

        public static List<MeshPoint> FromVec3(List<Vector3> list_v3, List<Vector2> UVs)
        {
            List<MeshPoint> list_MP = new List<MeshPoint>();
            MeshPoint MP;
            int iter = 0;
            foreach (Vector3 v3 in list_v3)
            {
                MP = new MeshPoint();
                MP.pos = new Vector2(v3.x, v3.y);
                MP.z_pos = v3.z;
                MP.uv = UVs[iter];
                list_MP.Add(MP);
                iter++;
            }
            return list_MP;
        }

        public static void SepLists(List<MeshPoint> list_MP,
                                    out List<Vector3> list_v3, out List<Vector2> list_uv)
        {
            list_v3 = new List<Vector3>();
            list_uv = new List<Vector2>();
            foreach (MeshPoint MP in list_MP)
            {
                list_v3.Add(new Vector3(MP.pos.x, MP.pos.y, MP.z_pos));
                list_uv.Add(MP.uv);
            }
        }

        #endregion
    }
}

