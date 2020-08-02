using MeshSystem;
using System;
using System.Collections;
using System.Collections.Generic;
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

    private void Awake()
    {
        this.frame_width = 0.0f;
        this.frame_height = 0.0f;
        this.text_offsety = 0.0f;
        this.weight_scale = 0.0f;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void init_frame(float FW, float FH, float TOY, float _weight_scale = 0.0f)
    {
        frame_width = FW;
        frame_height = FH;
        text_offsety = TOY;
        weight_scale = _weight_scale;
    }

    public void adjust_results()
    {
        adjust_text(format_WS().ToString());
    }

    private int format_WS()
    {
        return Mathf.RoundToInt(weight_scale * 100);
    }

    private void adjust_text(string score)
    {
        ScoreText_TRANS.position += new Vector3(0.0f, frame_height / 2.0f + text_offsety, 0.0f);
        ScoreText_TRANS.GetComponent<TextMesh>().text = ScoreText_prefix + score.ToString() + ScoreText_postfix;
    }

    public void set_weight_sca(float WS)
    {
        weight_scale = WS;
    }

}
