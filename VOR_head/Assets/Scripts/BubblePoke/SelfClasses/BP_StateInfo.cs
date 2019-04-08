using System.Collections.Generic;
using UnityEngine;

public class BP_StateInfo
{
    public List<Transform> Chara_list { get; set; }

    public BP_StateInfo()
    {
        this.Chara_list = new List<Transform>();
    }

    public BP_StateInfo(BP_StateInfo other_SI)
    {
        this.Chara_list = new List<Transform>(other_SI.Chara_list);
    }
}