using System;

[Serializable]
public class FS_Setting
{
    public float SliceSpeed;    //Cutting speed, need to above this to finish cut;
    public int ScoreIncrPerCut; //Score increase amount;
    public FS_Setting()
    {
        this.SliceSpeed = 50.0f;
        this.ScoreIncrPerCut = 10;
    }
}
