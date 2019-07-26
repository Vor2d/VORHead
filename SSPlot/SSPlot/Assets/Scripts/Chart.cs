using UnityEngine;
using EC;

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
    [SerializeField] private Transform BG_TRANS;
    [SerializeField] private Transform DotsPS_TRANS;

    private ChartModes chart_mode;
    private bool start_flag;
    private float proportion;
    private float y;
    private int plot_num;

    private void Awake()
    {
        this.chart_mode = ChartModes.HeadRotation;
        this.start_flag = false;
        this.proportion = 0.0f;
        this.y = 0.0f;
        this.plot_num = 0;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void plot()
    {
        switch(chart_mode)
        {
            case ChartModes.EyeRotation:
                y = 
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

    public void start_chart()
    {
        start_flag = true;
    }
}
