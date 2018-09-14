using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System;
using System.Text;
using System.IO;


public class JumpLogSystem : MonoBehaviour
{

    const string path = "Log/JumpLog/";
    const string first_line = "Line_#\tTime_Stamp\tSimulink_Sample\tTrail_#\tAction\t" +
                                "Degree\tDirection";

    public bool log_state_flag { get; set; }

    private Thread JumpLog_Thread;
    private StringBuilder JumpLog;
    private string file_name;
    private System.Diagnostics.Stopwatch stop_watch;
    private long time_stamp;
    private int line_counter;
    private CoilData CD_script;
    private uint simulink_sample;
    private DataController DC_script;
    private AutoLogSystem ALS_script;

    // Use this for initialization
    void Start()
    {
        this.DC_script = GameObject.Find("DataController").GetComponent<DataController>();
        this.ALS_script = GetComponent<AutoLogSystem>();

        this.JumpLog = new StringBuilder();
        this.log_state_flag = false;
        this.file_name = path + "JumpLog_" +
                            String.Format("{0:_yyyy_MM_dd_hh_mm_ss}", DateTime.Now) + ".txt";
        this.stop_watch = new System.Diagnostics.Stopwatch();
        this.line_counter = 0;
        this.JumpLog_Thread = new Thread(write_file);
        this.time_stamp = 0;

        //Create Log folder;
        if (!Directory.Exists(path))
        {
            try
            {
                Directory.CreateDirectory(path);
            }
            catch (Exception e)
            {
                Debug.Log("Folder creation failed, " + e);
            }
        }
    }

    void Update()
    {

    }

    public void toggle_Log()
    {
        if (log_state_flag)
        {
            stop_watch.Stop();
            stop_watch.Reset();
            log_state_flag = false;
            JumpLog_Thread = new Thread(write_file);
            JumpLog_Thread.Start();
            Debug.Log("JumpLOG Stoped!!");
        }
        else
        {
            Debug.Log("Start Logging JumpLOG!!");
            stop_watch.Start();
            log_state_flag = true;
        }
    }

    public void log_action(uint stimulink_sample, int trail_number, string action,float degree,
                                                                                int direction)
    {
        time_stamp = stop_watch.ElapsedMilliseconds;
        if (log_state_flag)
        {            
            JumpLog.Append(time_stamp.ToString());
            JumpLog.Append("\t");
            JumpLog.Append(stimulink_sample.ToString());
            JumpLog.Append("\t");
            JumpLog.Append(trail_number.ToString());
            JumpLog.Append("\t");
            JumpLog.Append(action);
            JumpLog.Append("\t");
            JumpLog.Append(degree.ToString("F2"));
            JumpLog.Append("\t");
            JumpLog.Append(direction.ToString());
            JumpLog.AppendLine();
        }
        if(log_state_flag && ALS_script.JumpAutoLog_On)
        {
            try
            {
                ALS_script.JumpAutoLog_string.Append(time_stamp.ToString());
                ALS_script.JumpAutoLog_string.Append("\t");
                ALS_script.JumpAutoLog_string.Append(stimulink_sample.ToString());
                ALS_script.JumpAutoLog_string.Append("\t");
                ALS_script.JumpAutoLog_string.Append(trail_number.ToString());
                ALS_script.JumpAutoLog_string.Append("\t");
                ALS_script.JumpAutoLog_string.Append(action);
                ALS_script.JumpAutoLog_string.Append("\t");
                ALS_script.JumpAutoLog_string.Append(degree.ToString("F2"));
                ALS_script.JumpAutoLog_string.Append("\t");
                ALS_script.JumpAutoLog_string.Append(direction.ToString());
                ALS_script.JumpAutoLog_string.AppendLine();
            }
            catch(Exception e)
            {
                Debug.Log(e);
            }

        }
    }

    //private void turn_on_Log()
    //{
        
    //    JumpLog_Thread = new Thread(log_jump);
    //    JumpLog_Thread.Start();
    //}

    //private void turn_off_Thread()
    //{
    //    quit_Thread();
        
    //}

    //private void log_jump()
    //{
        //stop_watch.Start();
        //last_time = stop_watch.Elapsed.Milliseconds;
        //while (thread_state_flag)
        //{
        //    if (last_velocity != curr_velocity)
        //    {
        //        JumpLog.Append(curr_velocity.ToString("F4"));
        //        JumpLog.Append("\t");
        //        JumpLog.Append(curr_orientation.ToString("F4"));
        //        JumpLog.Append("\t");
        //        total_time = stop_watch.Elapsed.TotalMilliseconds;
        //        JumpLog.Append((total_time - last_time).ToString("F0"));
        //        JumpLog.Append("\t");
        //        JumpLog.AppendLine(total_time.ToString("F0"));
        //        last_velocity = curr_velocity;
        //        last_time = total_time;
        //    }

        //}

        //write_file();

    //}

    private void OnDestroy()
    {
        quit_Thread();
    }

    private void OnApplicationQuit()
    {
        quit_Thread();
    }

    private void write_file()
    {
        StreamWriter file;
        try
        {
            // create log file if it does not already exist. Otherwise open it for appending new trial
            if (!File.Exists(file_name))
            {
                file = new StreamWriter(file_name);
                file.WriteLine(DC_script.VarToString());
                file.WriteLine(first_line);
            }
            else
            {
                file = File.AppendText(file_name);
            }

            file.WriteLine(JumpLog);
            file.Close();
            JumpLog = new StringBuilder();
        }
        catch (System.Exception e)
        {
            Debug.Log("Error in accessing file: " + e);
        }
    }

    private void quit_Thread()
    {
        try
        {
            // attempt to join for 500ms
            if (!JumpLog_Thread.Join(2000))
            {
                // force shutdown
                JumpLog_Thread.Abort();
            }
        }
        catch (Exception e)
        {
            Debug.Log(e);
            Debug.Log("[polhemus] PlStream was unable to close the connection thread upon application exit. This is not a critical exception.");
        }
    }
}
