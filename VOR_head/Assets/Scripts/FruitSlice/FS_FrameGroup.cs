using System.Collections;
using UnityEngine;

public class FS_FrameGroup : MonoBehaviour
{
    [SerializeField] private Transform ScoreText_TRANS;
    [SerializeField] private string ScoreText_prefix;
    [SerializeField] private string ScoreText_postfix;

    private float frame_width;
    private float frame_height;
    private float text_offsety;
    private float weight_scale;
    private float font_size;

    private void Awake()
    {
        this.frame_width = 0.0f;
        this.frame_height = 0.0f;
        this.text_offsety = 0.0f;
        this.weight_scale = 0.0f;
        this.font_size = 0.0f;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void init_frame(float FW, float FH, float TOY, float FS, float _weight_scale = 0.0f)
    {
        frame_width = FW;
        frame_height = FH;
        text_offsety = TOY;
        font_size = FS;
        weight_scale = _weight_scale;
    }

    public void adjust_results(bool text_ANI = false, float ANI_time = 0.0f)
    {
        adjust_text(format_WS().ToString(), text_ANI: text_ANI, ANI_time: ANI_time);
    }

    private int format_WS()
    {
        return Mathf.RoundToInt(weight_scale * 100);
    }

    private int format_WS(float WS)
    {
        return Mathf.RoundToInt(WS * 100);
    }

    private void adjust_text(string score, bool text_ANI = false, float ANI_time = 0.0f)
    {
        ScoreText_TRANS.position += new Vector3(0.0f, frame_height / 2.0f + text_offsety, 0.0f);
        if (text_ANI) { start_text_ANI(weight_scale, ANI_time); }
        else
        {
            ScoreText_TRANS.GetComponent<TextMesh>().text = ScoreText_prefix + score.ToString() + 
                ScoreText_postfix;
        }
        ScoreText_TRANS.GetComponent<TextMesh>().characterSize = font_size;
    }

    public void set_weight_sca(float WS)
    {
        weight_scale = WS;
    }

    private void start_text_ANI(float score, float time)
    {
        StartCoroutine(text_ANI(ScoreText_prefix, ScoreText_postfix, score, time,
            ScoreText_TRANS.GetComponent<TextMesh>()));
    }

    private IEnumerator text_ANI(string prefix, string postfix, float score, float time, TextMesh TM)
    {
        float speed = score / time;
        bool running = true;
        float curr_scr = 0.0f;
        while(running)
        {
            if (curr_scr >= score) 
            {
                running = false;
                break;
            }
            curr_scr += speed * Time.deltaTime;
            TM.text = prefix + format_WS(curr_scr).ToString() + postfix;
            yield return null;
        }
    }

    public void clean_destroy()
    {
        Destroy(gameObject);
    }
}
