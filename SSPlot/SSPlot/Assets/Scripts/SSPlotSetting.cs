﻿using System;
using System.Collections.Generic;
using EC;
using UnityEngine;

[Serializable]
public class SSPlotSetting
{
    public List<ChartModes> ChartModesList;
    public Vector2 ChartSize;
    public int HorizontalNum;
    public float HorizontalDegree;
    public float VerticalDegree;
    public float HeadSpeedThreshold;
    public float HeadRotationMax;
    public float HeadSpeedMax;
    public float EyeRotationMax;
    public float InitZ;
    public float PlotSpeed;
    public List<Color> ColorList;

    public SSPlotSetting()
    {
        this.ChartModesList = new List<ChartModes>();
        this.ChartSize = new Vector2();
        this.HorizontalNum = 0;
        this.HorizontalDegree = 0.0f;
        this.VerticalDegree = 0.0f;
        this.HeadSpeedThreshold = 0.0f;
        this.HeadRotationMax = 0.0f;
        this.HeadSpeedMax = 0.0f;
        this.EyeRotationMax = 0.0f;
        this.InitZ = 0.0f;
        this.PlotSpeed = 0.0f;
        this.ColorList = new List<Color>();

        IS = this;
    }

    public static SSPlotSetting IS;


}
