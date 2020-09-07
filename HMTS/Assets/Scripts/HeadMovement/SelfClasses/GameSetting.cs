using System.Collections.Generic;
using System;

[Serializable]
public class GameSetting
{
    public string Camera1_display;  //Main: virtual camera; 1: Main capture camera; 2: Setting camera; 3: Second camture camera;
    public string Camera2_display;
    public string Camera3_display;
    public bool Using_curved_screen;
    public float Player_screen_cm;
    public float Screen_width_cm;
    public float MainCameraFOV;
    public float Camera1FOV;
    public float Camera2FOV;
    public float Camera3FOV;
    public float MCPPI;
    public int ScreenResoH;
    public int ScreenResoV;

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
    public int PostDelayASOffset;
    public int DynaDelayRepeatNum;
    public int DynaStopThresh;
    public float SecondHeadSpeed;
    public float Cam1Angle;
    public float Cam3Angle;
    public float RotationScale;
    public float DistScale;
    public float HIOCRange;
    public int MLHMaxSize;
    public int MLHSinRepeatNum;
    public int MLHDouRepeatNum;

    public GameSetting()
    {
        this.Camera1_display = "0";
        this.Camera2_display = "1";
        this.Camera3_display = "3";
        this.Using_curved_screen = true;
        this.Player_screen_cm = 100.0f;
        this.Screen_width_cm = 100.0f;
        this.MainCameraFOV = 120.0f;
        this.Camera1FOV = 60.0f;
        this.Camera2FOV = 60.0f;
        this.Camera3FOV = 60.0f;
        this.MCPPI = 80.6f;

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
        this.PostDelayASOffset = 0;
        this.DynaDelayRepeatNum = 0;
        this.DynaStopThresh = Int32.MaxValue; //default as no threshold;
        this.SecondHeadSpeed = 0.0f;
        this.Cam1Angle = 0.0f;
        this.Cam3Angle = 0.0f;
        this.RotationScale = 1.0f;
        this.DistScale = 1.0f;
        this.HIOCRange = 5760.0f;
        this.MLHMaxSize = 9;
        this.MLHSinRepeatNum = 3;
        this.MLHDouRepeatNum = 15;
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
        result_str += "Camera3_display" + " " + Camera3_display.ToString() + " ";
        result_str += "Camera1FOV" + " " + Camera1FOV.ToString() + " ";
        result_str += "Camera2FOV" + " " + Camera2FOV.ToString() + " ";
        result_str += "Camera3FOV" + " " + Camera3FOV.ToString() + " ";
        result_str += "MainCameraFOV" + " " + MainCameraFOV.ToString() + " ";
        result_str += "MCPPI" + " " + MCPPI.ToString() + " ";

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
        result_str += "PostDelayASOffset" + " " + PostDelayASOffset.ToString() + " ";
        result_str += "DynaDelayRepeatNum" + " " + DynaDelayRepeatNum.ToString() + " ";
        result_str += "DynaStopThresh" + " " + DynaStopThresh.ToString() + " ";
        result_str += "SecondHeadSpeed" + " " + SecondHeadSpeed.ToString() + " ";
        result_str += "Cam1Angle" + " " + Cam1Angle.ToString("F2") + " ";
        result_str += "Cam3Angle" + " " + Cam3Angle.ToString("F2");
        result_str += "RotationScale" + " " + RotationScale.ToString("F2") + " ";
        result_str += "DistScale" + " " + DistScale.ToString("F2") + " ";
        result_str += "HIOCRange" + " " + HIOCRange.ToString("F2") + " ";
        result_str += "MLHMaxSize" + " " + MLHMaxSize.ToString() + " ";
        result_str += "MLHSinRepeatNum" + " " + MLHSinRepeatNum.ToString() + " ";
        result_str += "MLHDouRepeatNum" + " " + MLHDouRepeatNum.ToString() + " ";



        return result_str;
    }
}
