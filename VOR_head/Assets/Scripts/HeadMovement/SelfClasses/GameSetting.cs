using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameSetting
{
    public string Camera1_display { get; set; }
    public string Camera2_display { get; set; }
    public float Player_screen_cm { get; set; }
    public float Screen_width_cm { get; set; }

    public float GazeTime { get; set; }
    public float RandomGazeTime { get; set; }
    public float HideTime { get; set; }
    public float ErrorTime { get; set; }
    public float SpeedThreshold { get; set; }
    public float StopWinodow { get; set; }
    public float TargetChangeTime { get; set; }

    public GameSetting()
    {
        set_preset_setting(new Dictionary<string, string>());
    }

    public GameSetting(Dictionary<string,string> setting_dict)
    {
        set_preset_setting(setting_dict);
    }

    public void set_preset_setting(Dictionary<string, string> setting_dict)
    {
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
    }

    public string VarToString()
    {
        string result_str = "";

        result_str += "Player_screen_cm" + " " + Player_screen_cm.ToString() + " ";
        result_str += "Screen_width_cm" + " " + Screen_width_cm.ToString() + " ";
        result_str += "Camera1_display" + " " + Camera1_display.ToString() + " ";
        result_str += "Camera2_display" + " " + Camera2_display.ToString() + " ";

        result_str += "GazeTime" + " " + GazeTime.ToString() + " ";
        result_str += "RandomGazeTime" + " " + RandomGazeTime.ToString() + " ";
        result_str += "HideTime" + " " + HideTime.ToString() + " ";
        result_str += "SpeedThreshold" + " " + SpeedThreshold.ToString() + " ";
        result_str += "StopWinodow" + " " + StopWinodow.ToString() + " ";
        result_str += "TargetChangeTime" + " " + TargetChangeTime.ToString() + " ";


        return result_str;
    }
}
