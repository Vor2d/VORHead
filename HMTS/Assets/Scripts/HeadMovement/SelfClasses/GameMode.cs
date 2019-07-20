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
    public bool UsingPostDelay { get; set; }
    public PostDelayModes PostDelayMode { get; set; }
    public float PostDelayInit { get; set; }
    public float PostDelayIMax { get; set; }
    public bool ADUsingStaticData { get; set; }
    public bool UsingPresetSD { get; set; }
    public int StaticDataSize { get; set; }
    public float StaticDataMLH { get; set; }
    public bool UsingDynamicDelay { get; set; }
    public DynamicDelayModes DynamicDelayMode { get; set; }
    public float DynaDelayInit { get; set; }
    public float DynaDelayMax { get; set; }
    public float DynaDelayInter { get; set; }

    public GameModeEnum GameModeName { get; set; }
    //public string game_mode_str{ get; set; }

    public GameMode()
    {
        set_preset_para(new Dictionary<string, string>());
        set_preset_mode(GameModeEnum.Default);
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
        this.UsingPostDelay = other_GM.UsingPostDelay;
        this.PostDelayMode = other_GM.PostDelayMode;
        this.PostDelayInit = other_GM.PostDelayInit;
        this.PostDelayIMax = other_GM.PostDelayIMax;
        this.ADUsingStaticData = other_GM.ADUsingStaticData;
        this.UsingPresetSD = other_GM.UsingPresetSD;
        this.StaticDataSize = other_GM.StaticDataSize;
        this.StaticDataMLH = other_GM.StaticDataMLH;
        this.UsingDynamicDelay = other_GM.UsingDynamicDelay;
        this.DynamicDelayMode = other_GM.DynamicDelayMode;
        this.DynaDelayInit = other_GM.DynaDelayInit;
        this.DynaDelayMax = other_GM.DynaDelayMax;
        this.DynaDelayInter = other_GM.DynaDelayInter;

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
            case GameModeEnum.StaticAcuity:
                {
                    ChangeTargetByTime = true;
                    HideHeadIndicator = true;
                    SkipCenterFlag = true;
                    HideFlag = true;
                    ShowTargetFlag = true;
                    UsingAcuityAfter = false;
                    UsingAcuityBefore = false;
                    UsingAcuityWaitTime = true;
                    break;
                }
            case GameModeEnum.DynamicAcuity:
                {
                    HideFlag = false;
                    ShowTargetFlag = true;
                    SkipCenterFlag = true;
                    HeadIndicatorChange = true;
                    UsingAcuityAfter = false;
                    UsingAcuityBefore = true;
                    UsingAcuityWaitTime = false;
                    break;
                }
            case GameModeEnum.PostDynamicAcuity:
                {
                    HideFlag = false;
                    ShowTargetFlag = true;
                    SkipCenterFlag = true;
                    HeadIndicatorChange = true;
                    UsingAcuityAfter = true;
                    UsingAcuityBefore = false;
                    UsingAcuityWaitTime = false;
                    break;
                }
        }
    }

    public void set_preset_para(Dictionary<string,string> para_dict)
    {
        try { Gain = float.Parse(para_dict["Gain"]); }
        catch { Gain = 1.0f; }
        try { UsingAcuityAfter = para_dict["UsingAcuityAfter"] == "True"; }
        catch { }
        try { UsingAcuityBefore = para_dict["UsingAcuityBefore"] == "True"; }
        catch { }
        try { UsingAcuityWaitTime = para_dict["UsingAcuityWaitTime"] == "True"; }
        catch { }
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
        try { UsingPostDelay = (para_dict["UsingPostDelay"] == "True"); }
        catch { UsingPostDelay = false; }
        try
        {
            string[] temp_enums = Enum.GetNames(typeof(PostDelayModes));
            for (int i = 0; i < temp_enums.Length; i++)
            {
                if (para_dict["PostDelayMode"] == temp_enums[i])
                {
                    PostDelayMode = (PostDelayModes)i;
                    break;
                }
            }
        }
        catch { PostDelayMode = PostDelayModes.random; }
        try { PostDelayInit = float.Parse(para_dict["PostDelayInit"]); }
        catch { PostDelayInit = 0.0f; }
        try { PostDelayIMax = float.Parse(para_dict["PostDelayIMax"]); }
        catch { PostDelayIMax = 0.0f; }
        try { ADUsingStaticData = (para_dict["ADUsingStaticData"] == "True"); }
        catch { ADUsingStaticData = false; }
        try { UsingPresetSD = (para_dict["UsingPresetSD"] == "True"); }
        catch { UsingPresetSD = false; }
        try { StaticDataSize = int.Parse(para_dict["StaticDataSize"]); }
        catch { StaticDataSize = 0; }
        try { StaticDataMLH = float.Parse(para_dict["StaticDataMLH"]); }
        catch { StaticDataMLH = 0.0f; }
        try { UsingDynamicDelay = (para_dict["UsingDynamicDelay"] == "True"); }
        catch { UsingDynamicDelay = false; }
        try
        {
            string[] temp_enums = Enum.GetNames(typeof(DynamicDelayModes));
            for (int i = 0; i < temp_enums.Length; i++)
            {
                if (para_dict["DynamicDelayModes"] == temp_enums[i])
                {
                    DynamicDelayMode = (DynamicDelayModes)i;
                    break;
                }
            }
        }
        catch { DynamicDelayMode = DynamicDelayModes.fix_amount; }
        try { DynaDelayInit = float.Parse(para_dict["DynaDelayInit"]); }
        catch { DynaDelayInit = 0.0f; }
        try { DynaDelayMax = float.Parse(para_dict["DynaDelayMax"]); }
        catch { DynaDelayMax = 0.0f; }
        try { DynaDelayInter = float.Parse(para_dict["DynaDelayInter"]); }
        catch { DynaDelayInter = 0.0f; }
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
        result_str += "UsingPostDelay" + " " + UsingPostDelay.ToString() + " ";
        result_str += "PostDelayMode" + " " + PostDelayMode.ToString() + " ";
        result_str += "PostDelayInit" + " " + PostDelayInit.ToString("F2") + " ";
        result_str += "PostDelayIMax" + " " + PostDelayIMax.ToString("F2") + " ";
        result_str += "ADUsingStaticData" + " " + ADUsingStaticData.ToString() + " ";
        result_str += "UsingPresetSD" + " " + UsingPresetSD.ToString() + " ";
        result_str += "StaticDataSize" + " " + StaticDataSize.ToString() + " ";
        result_str += "StaticDataMLH" + " " + StaticDataMLH.ToString("F3") + " ";
        result_str += "DynamicDelayMode" + " " + DynamicDelayMode.ToString() + " ";
        result_str += "DynaDelayInit" + " " + DynaDelayInit.ToString("F3") + " ";
        result_str += "DynaDelayMax" + " " + DynaDelayMax.ToString("F3") + " ";
        result_str += "DynaDelayInter" + " " + DynaDelayInter.ToString("F3") + " ";

        return result_str;
    }





}
