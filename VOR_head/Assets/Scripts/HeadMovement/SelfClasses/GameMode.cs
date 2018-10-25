using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMode
{
    public enum GameModeEnum { Default, Test, Feedback_Learning,HC_FB_Learning, Jump_Learning }

    //Game Flag
    public bool HideFlag { get; set; }
    public bool JumpFlag { get; set; }
    public bool ShowTargetFlag { get; set; }
    public bool HeadIndicatorChange { get; set; }
    public bool SkipCenterFlag { get; set; }
    public bool HideHeadIndicator { get; set; }
    //Variables;
    public float Gain { get; set; }

    public GameModeEnum game_mode { get; set; }
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

        this.Gain = other_GM.Gain;

        this.game_mode = other_GM.game_mode;
    }

    public void set_preset_mode(GameModeEnum GM_preset)
    {
        game_mode = GM_preset;
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
                    break;
                }
            case GameModeEnum.Test:
                {
                    HideFlag = true;
                    HideHeadIndicator = true;
                    SkipCenterFlag = true;
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
                    break;
                }
            case GameModeEnum.HC_FB_Learning:
                {
                    SkipCenterFlag = true;
                    HeadIndicatorChange = true;
                    break;
                }
        }
    }

    public void set_preset_para(Dictionary<string,string> para_dict)
    {
        try { Gain = float.Parse(para_dict["Gain"]); }
        catch { Gain = 1.0f; }
    }

    public string VarToString()
    {
        

        string result_str = "";

        string game_mode_str = "";
        switch (game_mode)
        {
            case GameModeEnum.Default:
                {
                    game_mode_str = "Default";
                    break;
                }
            case GameModeEnum.Test:
                {
                    game_mode_str = "Test";
                    break;
                }
            case GameModeEnum.Feedback_Learning:
                {
                    game_mode_str = "Feedback_Learning";
                    break;
                }
            case GameModeEnum.Jump_Learning:
                {
                    game_mode_str = "Jump_Learning";
                    break;
                }
            case GameModeEnum.HC_FB_Learning:
                {
                    game_mode_str = "HC_FB_Learning";
                    break;
                }
        }

        result_str += "GameMode" + " " + game_mode_str + " ";

        result_str += "HideFlag" + " " + HideFlag.ToString() + " ";
        result_str += "JumpFlag" + " " + JumpFlag.ToString() + " ";
        result_str += "ShowTargetFlag" + " " + ShowTargetFlag.ToString() + " ";
        result_str += "HeadIndicatorChange" + " " + HeadIndicatorChange.ToString() + " ";
        result_str += "SkipCenterFlag" + " " + SkipCenterFlag.ToString() + " ";
        result_str += "HideHeadIndicator" + " " + HideHeadIndicator.ToString() + " ";

        result_str += "Gain" + " " + Gain.ToString() + " ";

        return result_str;
    }





}
