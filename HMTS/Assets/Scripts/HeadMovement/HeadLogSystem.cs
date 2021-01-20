using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;
using System.IO;
using System.Text;

public class HeadLogSystem : GeneralLogSystem
{
    [SerializeField] private bool log_head;

    private const string _first_line = "Line_#\tSimulink_Sample\tHead_speedx\tHead_speedy\tHead_speedz\tTimer";
    private bool logging_flag;
    private System.Diagnostics.Stopwatch sW;

    public static HeadLogSystem IS;

    private void Awake()
    {
        IS = this;

        this.first_line = _first_line;
        this.logging_flag = false;
        this.sW = new System.Diagnostics.Stopwatch();
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        if (log_head) { Start_log(); }   
    }

    // Update is called once per frame
    void Update()
    {
        Logdown_head();
    }

    protected override void Logging()
    {
        while (thread_state_flag) { Thread.Sleep(100); }

        write_file();
    }

    public void Start_log()
    {
        sW.Start();
        logging_flag = true;
    }

    private void Logdown_head()
    {
        if(logging_flag)
        {
            Vector3 head_speed = CoilData.IS.currentHeadVelocity;
            UInt32 SS = CoilData.IS.simulinkSample;
            Logdown_head_str(head_speed, SS);
        }
    }

    private void Logdown_head_str(Vector3 head_speed, UInt32 SS)
    {
        line_counter++;
        Log_SB.Append(line_counter.ToString());
        Log_SB.Append("\t");
        Log_SB.Append(SS.ToString());
        Log_SB.Append("\t");
        Log_SB.Append(head_speed.x.ToString("F2"));
        Log_SB.Append("\t");
        Log_SB.Append(head_speed.y.ToString("F2"));
        Log_SB.Append("\t");
        Log_SB.Append(head_speed.z.ToString("F2"));
        Log_SB.Append("\t");
        Log_SB.Append(sW.ElapsedMilliseconds.ToString());
        Log_SB.AppendLine();
    }

    public void Add_game_state(string str)
    {
        Log_SB.Append(str);
        Log_SB.AppendLine();
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
