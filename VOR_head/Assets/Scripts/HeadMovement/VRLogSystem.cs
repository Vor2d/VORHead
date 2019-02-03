using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;

public class VRLogSystem : GeneralLogSystem
{
    private const string _first_line =
                    "Line_#\tVR_Velocity\tVR_Orientation\tDeltaTime\tTotalTime";

    private DataController DC_script;
    private AutoLogSystem ALS_script;
    private Vector3 last_velocity;
    private Vector3 curr_velocity;
    private Quaternion curr_orientation;


    // Use this for initialization
    protected override void Start () {
        load_data_controller();
        this.ALS_script = GetComponent<AutoLogSystem>();

        base.Start();

        this.last_velocity = new Vector3();
        this.curr_velocity = new Vector3();
        this.first_line = _first_line;
        this.curr_orientation = new Quaternion();
    }

    protected virtual void load_data_controller()
    {
        this.DC_script = GameObject.Find("DataController").GetComponent<DataController>();
    }

    protected override void logging()
    {
        base.logging();

        log_headset();
    }

    private void log_headset()
    {
        stop_watch.Start();
        last_time = stop_watch.Elapsed.Milliseconds;
        while (thread_state_flag)
        {
            curr_velocity = GeneralMethods.getVRspeed();
            curr_orientation = GeneralMethods.getVRrotation();
            if (last_velocity != curr_velocity)
            {
                Log_SB.Append(line_counter.ToString());
                Log_SB.Append("\t");
                Log_SB.Append(curr_velocity.ToString("F4"));
                Log_SB.Append("\t");
                Log_SB.Append(curr_orientation.ToString("F4"));
                Log_SB.Append("\t");
                total_time = stop_watch.Elapsed.TotalMilliseconds;
                Log_SB.Append((total_time-last_time).ToString("F0"));
                Log_SB.Append("\t");
                Log_SB.AppendLine(total_time.ToString("F0"));
                last_velocity = curr_velocity;
                last_time = total_time;
                line_counter++;
            }

        }

        write_file();
    }

    public override void update_headline()
    {
        head_line = DC_script.SystemSetting.VarToString();
    }

    public override void write_file()
    {
        base.write_file();

        Debug.Log("Starting write file: " + file_name);
        StreamWriter file;
        try
        {
            // create log file if it does not already exist. Otherwise open it for appending new trial
            if (!File.Exists(file_name))
            {
                file = new StreamWriter(file_name);
                file.WriteLine(head_line);
                file.WriteLine(first_line);
            }
            else
            {
                file = File.AppendText(file_name);
            }

            
            file.WriteLine(Log_SB);
            file.Close();
            Log_SB = new StringBuilder();
            Debug.Log("Writing finished: " + file_name);
        }
        catch (System.Exception e)
        {
            Debug.Log("Error in accessing file: " + e);
        }

    }


}
