using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MeshSystem;
using UnityEngine.UI;
using System.Linq;

public class FS_FruitMesh : MonoBehaviour
{
    [SerializeField] private bool Infinite_cut;
    [SerializeField] string Shader_str;
    [SerializeField] bool Force_prob_Tomass;

    private Texture2D curr_tex;
    private List<List<Transform>> cutted_TRANSs;
    

    private void Awake()
    {
        this.curr_tex = null;
        this.cutted_TRANSs = new List<List<Transform>>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void first_create_mesh(Vector2[] poss,Texture2D texture2D)
    {
        clear_mesh();
        set_curr_tex(texture2D);
        create_mesh_Fposs(poss);
    }

    /// <summary>
    /// Create the mesh with ractangle points;
    /// </summary>
    /// <param name="poss">{upper left point, down right point}</param>
    private void create_mesh_Fposs(Vector2[] poss)
    {
        Vector3[] uvs = new Vector3[] {
            new Vector3(poss[0].x, poss[1].y, 0.0f), new Vector3(poss[1].x, poss[0].y, 0.0f) };
        List<Vector3> points = new List<Vector3>();
        points.Add(new Vector3(poss[0].x, poss[0].y, 0.0f));
        points.Add(new Vector3(poss[0].x, poss[1].y, 0.0f));
        points.Add(new Vector3(poss[1].x, poss[0].y, 0.0f));
        points.Add(new Vector3(poss[1].x, poss[1].y, 0.0f));
        
        MeshCreater.create_mesh(points.ToArray(), uvs, curr_tex, transform, trans_pos: Vector3.zero, 
            shader_str: Shader_str);
    }

    private void set_curr_tex(Texture2D texture2D)
    {
        curr_tex = texture2D;
    }




    public void cut_mseh(Vector3 start_pos, Vector3 stop_pos, bool Using_rigidbody = false)
    {
        cutted_TRANSs = MeshCutter.get_lines_Acut(start_pos, stop_pos, FS_RC.IS.MeshDataPool, Infinite_cut,
            curr_tex, transform, shader_str: Shader_str);
        if (Using_rigidbody) { move_cut_RB(); }
        else { move_cut_translate(start_pos, stop_pos); }
    }

    private void move_cut_RB()
    {
        foreach (List<Transform> c_TRANSs in cutted_TRANSs)
        {
            apply_CO_RB(c_TRANSs[0]);
            apply_CO_RB(c_TRANSs[1]);
        }
    }

    private void apply_CO_RB(Transform mesh_TRANS)
    {
        PolygonCollider2D MC = mesh_TRANS.gameObject.AddComponent<PolygonCollider2D>();
        edit_PC(MC, mesh_TRANS);
        Rigidbody2D RB = mesh_TRANS.gameObject.AddComponent<Rigidbody2D>();
        edit_RB(RB);
        apply_force(RB);
    }

    private void apply_force(Rigidbody2D RB)
    {
        float force = FS_Setting.IS.ThrowForce + 
            Random.Range(-FS_Setting.IS.ForceRange, FS_Setting.IS.ForceRange);
        float torque = FS_Setting.IS.ThrowTorque +
            Random.Range(-FS_Setting.IS.TorqueRange, FS_Setting.IS.TorqueRange);
        if(Force_prob_Tomass)
        {
            force *= RB.mass;
            torque *= RB.mass;
        }
        if (FS_Setting.IS.ApplyForce)
        {
            RB.AddForce(Vector2.up * force, ForceMode2D.Impulse);
            RB.AddTorque(torque, ForceMode2D.Impulse);
        }
    }

    private void edit_PC(PolygonCollider2D PC,Transform mesh_TRANS)
    {
        MeshData MD = get_MD_from_pool(mesh_TRANS);
        PC.points = MD.get_vert_tri_uv_2d().Item1;
    }

    private void edit_RB(Rigidbody2D RB)
    {
        Rigidbody2D RB_prefab = FS_RC.IS.RB_sam_Prefab.GetComponent<Rigidbody2D>();
        RB.useAutoMass = RB_prefab.useAutoMass;
        RB.mass = RB_prefab.mass;
        RB.constraints = RB_prefab.constraints;
    }

    private MeshData get_MD_from_pool(Transform mesh_TRANS)
    {
        return FS_RC.IS.MeshDataPool.FirstOrDefault(x => x.Value == mesh_TRANS).Key;
    }

    private void move_cut_translate(Vector3 start_pos, Vector3 stop_pos)
    {
        Vector2 cut_vec = (stop_pos - start_pos).normalized;
        Vector2 updir_vec = get_perpen(cut_vec, true);
        Vector2 downdir_vec = get_perpen(cut_vec, false);
        float speed = FS_DataController.IS.GameSetting.CutHalfMoveSpeed, 
            dist = FS_DataController.IS.GameSetting.CutHalfMoveDist;
        foreach (List<Transform> c_TRANSs in cutted_TRANSs)
        {
            StartCoroutine(move_piece(c_TRANSs[0], updir_vec, speed, dist));
            StartCoroutine(move_piece(c_TRANSs[1], downdir_vec, speed, dist));
        }
        //cutted_TRANSs.Clear();
    }

    private IEnumerator move_piece(Transform trans,Vector3 dir, float speed, float dist)
    {
        float timer = 0.0f;
        float c_dist = 0.0f;
        Vector3 move_vec = new Vector3();
        while (c_dist < dist)
        {
            if (trans == null) { break; }
            move_vec = dir * speed * Time.deltaTime;
            c_dist += move_vec.magnitude;
            timer += Time.deltaTime;
            trans.Translate(move_vec);
            yield return null;
        }
    }

    /// <summary>
    /// Return the perpendicular vector to the cut vector, based on the upper/left direction; 
    /// </summary>
    /// <param name="vec2"></param>
    /// <param name="upper"></param>
    /// <returns></returns>
    private Vector2 get_perpen(Vector2 vec2,bool upper)
    {
        Vector2[] perpen_vec = GeneralMethods.get_perpen_vec(vec2);
        if(upper)
        {
            return perpen_vec[0].y == 0.0f ? (perpen_vec[0].x < 0.0f ? perpen_vec[0] : perpen_vec[1]) :
                (perpen_vec[0].y > 0.0f ? perpen_vec[0] : perpen_vec[1]);
        }
        else
        {
            return perpen_vec[0].y == 0.0f ? (perpen_vec[0].x < 0.0f ? perpen_vec[1] : perpen_vec[0]) :
                (perpen_vec[0].y > 0.0f ? perpen_vec[1] : perpen_vec[0]);
        }
    }

    public void clear_mesh()
    {
        foreach (KeyValuePair<MeshData, Transform> MT_en in FS_RC.IS.MeshDataPool)
        {
            Destroy(MT_en.Value.gameObject);
        }
        FS_RC.IS.MeshDataPool.Clear();
    }
}
