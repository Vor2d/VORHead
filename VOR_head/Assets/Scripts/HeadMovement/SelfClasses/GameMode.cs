using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMode
{
    public enum GameModeEnum { Default,A1,A2,B1}

    //Game Flag
    public bool HideFlag { get; set; }
    public bool JumpFlag { get; set; }
    public bool ShowTargetFlag { get; set; }
    public bool HeadIndicatorChange { get; set; }
    public bool SkipCenterFlag { get; set; }
    public bool HideHeadIndicator { get; set; }
    //Variables;
    public float GazeTime { get; set; }
    public float RandomGazeTime { get; set; }
    public float HideTime { get; set; }
    public float ErrorTime { get; set; }
    public float SpeedThreshold { get; set; }
    public float StopWinodow { get; set; }
    public float Gain { get; set; }

    public GameMode()
    {
        set_preset_mode(GameModeEnum.Default);
        set_preset_para(new Dictionary<string, string>());
    }

    public GameMode(GameMode other_GM)
    {
        this.HideFlag = other_GM.HideFlag;
        this.JumpFlag = other_GM.JumpFlag;
        this.ShowTargetFlag = other_GM.ShowTargetFlag;
        this.HeadIndicatorChange = other_GM.HeadIndicatorChange;
        this.SkipCenterFlag = other_GM.SkipCenterFlag;
        this.HideHeadIndicator = other_GM.HideHeadIndicator;
        this.GazeTime = other_GM.GazeTime;
        this.RandomGazeTime = other_GM.RandomGazeTime;
        this.HideTime = other_GM.HideTime;
        this.ErrorTime = other_GM.ErrorTime;
        this.SpeedThreshold = other_GM.SpeedThreshold;
        this.StopWinodow = other_GM.StopWinodow;
        this.Gain = other_GM.Gain;
    }

    public void set_preset_mode(GameModeEnum GM_preset)
    {
        switch(GM_preset)
        {
            case GameModeEnum.Default:
                {
                    HideFlag = false;
                    JumpFlag = false;
                    ShowTargetFlag = false;
                    HeadIndicatorChange = false;
                    SkipCenterFlag = false;
                    HideHeadIndicator = false;
                    break;
                }
            case GameModeEnum.A1:
                {
                    HideFlag = true;
                    break;
                }
            case GameModeEnum.A2:
                {
                    JumpFlag = true;
                    break;
                }
        }
    }

    public void set_preset_para(Dictionary<string,string> para_dict)
    {
        try { GazeTime = float.Parse(para_dict["GazeTime"]); }
        catch { GazeTime = 2.0f; }
        try { HideTime = float.Parse(para_dict["HideTime"]); }
        catch { HideTime = 0.2f; }
        try { ErrorTime = float.Parse(para_dict["ErrorTime"]); }
        catch { ErrorTime = 2.0f; }
        try { SpeedThreshold = float.Parse(para_dict["SpeedThreshold"]); }
        catch { SpeedThreshold = 10.0f; }
        try { StopWinodow = float.Parse(para_dict["StopWinodow"]); }
        catch { StopWinodow = 0.1f; }
        try { Gain = float.Parse(para_dict["Gain"]); }
        catch { Gain = 1.0f; }
        try { GazeTime = float.Parse(para_dict["GazeTime"]); }
        catch { GazeTime = 2.0f; }
    }

    public string VarToString()
    {
        string result_str = "";

        result_str += "HideFlag" + " " + HideFlag.ToString() + " ";
        result_str += "JumpFlag" + " " + JumpFlag.ToString() + " ";
        result_str += "ShowTargetFlag" + " " + ShowTargetFlag.ToString() + " ";
        result_str += "HeadIndicatorChange" + " " + HeadIndicatorChange.ToString() + " ";
        result_str += "SkipCenterFlag" + " " + SkipCenterFlag.ToString() + " ";
        result_str += "GazeTime" + " " + GazeTime.ToString() + " ";
        result_str += "RandomGazeTime" + " " + RandomGazeTime.ToString() + " ";
        result_str += "HideTime" + " " + HideTime.ToString() + " ";
        result_str += "SpeedThreshold" + " " + SpeedThreshold.ToString() + " ";
        result_str += "StopWinodow" + " " + StopWinodow.ToString() + " ";
        result_str += "Gain" + " " + Gain.ToString() + " ";

        return result_str;
    }





}
