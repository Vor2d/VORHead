using System.Collections.Generic;
using System;

[Serializable]
public class GameSetting
{
    public string Camera1_display;
    public string Camera2_display;
    public bool Using_curved_screen;
    public float Player_screen_cm;
    public float Screen_width_cm;
    public float CameraFOVFactor;

    public float GazeTime;
    public float RandomGazeTime;
    public float HideTime;
    public float ErrorTime;
    public float SpeedThreshold;
    public float StopWinodow;
    public float TargetChangeTime;
    public float TargetChangeTimeRRange;
    public bool UseAcuityIndicator;
    public int AcuityMode;  //enum {four_dir,eight_dir};
    public float AcuityFlashTime;
    public int AcuityChangeNumber;
    public float AcuityChangeUpPerc;
    public float AcuityChangeDownPerc;
    public List<int> AcuityList;
    public float StopSpeed;
    public List<float> PostDelayList;
    public int PostDelayNumber;
    public float PostDelayUpPC;
    public float PostDelayLoPC;
    public float PostDelayIncPC;
    public int PostDelayConvNum;
    public int PostDelayRepeatNum;

    public GameSetting()
    {
        this.Camera1_display = "0";
        this.Camera2_display = "1";
        this.Using_curved_screen = true;
        this.Player_screen_cm = 100.0f;
        this.Screen_width_cm = 100.0f;
        this.CameraFOVFactor = 1.0f;

        this.GazeTime = 2.0f;
        this.RandomGazeTime = 0.5f;
        this.HideTime = 0.1f;
        this.ErrorTime = 2.0f;
        this.SpeedThreshold = 10.0f;
        this.StopWinodow = 0.1f;
        this.TargetChangeTime = 2.0f;
        this.TargetChangeTimeRRange = 0.5f;
        this.UseAcuityIndicator = false;
        this.AcuityMode = 0;  //enum {four_dir,eight_dir};
        this.AcuityFlashTime = 0.1f;
        this.AcuityChangeNumber = 10;
        this.AcuityChangeUpPerc = 0.8f;
        this.AcuityChangeDownPerc = 0.5f;
        this.AcuityList = new List<int>();
        this.StopSpeed = 10.0f;
        this.PostDelayList = new List<float>();
        this.PostDelayNumber = 3;
        this.PostDelayUpPC = 0.6f;
        this.PostDelayLoPC = 0.6f;
        this.PostDelayIncPC = 0.5f;
        this.PostDelayConvNum = 3;
        this.PostDelayRepeatNum = 0;
    }

    [Obsolete("Not using txt file anymore")]
    public GameSetting(Dictionary<string,string> setting_dict)
    {
        set_preset_setting(setting_dict);
    }

    [Obsolete("Not using txt file anymore")]
    public void set_preset_setting(Dictionary<string, string> setting_dict)
    {
        try { Using_curved_screen = setting_dict["UsingCurvedScreen"] == "True"; }
        catch { Using_curved_screen = false; }
        try { Player_screen_cm = float.Parse(setting_dict["PlayerScreenCM"]); }
        catch { Player_screen_cm = 100.0f; }
        try { Screen_width_cm = float.Parse(setting_dict["ScreenWidthCM"]); }
        catch { Screen_width_cm = 100.0f; }
        try { Camera1_display = setting_dict["Camera1Display"]; }
        catch { Camera1_display = "0"; }
        try { Camera2_display = setting_dict["Camera2Display"]; }
        catch { Camera2_display = "1"; }


        try { GazeTime = float.Parse(setting_dict["GazeTime"]); }
        catch { GazeTime = 1.5f; }
        try { RandomGazeTime = float.Parse(setting_dict["RandomGazeTime"]); }
        catch { RandomGazeTime = 0.5f; }
        try { HideTime = float.Parse(setting_dict["HideTime"]); }
        catch { HideTime = 0.1f; }
        try { ErrorTime = float.Parse(setting_dict["ErrorTime"]); }
        catch { ErrorTime = 2.0f; }
        try { SpeedThreshold = float.Parse(setting_dict["SpeedThreshold"]); }
        catch { SpeedThreshold = 10.0f; }
        try { StopWinodow = float.Parse(setting_dict["StopWinodow"]); }
        catch { StopWinodow = 0.1f; }
        try { TargetChangeTime = float.Parse(setting_dict["TargetChangeTime"]); }
        catch { TargetChangeTime = 2.0f; }
        try { TargetChangeTimeRRange = float.Parse(setting_dict["TargetChangeTimeRRange"]); }
        catch { TargetChangeTimeRRange = 0.5f; }
        try { UseAcuityIndicator = (setting_dict["UseAcuityIndicator"] == "True"); }
        catch { UseAcuityIndicator = true; }
        try { AcuityMode = int.Parse(setting_dict["AcuityMode"]); }
        catch { AcuityMode = 0; }
        try { AcuityFlashTime = float.Parse(setting_dict["AcuityFlashTime"]); }
        catch { AcuityFlashTime = 0.1f; }
    }

    public string VarToString()
    {
        string result_str = "";

        result_str += "Using_curved_screen" + " " + Using_curved_screen.ToString() + " ";
        result_str += "Player_screen_cm" + " " + Player_screen_cm.ToString() + " ";
        result_str += "Screen_width_cm" + " " + Screen_width_cm.ToString() + " ";
        result_str += "Camera1_display" + " " + Camera1_display.ToString() + " ";
        result_str += "Camera2_display" + " " + Camera2_display.ToString() + " ";
        result_str += "CameraFOVFactor" + " " + CameraFOVFactor.ToString() + " ";

        result_str += "GazeTime" + " " + GazeTime.ToString() + " ";
        result_str += "RandomGazeTime" + " " + RandomGazeTime.ToString() + " ";
        result_str += "HideTime" + " " + HideTime.ToString() + " ";
        result_str += "SpeedThreshold" + " " + SpeedThreshold.ToString() + " ";
        result_str += "StopWinodow" + " " + StopWinodow.ToString() + " ";
        result_str += "TargetChangeTime" + " " + TargetChangeTime.ToString() + " ";
        result_str += "TargetChangeTimeRRange" + " " + TargetChangeTimeRRange.ToString() + " ";
        result_str += "UseAcuityIndicator" + " " + UseAcuityIndicator.ToString() + " ";
        result_str += "AcuityMode" + " " + AcuityMode.ToString() + " ";
        result_str += "AcuityFlashTime" + " " + AcuityFlashTime.ToString() + " ";
        result_str += "AcuityChangeNumber" + " " + AcuityChangeNumber.ToString() + " ";
        result_str += "AcuityChangeUpPerc" + " " + AcuityChangeUpPerc.ToString() + " ";
        result_str += "AcuityChangeDownPerc" + " " + AcuityChangeDownPerc.ToString() + " ";
        result_str += "ACuityList" + " " + AcuityList.Count.ToString() + " ";
        result_str += "StopSpeed" + " " + StopSpeed.ToString("F2") + " ";
        result_str += "PostDelayList" + " " + PostDelayList.Count.ToString() + " ";
        result_str += "PostDelayNumber" + " " + PostDelayNumber.ToString() + " ";
        result_str += "PostDelayUpC" + " " + PostDelayUpPC.ToString("F2") + " ";
        result_str += "PostDelayLoC" + " " + PostDelayLoPC.ToString("F2") + " ";
        result_str += "PostDelayConvNum" + " " + PostDelayConvNum.ToString() + " ";
        result_str += "PostDelayRepeatNum" + " " + PostDelayRepeatNum.ToString() + " ";


        return result_str;
    }
}
