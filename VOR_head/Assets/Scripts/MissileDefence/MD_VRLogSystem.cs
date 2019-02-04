using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.IO;

public class MD_VRLogSystem : VRLogSystem
{
    [SerializeField] private MD_GameController MDGC_script;
    [SerializeField] private Controller_Input CI_script;

    protected override void Start()
    {
        CI_script.IndexTrigger += log_controller;

        base.Start();
    }

    protected override void load_data_controller()
    {
        
    }

    public override void update_headline()
    {
        head_line = MDGC_script.var_to_string();
    }

    private void log_controller()
    {
        Log_SB.Append(line_counter.ToString());
        Log_SB.Append("\t");
        Log_SB.Append("Index trigger pushed\t");
        total_time = stop_watch.Elapsed.TotalMilliseconds;
        Log_SB.Append((total_time - last_time).ToString("F0"));
        Log_SB.Append("\t");
        Log_SB.AppendLine(total_time.ToString("F0"));
        last_velocity = curr_velocity;
    }
}
