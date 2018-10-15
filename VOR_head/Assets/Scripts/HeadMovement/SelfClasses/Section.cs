using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Section
{

    public GameMode SectionGameMode { get; set; }
    public TrialInfo SectionTrialInfo { get; set; }

    public Section()
    {
        this.SectionGameMode = new GameMode();
        this.SectionTrialInfo = new TrialInfo();
    }

    public Section(GameMode gameMode,TrialInfo trialInfo)
    {
        this.SectionGameMode = new GameMode(gameMode);
        this.SectionTrialInfo = new TrialInfo(trialInfo);
    }

}
