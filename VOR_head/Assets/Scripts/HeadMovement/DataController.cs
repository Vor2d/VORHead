using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class DataController : MonoBehaviour {

    private const string turn_path = "Default.txt";
    private const string setting_path = "SettingDefault.txt";

    public List<float> turn_data { get; set; }
    public List<float> jump_data { get; set; }
    
    //Game Mode
    public bool HideFlag { get; set; }
    public bool JumpFlag { get; set; }
    public bool ShowTargetFlag { get; set; }
    public bool HeadIndicatorChange { get; set; }
    public bool SkipCenterFlag { get; set; }
    public bool Loop_trial_flag { get; set; }
    public bool HideHeadIndicator { get; set; }
    //Variables;
    public float GazeTime { get; set; }
    public float RandomGazeTime { get; set; }
    public float HideTime { get; set; }
    //public float HideTimeRandom { get; set; }
    public float ErrorTime { get; set; }
    public float SpeedThreshold { get; set; }
    public float StopWinodow { get; set; }
    public float Player_screen_cm { get; set; }
    public float Screen_width_cm { get; set; }
    public string Camera1_display { get; set; }
    public string Camera2_display { get; set; }
    public int Loop_number { get; set; }
    //Other data;
    public bool UsingCoilFlag { get; set; }
    public bool UsingVRFlag { get; set; }
    public Quaternion Head_origin { get; set; }
    public float Gain { get; set; }

    private Dictionary<string, string> init_data;

    void Awake()
    {
        DontDestroyOnLoad(this);

        this.turn_data = new List<float>();
        this.jump_data = new List<float>();
        this.init_data = new Dictionary<string, string>();
        this.Head_origin = new Quaternion(0.0f,0.0f,0.0f,1.0f);

        try
        {
            StreamReader reader = new StreamReader(setting_path);
            while (!reader.EndOfStream)
            {
                string[] splitstr = reader.ReadLine().Split(new char[]{' ', '\t'});
                init_data.Add(splitstr[0], splitstr[1]);
            }
            reader.Close();

            this.Loop_trial_flag = (init_data["LoopTrialFlag"] == "True");
            this.Player_screen_cm = float.Parse(init_data["PlayerScreenCM"]);
            this.Screen_width_cm = float.Parse(init_data["ScreenWidthCM"]);
            this.Camera1_display = init_data["Camera1Display"];
            this.Camera2_display = init_data["Camera2Display"];
            this.GazeTime = float.Parse(init_data["GazeTime"]);
            this.RandomGazeTime = float.Parse(init_data["RandomGazeTime"]);
            this.HideTime = float.Parse(init_data["HideTime"]);
            this.ErrorTime = float.Parse(init_data["ErrorTime"]);
            this.SpeedThreshold = float.Parse(init_data["SpeedThreshold"]);
            this.StopWinodow = float.Parse(init_data["StopWinodow"]);
            this.HideFlag = (init_data["HideFlag"] == "True");
            this.JumpFlag = (init_data["JumpFlag"] == "True");
            this.ShowTargetFlag = (init_data["ShowTargetFlag"] == "True");
            this.HeadIndicatorChange = (init_data["HeadIndicatorChangeFlag"] == "True");
            this.SkipCenterFlag = (init_data["SkipCenterFlag"] == "True");
            //this.HideTimeRandom = float.Parse(init_data["HideTimeRandom"]);
            this.Loop_number = Int32.Parse(init_data["LoopNumber"]);
            this.Gain = float.Parse(init_data["Gain"]);
            this.HideHeadIndicator = (init_data["HideHeadIndicator"] == "True");
        }
        catch(Exception e)
        {
            Debug.Log(e);

            this.Loop_trial_flag = true;
            this.Player_screen_cm = 100.0f;
            this.Screen_width_cm = 50f;
            this.Camera1_display = "0";
            this.Camera2_display = "1";
            this.GazeTime = 3.0f;
            this.RandomGazeTime = 0.0f;
            this.HideTime = 1.0f;
            this.ErrorTime = 1.5f;
            this.SpeedThreshold = 50.0f;
            this.StopWinodow = 0.5f;
            this.HideFlag = false;
            this.JumpFlag = false;
            this.ShowTargetFlag = false;
            this.HeadIndicatorChange = false;
            this.SkipCenterFlag = false;
            //this.HideTimeRandom = 0.0f;
            this.Loop_number = -1;
            this.Gain = 1.0f;
            this.HideHeadIndicator = false;
        }

        this.UsingCoilFlag = false;
        this.UsingVRFlag = true;

        List<float> turn_data_temp = new List<float>();
        List<float> jump_data_temp = new List<float>();
        GeneralMethods.load_turn_jump_data_general(turn_path, out turn_data_temp, out jump_data_temp);
        turn_data = turn_data_temp;
        jump_data = jump_data_temp;


    }

    public string VarToString()
    {
        string result_str = "";

        //        //Game Mode
        //public bool HideFlag { get; set; }
        //public bool JumpFlag { get; set; }
        //public bool ShowTargetFlag { get; set; }
        //public bool HeadIndicatorChange { get; set; }
        //public bool SkipCenterFlag { get; set; }
        //public bool Loop_trial_flag { get; set; }
        ////Variables;
        //public float GazeTime { get; set; }
        //public float HideTime { get; set; }
        //public float HideTimeRandom { get; set; }
        //public float ErrorTime { get; set; }
        //public float SpeedThreshold { get; set; }
        //public float StopWinodow { get; set; }
        //public float Player_screen_cm { get; set; }
        //public float Screen_width_cm { get; set; }
        //public string Camera1_display { get; set; }
        //public string Camera2_display { get; set; }
        //public int Loop_number { get; set; }

        result_str += "HideFlag" + " " + HideFlag.ToString() + " ";
        result_str += "JumpFlag" + " " + JumpFlag.ToString() + " ";
        result_str += "ShowTargetFlag" + " " + ShowTargetFlag.ToString() + " ";
        result_str += "HeadIndicatorChange" + " " + HeadIndicatorChange.ToString() + " ";
        result_str += "SkipCenterFlag" + " " + SkipCenterFlag.ToString() + " ";
        result_str += "Loop_trial_flag" + " " + Loop_trial_flag.ToString() + " ";
        result_str += "GazeTime" + " " + GazeTime.ToString() + " ";
        result_str += "RandomGazeTime" + " " + RandomGazeTime.ToString() + " ";
        result_str += "HideTime" + " " + HideTime.ToString() + " ";
        //result_str += "HideTimeRandom" + " " + HideTimeRandom.ToString() + " ";
        result_str += "SpeedThreshold" + " " + SpeedThreshold.ToString() + " ";
        result_str += "StopWinodow" + " " + StopWinodow.ToString() + " ";
        result_str += "Player_screen_cm" + " " + Player_screen_cm.ToString() + " ";
        result_str += "Screen_width_cm" + " " + Screen_width_cm.ToString() + " ";
        result_str += "Camera1_display" + " " + Camera1_display.ToString() + " ";
        result_str += "Camera2_display" + " " + Camera2_display.ToString() + " ";
        result_str += "Loop_number" + " " + Loop_number.ToString() + " ";
        result_str += "Gain" + " " + Gain.ToString() + " ";

        return result_str;
    }


}
