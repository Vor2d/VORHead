using System.Collections.Generic;
using UnityEngine;

public class BP_StateInfo
{
    public List<Transform> Chara_list { get; set; }

    //public List<byte[]> Chara_byte_list { get; set; }

    public BP_StateInfo()
    {
        this.Chara_list = new List<Transform>();
        //this.Chara_byte_list = new List<byte[]>();
    }

    public BP_StateInfo(BP_StateInfo other_SI)
    {
        this.Chara_list = new List<Transform>(other_SI.Chara_list);

        //this.Chara_byte_list = new List<byte[]>(other_SI.Chara_byte_list);
    }
}