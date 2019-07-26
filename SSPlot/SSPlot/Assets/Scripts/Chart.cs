using UnityEngine;
using EC;
using System.Collections.Generic;
using System;
using TMPro;
using UnityEngine.Rendering;

public class Chart : MonoBehaviour
{
    [SerializeField] private LineRenderer X_LR;
    [SerializeField] private LineRenderer Y_LR;
    [SerializeField] private LineRenderer EX1_LR;
    [SerializeField] private LineRenderer EX2_LR;
    [SerializeField] private Transform LeftCenter_TRANS;
    [SerializeField] private Transform RightCenter_TRANS;
    [SerializeField] private Transform UpCenter_TRANS;
    [SerializeField] private Transform DownCenter_TRANS;
    [SerializeField] private Transform UpLeft_TRANS;
    [SerializeField] private Transform BG_TRANS;
    [SerializeField] private Transform HeadText_TMP;

    private ChartModes chart_mode;
    private bool start_flag;
    private float proportion;
    private List<float> ys;
    private int dots_num;
    private List<Transform> dots_TRANSs;

    private void Awake()
    {
        this.chart_mode = ChartModes.HeadRotation;
        this.start_flag = false;
        this.proportion = 0.0f;
        this.ys = new List<float>();
        this.dots_num = 0;
        this.dots_TRANSs = new List<Transform>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //test_run();
        //move_dots();
        if(start_flag)
        {
            plot();
        }
    }

    private void test_run()
    {
        ys[0] = Mathf.Sin(DateTime.Now.Second) * proportion;
    }


    private void plot()
    {
        switch(chart_mode)
        {
            case ChartModes.EyeRotation:
                ys[0] = CoilData.IS.Left_eye_voltage.x * proportion;
                ys[1] = CoilData.IS.Left_eye_voltage.y * proportion;
                ys[2] = CoilData.IS.Right_eye_voltage.x * proportion;
                ys[3] = CoilData.IS.Right_eye_voltage.y * proportion;
                break;
            case ChartModes.HeadRotation:
                ys[0] = CoilData.IS.currentHeadOrientation.x * proportion;
                ys[1] = CoilData.IS.currentHeadOrientation.y * proportion;
                ys[2] = CoilData.IS.currentHeadOrientation.z * proportion;
                break;
            case ChartModes.HeadSpeed:
                ys[0] = CoilData.IS.currentHeadVelocity.x * proportion;
                ys[1] = CoilData.IS.currentHeadVelocity.y * proportion;
                ys[2] = CoilData.IS.currentHeadVelocity.z * proportion;
                break;
        }

        move_dots();
    }

    private void move_dots()
    {
        for(int i = 0;i < dots_num;i++)
        {
            dots_TRANSs[i].localPosition = new Vector3(dots_TRANSs[i].localPosition.x, 
                                            ys[i], dots_TRANSs[i].localPosition.z);
        }
    }

    public void init_chart(ChartModes _chart_mode)
    {
        chart_mode = _chart_mode;

        prop_cal();

        BG_TRANS.localScale =
            new Vector3(SSPlotSetting.IS.ChartSize.x, SSPlotSetting.IS.ChartSize.y, 1.0f);
        X_LR.SetPositions(
            new Vector3[] { LeftCenter_TRANS.position, RightCenter_TRANS.position });
        Y_LR.SetPositions(
            new Vector3[] { UpCenter_TRANS.position, DownCenter_TRANS.position });

        init_extra_lines();
        init_dots();
        HeadText_TMP.position = UpLeft_TRANS.position;
        HeadText_TMP.GetComponent<TextMeshPro>().text = chart_mode.ToString();
    }

    private void prop_cal()
    {
        switch(chart_mode)
        {
            case ChartModes.EyeRotation:
                proportion = (SSPlotSetting.IS.ChartSize.y / 2.0f) / 
                                SSPlotSetting.IS.EyeRotationMax;
                break;
            case ChartModes.HeadRotation:
                proportion = (SSPlotSetting.IS.ChartSize.y / 2.0f) /
                                SSPlotSetting.IS.HeadRotationMax;
                break;
            case ChartModes.HeadSpeed:
                proportion = (SSPlotSetting.IS.ChartSize.y / 2.0f) /
                                SSPlotSetting.IS.HeadSpeedMax;
                break;
        }
    }

    private void init_extra_lines()
    {
        switch(chart_mode)
        {
            case ChartModes.EyeRotation:
                break;
            case ChartModes.HeadRotation:
                break;
            case ChartModes.HeadSpeed:
                EX1_LR.enabled = true;
                Vector3 start_pos1 = new Vector3(LeftCenter_TRANS.position.x,
                                    SSPlotSetting.IS.HeadSpeedThreshold * proportion,
                                    LeftCenter_TRANS.position.z);
                Vector3 end_pos1 = new Vector3(RightCenter_TRANS.position.x,
                                    SSPlotSetting.IS.HeadSpeedThreshold * proportion,
                                    RightCenter_TRANS.position.z);
                EX1_LR.SetPositions(new Vector3[] { start_pos1, end_pos1 });
                EX2_LR.enabled = true;
                Vector3 start_pos2 = new Vector3(LeftCenter_TRANS.position.x,
                                    -SSPlotSetting.IS.HeadSpeedThreshold * proportion,
                                    LeftCenter_TRANS.position.z);
                Vector3 end_pos2 = new Vector3(RightCenter_TRANS.position.x,
                                    -SSPlotSetting.IS.HeadSpeedThreshold * proportion,
                                    RightCenter_TRANS.position.z);
                EX2_LR.SetPositions(new Vector3[] { start_pos2, end_pos2 });
                break;
        }
    }

    private void init_dots()
    {
        switch(chart_mode)
        {
            case ChartModes.EyeRotation:
                dots_num = 4;
                break;
            case ChartModes.HeadRotation:
                dots_num = 3;
                break;
            case ChartModes.HeadSpeed:
                dots_num = 3;
                break;
        }

        for(int i = 0;i < dots_num;i++)
        {
            Transform temp_TRANS =
                Instantiate(RC.IS.DotsPS_Prefab, RightCenter_TRANS.position, transform.rotation).transform;
            temp_TRANS.parent = transform;
            temp_TRANS.localPosition = new Vector3(temp_TRANS.localPosition.x, 
                                        temp_TRANS.localPosition.y, temp_TRANS.localPosition.z - 0.1f);
            ParticleSystem temp_PS = temp_TRANS.GetComponent<ParticleSystem>();
            ParticleSystem.MainModule temp_MM = temp_PS.main;
            temp_MM.startSpeed = SSPlotSetting.IS.PlotSpeed;
            temp_MM.startLifetime = SSPlotSetting.IS.ChartSize.x / SSPlotSetting.IS.PlotSpeed;
            Material[] mat_arr = temp_TRANS.GetComponent<Renderer>().materials;
            foreach(Material mat in mat_arr)
            {
                mat.color = SSPlotSetting.IS.ColorList[RC.DotReference];
            }
            temp_PS.Play();
            dots_TRANSs.Add(temp_TRANS);
            ys.Add(0.0f);
            RC.DotReference++;
            RC.DotReference %= SSPlotSetting.IS.ColorList.Count;
        }
    }

    public void start_chart()
    {
        start_flag = true;
    }
}
