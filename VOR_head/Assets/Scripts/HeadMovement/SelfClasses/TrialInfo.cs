using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrialInfo
{
    
    public int Loop_number { get; set; }
    public List<float> Turn_data { get; set; }
    public List<float> Jump_data { get; set; }

    public TrialInfo()
    {
        this.Turn_data = new List<float>();
        this.Jump_data = new List<float>();
    }

    public TrialInfo(TrialInfo other_TI)
    {
        this.Turn_data = new List<float>(other_TI.Turn_data);
        this.Jump_data = new List<float>(other_TI.Jump_data);
    }

    public TrialInfo(List<float> tu_data, List<float> ju_data)
    {
        this.Turn_data = new List<float>(tu_data);
        this.Jump_data = new List<float>(ju_data);
    }



}
