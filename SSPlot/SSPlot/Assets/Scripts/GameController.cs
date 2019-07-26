using UnityEngine;
using EC;
using System.IO;
using System;

public class GameController : MonoBehaviour
{
    [SerializeField] private string setting_path;
    [SerializeField] private string setting_file_name;

    private void Awake()
    {
        load_setting();
    }

    // Start is called before the first frame update
    void Start()
    {
        init_charts();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            recenter_VR();
        }
    }

    private void load_setting()
    {
        SSPlotSetting SSPS;
        try
        {
            SSPS = new SSPlotSetting();
            string from_json = File.ReadAllText(setting_path + setting_file_name);
            SSPS = JsonUtility.FromJson<SSPlotSetting>(from_json);
            Debug.Log("Loading settings succeed!!");
        }
        catch (Exception e) { Debug.Log("Reading settings error " + e); SSPS = new SSPlotSetting(); }
    }

    private void init_charts()
    {
        int index = 0;
        foreach(ChartModes chart_mode in SSPlotSetting.IS.ChartModesList)
        {
            Vector3[] pos_dir = pos_dir_cal(index);
            Transform temp_TRANS = 
                Instantiate(RC.IS.Chart_Prefab, pos_dir[0], Quaternion.Euler(pos_dir[1])).transform;
            RC.IS.Charts_TRANSs.Add(temp_TRANS);
            Chart temp_chart = temp_TRANS.GetComponent<Chart>();
            temp_chart.init_chart(SSPlotSetting.IS.ChartModesList[index]);
            index++;
            temp_chart.start_chart();
        }
    }

    private Vector3[] pos_dir_cal(int index)
    {
        float frac_num = (SSPlotSetting.IS.HorizontalNum - 1) / 2.0f;
        float init_degree = -(frac_num * SSPlotSetting.IS.HorizontalDegree);
        float curr_degree = init_degree + (index % SSPlotSetting.IS.HorizontalNum)
                                            * SSPlotSetting.IS.HorizontalDegree;
        float curr_raid = curr_degree * Mathf.PI / 180.0f;
        float x = Mathf.Sin(curr_raid) * SSPlotSetting.IS.InitZ;
        float z1 = Mathf.Cos(curr_raid) * SSPlotSetting.IS.InitZ;
        float frac_num_y = Mathf.Ceil((float)SSPlotSetting.IS.ChartModesList.Count /
                            (float)SSPlotSetting.IS.HorizontalNum);
        frac_num_y = (frac_num_y - 1) / 2.0f;
        float init_degree_y = -(frac_num_y * SSPlotSetting.IS.VerticalDegree);
        float curr_degree_y = init_degree_y + Mathf.Floor((float)index / (float)SSPlotSetting.IS.HorizontalNum)
                                                * SSPlotSetting.IS.VerticalDegree;
        float curr_raid_y = curr_degree_y * Mathf.PI / 180.0f;
        float y = Mathf.Sin(curr_raid_y) * SSPlotSetting.IS.InitZ;
        float z2 = Mathf.Cos(curr_raid_y) * SSPlotSetting.IS.InitZ;
        Vector3 pos = new Vector3(x, y, (z1 + z2) / 2.0f);
        Vector3 dir = new Vector3(-curr_degree_y, curr_degree, 0.0f);
        return new Vector3[] { pos, dir };
    }

    private void recenter_VR()
    {
        UnityEngine.XR.InputTracking.Recenter();
    }
}
