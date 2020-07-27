using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FS_CutResult : MonoBehaviour
{
    [SerializeField] private Transform CurLineRender_TRANS;
    [SerializeField] private Transform StraLineRender_TRANS;
    [SerializeField] private Transform EndP_TRANS;
    [SerializeField] private Transform RText_TRANS;
    [SerializeField] private string Dist_score_prefix;
    [SerializeField] private string Len_score_prefix;

    private bool record_started;
    private List<Vector3> cut_poss;

    private void Awake()
    {
        this.record_started = false;
        this.cut_poss = new List<Vector3>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (record_started) { record_path(); }
    }

    public void init_CR(float LR_width, float SLR_width)
    {
        CurLineRender_TRANS.GetComponent<LineRenderer>().startWidth = LR_width;
        CurLineRender_TRANS.GetComponent<LineRenderer>().endWidth = LR_width;
        CurLineRender_TRANS.GetComponent<LineRenderer>().enabled = false;
        StraLineRender_TRANS.GetComponent<LineRenderer>().startWidth = SLR_width;
        StraLineRender_TRANS.GetComponent<LineRenderer>().endWidth = SLR_width;
        StraLineRender_TRANS.GetComponent<LineRenderer>().enabled = false;
    }

    public void start_record()
    {
        record_started = true;
    }

    public void stop_record()
    {
        record_started = false;
    }

    private void record_path()
    {
        Vector3 hit_point = new Vector3();
        if (FS_RC.IS.RC_script.check_object_pos(FS_SD.FruitPlane_tag, out hit_point))
        { cut_poss.Add(hit_point); }
    }

    public void generate_path()
    {
        LineRenderer LR = CurLineRender_TRANS.GetComponent<LineRenderer>();
        LR.positionCount = cut_poss.Count;
        LR.SetPositions(cut_poss.ToArray());
        LR.enabled = true;
    }

    public void generate_stra_path(Vector3 sta_pos, Vector3 end_pos)
    {
        LineRenderer LR = StraLineRender_TRANS.GetComponent<LineRenderer>();
        LR.positionCount = 2;
        LR.SetPositions(new Vector3[]{ sta_pos, end_pos});
        LR.enabled = true;
        EndP_TRANS.position = end_pos;
        EndP_TRANS.GetComponent<MeshRenderer>().enabled = true;
    }

    public void turn_off_mesh()
    {
        CurLineRender_TRANS.GetComponent<LineRenderer>().enabled = false;
        StraLineRender_TRANS.GetComponent<LineRenderer>().enabled = false;
        EndP_TRANS.GetComponent<MeshRenderer>().enabled = false;
        RText_TRANS.GetComponent<MeshRenderer>().enabled = false;
    }

    public void clear_path()
    {
        record_started = false;
        turn_off_mesh();
        cut_poss.Clear();
    }

    public float cur_len_cal()
    {
        float len = 0.0f;
        for(int i = 0;i<cut_poss.Count-1;i++)
        {
            len += Vector3.Distance(cut_poss[i], cut_poss[i + 1]);
        }
        return len;
    }

    public void show_text_result(float dist_score, float len_score,Vector3 pos)
    {
        RText_TRANS.GetComponent<TextMesh>().text = Dist_score_prefix +
            Mathf.RoundToInt(dist_score).ToString() + "\n" + Len_score_prefix +
            Mathf.RoundToInt(len_score).ToString();
        RText_TRANS.position = pos;
        RText_TRANS.GetComponent<MeshRenderer>().enabled = true;
    }
}
