using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace MeshSystem
{
    public class MeshData
    {
        public List<MeshPoint> Verticies { get; set; }
        public List<int> Triangles { get; set; }    //Index of vertices {0,1,2,0,2,3};
        //public List<Vector2> UVs { get; set; }
        public Transform target_TRANS { get; set; }
        public Vector2 center_pos { get; set; }
        public Vector2[] UV_poss { get; set; }  //Upper left pos and Down right pos;
        public List<MeshLine> Mesh_lines { get; set; }  //Virtual lines for each verticies that stored for further calculation; 

        public MeshData()
        {
            this.Verticies = new List<MeshPoint>();
            this.Triangles = new List<int>();
            this.target_TRANS = null;
            //this.UVs = new List<Vector2>();
            this.center_pos = new Vector2();
            this.UV_poss = new Vector2[2];
            this.Mesh_lines = new List<MeshLine>();
        }

        /// <summary>
        /// Create mesh from verticies;
        /// </summary>
        /// <param name="vert"></param>
        public void mesh_create(MeshPoint[] vert)
        {
            Verticies = vert.ToList<MeshPoint>();
            MD_regener();
        }

        public void set_init_UVs(Vector2[] _UV_pos)
        {
            UV_poss = _UV_pos;
        }

        public void set_init_UVs(Vector3 UL, Vector3 DR)
        {
            UV_poss[0] = (Vector2)UL;
            UV_poss[1] = (Vector2)DR;
        }

        public void uv_cal_all()
        {
            foreach (MeshPoint MP in Verticies)
            {
                MP.uv_cal();
            }
        }

        //public void uv_cal(ref MeshPoint MP)
        //{
        //    float x = MP.pos.x - UV_poss[0].x;
        //    float y = MP.pos.y - UV_poss[0].y;
        //    float range_x = UV_poss[1].x - UV_poss[0].x;
        //    float range_y = UV_poss[1].y - UV_poss[0].y;
        //    float uv_x = x / range_x;
        //    float uv_y = y / range_y;
        //    MP.uv = new Vector2(uv_x, uv_y);
        //}

        public void to_mesh(ref Mesh mesh)
        {
            List<Vector2> UVs;
            List<Vector3> V_pos;
            MeshPoint.SepLists(Verticies, out V_pos, out UVs);

            mesh.vertices = V_pos.ToArray();
            mesh.triangles = Triangles.ToArray();
            mesh.uv = UVs.ToArray();
            //mesh.UploadMeshData(false);
        }

        public void MD_regener()
        {
            center_cal();
            sort_vert();
            line_regener();

        }

        /// <summary>
        /// Generate the virtual lines;
        /// Vertices need to be sorted first;
        /// </summary>
        private void line_regener()
        {
            Mesh_lines = new List<MeshLine>();
            MeshLine mesh_line;
            for (int i = 0; i < Verticies.Count - 1; i++)
            {
                mesh_line = new MeshLine();
                mesh_line.line_cal(Verticies[i], Verticies[i + 1]);
                Mesh_lines.Add(mesh_line);
            }
            mesh_line = new MeshLine();
            mesh_line.line_cal(Verticies[Verticies.Count - 1], Verticies[0]);
            Mesh_lines.Add(mesh_line);
        }

        /// <summary>
        /// Center position of the mesh;
        /// </summary>
        public void center_cal()
        {
            float ave_x = 0.0f;
            float ave_y = 0.0f;
            foreach (MeshPoint poi in Verticies)
            {
                ave_x += poi.pos.x;
                ave_y += poi.pos.y;
            }
            center_pos = new Vector2(ave_x / (float)Verticies.Count,
                                        ave_y / (float)Verticies.Count);
        }

        public void sort_vert()
        {
            Triangles = new List<int>();
            Verticies.Sort((x, y) => x.CompareTo(y, center_pos));
            Triangles = tria_regener(Verticies.Count);
        }

        /// <summary>
        /// Generate the triangles from verticies. Generate by clockwise;
        /// </summary>
        /// <param name="number">Number of vertices</param>
        /// <returns></returns>
        private List<int> tria_regener(int number)
        {
            List<int> triangles = new List<int>();
            for (int i = 1; i < number - 1; i++)
            {
                triangles.Add(0);
                triangles.Add(i);
                triangles.Add(i + 1);
            }
            return triangles;
        }

        /// <summary>
        /// Calculate the cut action. Generate 2 new MeshDatas;
        /// </summary>
        /// <param name="MP1"></param>
        /// <param name="MP2"></param>
        /// <returns>2 new MeshData {upper mesh, lower mesh}</returns>
        public MeshData[] cut(MeshPoint MP1, MeshPoint MP2)
        {
            MeshLine cut_line = new MeshLine();
            cut_line.line_cal(MP1, MP2);
            MP1.uv_cal();
            MP2.uv_cal();

            return points_cal(cut_line, MP1, MP2);
        }

        /// <summary>
        /// Calculate the two grouped points with the cut line; Separate mesh by upper half and lower half by vertices from the list;
        /// </summary>
        /// <param name="cut_line"></param>
        /// <param name="p1">Cut line start point</param>
        /// <param name="p2">Cut line end point</param>
        /// <returns>2 MeshData {upper/left mesh, lower/right mesh}</returns>
        private MeshData[] points_cal(MeshLine cut_line, MeshPoint p1, MeshPoint p2)
        {
            MeshData Fhalf = new MeshData();
            MeshData Shalf = new MeshData();

            List<MeshPoint> FHMP = new List<MeshPoint>();
            List<MeshPoint> SHMP = new List<MeshPoint>();

            foreach (MeshPoint vert in Verticies)
            {
                int pos_cal = cut_line.position_cal(vert);
                if (pos_cal == 1)
                {
                    FHMP.Add(vert);
                }
                else if (pos_cal == 0)
                {
                    FHMP.Add(vert.clone());
                    SHMP.Add(vert);
                }
                else
                {
                    SHMP.Add(vert);
                }
            }
            MeshPoint p1C = p1.clone();
            MeshPoint p2C = p2.clone();
            FHMP.Add(p1C);
            FHMP.Add(p2C);
            SHMP.Add(p1);
            SHMP.Add(p2);

            Fhalf.Verticies = FHMP;
            Shalf.Verticies = SHMP;

            Fhalf.MD_regener();
            Shalf.MD_regener();

            Fhalf.set_init_UVs(this.UV_poss);
            Shalf.set_init_UVs(this.UV_poss);

            return new MeshData[] { Fhalf, Shalf };
        }

        /// <summary>
        /// Calculate the intersection of the cut line and the mesh;
        /// </summary>
        /// <param name="CL"></param>
        /// <returns></returns>
        public MeshPoint[] line_inter_cal(MeshLine CL)
        {
            List<MeshPoint> mesh_points = new List<MeshPoint>();
            Vector3 pos;
            bool cornor = false;
            HashSet<Vector3> pos_set = new HashSet<Vector3>();
            foreach (MeshLine mesh_line in Mesh_lines)
            {
                pos = MeshLine.line_inter_cal(mesh_line, CL, out cornor);
                if (!(pos == CV.NANVector3))
                {
                    if (cornor)
                    {
                        if (pos_set.Contains(pos))
                        {
                            create_cut_p(ref mesh_points, pos);
                        }
                        else
                        {
                            pos_set.Add(pos);
                        }
                    }
                    else
                    {
                        create_cut_p(ref mesh_points, pos);
                    }
                }
            }
            return mesh_points.ToArray();
        }

        /// <summary>
        /// Create the cut MeshPoint from the position; And add it to the list;
        /// </summary>
        /// <param name="mesh_points"></param>
        /// <param name="pos"></param>
        private void create_cut_p(ref List<MeshPoint> mesh_points, Vector3 pos)
        {
            MeshPoint MP = new MeshPoint(pos, true, this);
            MP.uv_cal();
            mesh_points.Add(MP);
        }

        public MeshData clone()
        {
            MeshData MD = new MeshData();
            MD.Verticies = new List<MeshPoint>(Verticies.Select(x => x.clone()));
            MD.Triangles = new List<int>(Triangles);
            MD.target_TRANS = this.target_TRANS;
            MD.UV_poss = UV_poss;
            return MD;
        }

        public string VarToString()
        {
            string result = "";

            foreach (MeshPoint vert in Verticies)
            {
                result += " vert " + vert.VarToString();
            }
            result += " \n";
            foreach (int tria in Triangles)
            {
                result += " " + tria.ToString();
            }
            result += " \n";
            foreach (MeshLine mesh_line in Mesh_lines)
            {
                result += " line " + mesh_line.VarToString();
            }
            result += " \n";

            result += " UV_poss " + UV_poss[0].ToString("F2") + " " + UV_poss[1].ToString("F2");
            result += " \n";

            //Transform mesh_TRANS = RC.IS.MD_TRANS_DICT[this];
            //result += " trans " + mesh_TRANS.position;

            return result;
        }

        public void clean_destroy(Dictionary<MeshData,Transform> MD_pool)
        {
            MD_pool.Remove(this);
        }

    }

}
