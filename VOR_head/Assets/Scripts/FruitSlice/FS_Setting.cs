using System;

[Serializable]
public class FS_Setting
{
    public float SliceSpeed;    //Cutting speed, need to above this to finish cut;
    public int ScoreIncrPerCut; //Score increase amount;
    //public float FruitFrameHeight;
    //public float FruitFrameWidth;
    public float FruitFrameSize;
    public float CutHalfMoveDist;
    public float CutHalfMoveSpeed;
    public FS_Setting()
    {
        this.SliceSpeed = 50.0f;
        this.ScoreIncrPerCut = 10;
        //this.FruitFrameHeight = 6.0f;
        //this.FruitFrameWidth = 6.0f;
        this.FruitFrameSize = 6.0f;
        this.CutHalfMoveDist = 1.0f;
        this.CutHalfMoveSpeed = 1.0f;
    }
}
