using System;
using UnityEngine;

[Serializable]
public class FS_Setting
{
    public float SliceSpeed;    //Stop speed, need to above this to finish cut;
    public int ScoreIncrPerCut; //Score increase amount;
    public float ScoreCalMaxLenDiff;  //Max length difference to get bonus;
    public float ScoreCalMaxDist;
    public float MaxLenScore;   //The max score possibile;
    public float MaxDistScore;
    //public float FruitFrameHeight;
    //public float FruitFrameWidth;
    public float FruitFrameSize;
    public float CutHalfMoveDist;
    public float CutHalfMoveSpeed;
    public bool ApplyForce;
    public float ThrowForce;
    public float ThrowTorque;
    public float ForceRange;
    public float TorqueRange;
    public float PathLineWidth;
    public float StraPLineWidth;
    public float IndicatorSize;
    public float SpeedThreshold;    //Reach the speed threshold or fail;
    public float ResultFrameWidth;
    public float ResultFrameHeight;
    public float ResultTransTime;
    public float ResultTextOffsety;
    public float ResultFrameGap;
    public int ResultFrameHoriMax;    //Max number of horizontal frames;
    public float ResultFontSize;    //Text Charator size;
    public float PreviewMeshSize;
    public int PreviewSideNumber;   //How many frame on one side, including center;
    public Vector2 PreviewRotateSize;   //Total size of the preview panel, {center to right, center to up};
    public float PreviewTransTime;
    public int MaxStar;
    public float StarCalOffset; //Percentage to shift the calculation down; 1.0f represents the first star starts at 0;
    public int WeightMaxScore;

    public static FS_Setting IS;
    public FS_Setting()
    {
        IS = this;

        this.SliceSpeed = 50.0f;
        this.ScoreIncrPerCut = 10;
        this.ScoreCalMaxLenDiff = 1.0f;
        this.ScoreCalMaxDist = 1.0f;
        this.MaxLenScore = 1.0f;
        this.MaxDistScore = 1.0f;
        //this.FruitFrameHeight = 6.0f;
        //this.FruitFrameWidth = 6.0f;
        this.FruitFrameSize = 6.0f;
        this.CutHalfMoveDist = 1.0f;
        this.CutHalfMoveSpeed = 1.0f;
        this.ApplyForce = true;
        this.ThrowForce = 0.0f;
        this.ThrowTorque = 0.0f;
        this.ForceRange = 0.0f;
        this.TorqueRange = 0.0f;
        this.PathLineWidth = 0.1f;
        this.StraPLineWidth = 0.1f;
        this.IndicatorSize = 1.0f;
        this.SpeedThreshold = 100.0f;
        this.ResultFrameWidth = 2.0f;
        this.ResultFrameHeight = 2.0f;
        this.ResultTransTime = 3.0f;
        this.ResultTextOffsety = 0.5f;
        this.ResultFrameGap = 2.0f;
        this.ResultFrameHoriMax = 6;
        this.ResultFontSize = 0.05f;
        this.PreviewMeshSize = 2.0f;
        this.PreviewSideNumber = 4;
        this.PreviewRotateSize = new Vector2(4.0f, 0.0f);
        this.PreviewTransTime = 0.2f;
        this.MaxStar = 3;
        this.StarCalOffset = 0.5f;
        this.WeightMaxScore = 20;
    }
}
