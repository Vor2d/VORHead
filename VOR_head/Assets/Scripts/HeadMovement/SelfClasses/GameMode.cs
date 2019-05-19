using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMode
{
    public enum GameModeEnum { Default, GazeTest, EyeTest, Feedback_Learning,HC_FB_Learning,
                                Jump_Learning,Training,NoReddot}

    //Game Flag
    public bool HideFlag { get; set; }
    public bool JumpFlag { get; set; }
    public bool ShowTargetFlag { get; set; }
    public bool HeadIndicatorChange { get; set; }
    public bool SkipCenterFlag { get; set; }
    public bool HideHeadIndicator { get; set; }
    public bool ChangeTargetByTime { get; set; }
    //Variables;
    public float Gain { get; set; }
    public bool UsingAcuityAfter { get; set; }
    public bool UsingAcuityBefore { get; set; }
    public int AcuitySize { get; set; }

    public GameModeEnum GameModeName { get; set; }
    //public string game_mode_str{ get; set; }

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
        this.ChangeTargetByTime = other_GM.ChangeTargetByTime;

        this.Gain = other_GM.Gain;
        this.UsingAcuityAfter = other_GM.UsingAcuityAfter;
        this.UsingAcuityBefore = other_GM.UsingAcuityBefore;
        this.AcuitySize = other_GM.AcuitySize;

        this.GameModeName = other_GM.GameModeName;
    }

    public void set_preset_mode(GameModeEnum GM_preset)
    {
        GameModeName = GM_preset;
        switch (GM_preset)
        {
            case GameModeEnum.Default:
                {
                    HideFlag = false;
                    JumpFlag = false;
                    ShowTargetFlag = false;
                    HeadIndicatorChange = false;
                    SkipCenterFlag = false;
                    HideHeadIndicator = false;
                    ChangeTargetByTime = false;
                    break;
                }
            case GameModeEnum.GazeTest:
                {
                    HideFlag = true;
                    HideHeadIndicator = true;
                    SkipCenterFlag = true;
                    break;
                }
            case GameModeEnum.EyeTest:
                {
                    ChangeTargetByTime = true;
                    HideHeadIndicator = true;
                    SkipCenterFlag = true;
                    HideFlag = true;
                    ShowTargetFlag = false;
                    break;
                }
            case GameModeEnum.Feedback_Learning:
                {
                    SkipCenterFlag = true;
                    break;
                }
            case GameModeEnum.Jump_Learning:
                {
                    SkipCenterFlag = true;
                    HideFlag = true;
                    ShowTargetFlag = true;
                    JumpFlag = true;
                    HeadIndicatorChange = true;
                    break;
                }
            case GameModeEnum.HC_FB_Learning:
                {
                    SkipCenterFlag = true;
                    HeadIndicatorChange = true;
                    break;
                }
            case GameModeEnum.Training:
                {
                    SkipCenterFlag = true;
                    break;
                }
            case GameModeEnum.NoReddot:
                {
                    SkipCenterFlag = true;
                    HideHeadIndicator = true;
                    break;
                }
        }
    }

    public void set_preset_para(Dictionary<string,string> para_dict)
    {
        try { Gain = float.Parse(para_dict["Gain"]); }
        catch { Gain = 1.0f; }
        try { UsingAcuityAfter = para_dict["UsingAcuityAfter"] == "True"; }
        catch { UsingAcuityAfter = false; }
        try { UsingAcuityBefore = para_dict["UsingAcuityBefore"] == "True"; }
        catch { UsingAcuityBefore = false; }
        try { AcuitySize = int.Parse(para_dict["AcuitySize"]); }
        catch { AcuitySize = 4; }
    }

    public string VarToString()
    {
        

        string result_str = "";

        result_str += "GameMode" + " " + GameModeName.ToString() + " ";

        result_str += "HideFlag" + " " + HideFlag.ToString() + " ";
        result_str += "JumpFlag" + " " + JumpFlag.ToString() + " ";
        result_str += "ShowTargetFlag" + " " + ShowTargetFlag.ToString() + " ";
        result_str += "HeadIndicatorChange" + " " + HeadIndicatorChange.ToString() + " ";
        result_str += "SkipCenterFlag" + " " + SkipCenterFlag.ToString() + " ";
        result_str += "HideHeadIndicator" + " " + HideHeadIndicator.ToString() + " ";

        result_str += "Gain" + " " + Gain.ToString() + " ";
        result_str += "UsingAcuityAfter" + " " + UsingAcuityAfter.ToString() + " ";
        result_str += "UsingAcuityBefore" + " " + UsingAcuityBefore.ToString() + " ";
        result_str += "AcuitySize" + " " + AcuitySize.ToString() + " ";

        return result_str;
    }





}
