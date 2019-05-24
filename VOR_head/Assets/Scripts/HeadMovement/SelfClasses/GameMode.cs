using System.Collections.Generic;
using System;
using UnityEngine;

using HMTS_enum;

public class GameMode
{
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
    public bool UsingAcuityWaitTime { get; set; }
    public int AcuitySize { get; set; }
    public bool UsingAcuityChange { get; set; }
    public AcuityChangeMode CurrAcuityChangeMode { get; set; }

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
        this.UsingAcuityWaitTime = other_GM.UsingAcuityWaitTime;
        this.AcuitySize = other_GM.AcuitySize;
        this.UsingAcuityChange = other_GM.UsingAcuityChange;
        this.CurrAcuityChangeMode = other_GM.CurrAcuityChangeMode;

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
        try { UsingAcuityWaitTime = para_dict["UsingAcuityWaitTime"] == "True"; }
        catch { UsingAcuityWaitTime = false; }
        try { AcuitySize = int.Parse(para_dict["AcuitySize"]); }
        catch { AcuitySize = 4; }
        try { UsingAcuityChange = (para_dict["UsingAcuityChange"] == "True"); }
        catch { UsingAcuityChange = false; }
        try
        {
            string[] temp_enums = Enum.GetNames(typeof(AcuityChangeMode));
            for (int i = 0;i< temp_enums.Length;i++)
            {
                if (para_dict["AcuityChangeMode"] == temp_enums[i])
                {
                    CurrAcuityChangeMode = (AcuityChangeMode)i;
                    break;
                }
            }

        }
        catch { CurrAcuityChangeMode = AcuityChangeMode.acuity_list; }
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
        result_str += "UsingAcuityWaitTime" + " " + UsingAcuityWaitTime.ToString() + " ";
        result_str += "AcuitySize" + " " + AcuitySize.ToString() + " ";
        result_str += "UsingAcuityChange" + " " + UsingAcuityChange.ToString() + " ";
        result_str += "CurrAcuityChangeMode" + " " + CurrAcuityChangeMode.ToString() + " ";

        return result_str;
    }





}
