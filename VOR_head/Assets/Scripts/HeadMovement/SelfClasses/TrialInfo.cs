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
        this.Loop_number = -1;
        this.Turn_data = new List<float>();
        this.Jump_data = new List<float>();
    }

    public TrialInfo(TrialInfo other_TI)
    {
        this.Loop_number = other_TI.Loop_number;
        this.Turn_data = new List<float>(other_TI.Turn_data);
        this.Jump_data = new List<float>(other_TI.Jump_data);
    }

    public TrialInfo(List<float> tu_data, List<float> ju_data)
    {
        this.Turn_data = new List<float>(tu_data);
        this.Jump_data = new List<float>(ju_data);
    }

    public string VarToString()
    {
        string result_str = "";

        result_str += ("Loop_number " + Loop_number.ToString() + " ");
        for(int i = 0; i<Turn_data.Count;i++)
        {
            result_str += ("Turn_data " + i + " " + Turn_data[i].ToString() + " ");
            result_str += ("Jump_data " + i + " " + Jump_data[i].ToString() + " ");
        }

        return result_str;

    }



}
