using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System;
using System.Text;
using System.IO;


public class VRLogSystem : MonoBehaviour {

    const string path = "Log/VRLOG/";
    const string first_line = "Line_#\tVR_Velocity\tVR_Orientation\tDeltaTime\tTotalTime";

    public bool thread_state_flag { get; set; }

    private Thread VRLOG_Thread;
    private StringBuilder VRLog;
    private string file_name;
    private Vector3 last_velocity;
    private Vector3 curr_velocity;
    private System.Diagnostics.Stopwatch stop_watch;
    private double last_time;
    private double total_time;
    private Quaternion curr_orientation;
    private int line_counter;
    //private Thread write_file_Thread;
    private string head_line;
    private DataController DC_script;

    // Use this for initialization
    void Start () {
        this.DC_script = GameObject.Find("DataController").GetComponent<DataController>();

        this.VRLog = new StringBuilder();
        this.thread_state_flag = false;
        this.file_name = path + "VRLog_" +
                            String.Format("{0:_yyyy_MM_dd_hh_mm_ss}", DateTime.Now) + ".txt";
        this.last_velocity = new Vector3();
        this.stop_watch = new System.Diagnostics.Stopwatch();
        this.last_time = 0;
        this.total_time = 0;
        this.line_counter = 0;
        this.head_line = "";
        
        //Create Log folder;
        if(!Directory.Exists(path))
        {
            try
            {
                Directory.CreateDirectory(path);
            }
            catch(Exception e)
            {
                Debug.Log("Folder creation failed, " + e);
            }
        }


    }

    public void toggle_Thread()
    {
        if(thread_state_flag)
        {
            turn_off_Thread();
            thread_state_flag = false;
        }
        else
        {
            turn_on_Thread();
            thread_state_flag = true;
        }
    }

    private void turn_on_Thread()
    {
        Debug.Log("Start Logging VRLOG!!");
        VRLOG_Thread = new Thread(log_headset);
        VRLOG_Thread.Start();
    }

    private void turn_off_Thread()
    {
        quit_VRLOG_Thread();
        Debug.Log("VRLOG Stoped!!");

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
                VRLog.Append(line_counter.ToString());
                VRLog.Append("\t");
                VRLog.Append(curr_velocity.ToString("F4"));
                VRLog.Append("\t");
                VRLog.Append(curr_orientation.ToString("F4"));
                VRLog.Append("\t");
                total_time = stop_watch.Elapsed.TotalMilliseconds;
                VRLog.Append((total_time-last_time).ToString("F0"));
                VRLog.Append("\t");
                VRLog.AppendLine(total_time.ToString("F0"));
                last_velocity = curr_velocity;
                last_time = total_time;
                line_counter++;
            }

        }

        write_file();
    }

    //private void OnApplicationQuit()
    //{
    //    quit_VRLOG_Thread();
    //    //quit_write_file_Thread();
    //}

    private void OnDestroy()
    {
        quit_VRLOG_Thread();
    }

    private void write_file()
    {
        Debug.Log("Starting write file: " + this.file_name);
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

            
            file.WriteLine(VRLog);
            file.Close();
            VRLog = new StringBuilder();
            Debug.Log("Writing finished: " + file_name);
        }
        catch (System.Exception e)
        {
            Debug.Log("Error in accessing file: " + e);
        }

    }

    private void quit_VRLOG_Thread()
    {
        try
        {
            thread_state_flag = false;
            // attempt to join for 500ms
            if (!VRLOG_Thread.Join(2000))
            {
                // force shutdown
                VRLOG_Thread.Abort();
            }
        }
        catch (Exception e)
        {
            Debug.Log(e);
            Debug.Log("Thread not able to close, this is not a critical exception.");
        }
    }

    //private void quit_write_file_Thread()
    //{
    //    try
    //    {
    //        // attempt to join for 500ms
    //        if (!write_file_Thread.Join(500))
    //        {
    //            // force shutdown
    //            write_file_Thread.Abort();
    //        }
    //    }
    //    catch (Exception e)
    //    {
    //        Debug.Log(e);
    //        Debug.Log("Thread not able to close, this is not a critical exception.");
    //    }
    //}
}
