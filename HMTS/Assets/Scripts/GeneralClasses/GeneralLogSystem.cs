﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System;
using System.Text;
using System.IO;

public class GeneralLogSystem : MonoBehaviour
{
    public bool thread_state_flag { get; set; }

    [SerializeField] private string path = "Log/__Log/";
    [SerializeField] private string LogTypeName = "";
    [SerializeField] private bool UsingSharedTime;
    [SerializeField] LogSystem LS_script;

    
    public StringBuilder Log_SB { get; protected set; }
    protected string file_name;
    protected string first_line;
    protected System.Diagnostics.Stopwatch stop_watch;
    protected double last_time;
    protected double total_time;
    protected int line_counter;
    protected string head_line;
    protected Thread LOG_Thread;

    protected virtual void Start()
    {
        this.Log_SB = new StringBuilder();
        this.thread_state_flag = false;
        this.file_name = path + LogTypeName +
                            String.Format("{0:_yyyy_MM_dd_hh_mm_ss}", DateTime.Now) + ".txt";
        if(UsingSharedTime)
        {
            this.stop_watch = LS_script.stop_watch;
        }
        else
        {
            this.stop_watch = new System.Diagnostics.Stopwatch();
        }
        this.last_time = 0;
        this.total_time = 0;
        this.line_counter = 0;
        this.head_line = "";
        this.first_line = "";

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

        update_headline();
    }

    protected virtual void OnDestroy()
    {
        quit_LOG_Thread();
    }

    protected virtual void OnApplicationQuit()
    {
        quit_LOG_Thread();
    }

    public void toggle_Thread()
    {
        if (thread_state_flag)
        {
            turn_off_Thread();
        }
        else
        {
            turn_on_Thread();
        }
    }

    public void turn_on_Thread()
    {
        if (!UsingSharedTime)
        {
            stop_watch.Start();
        }
        thread_state_flag = true;
        Debug.Log("Start Logging LOG!! "+LogTypeName);
        LOG_Thread = new Thread(Logging);
        LOG_Thread.Start();
    }

    protected virtual void Logging()
    {
        Debug.Log("Logging!!!!, need to be overrided");
    }

    public void turn_off_Thread()
    {
        quit_LOG_Thread();
        Debug.Log("LOG Stoped!! "+LogTypeName);
        thread_state_flag = false;
    }

    public virtual void update_headline()
    {

    }

    public virtual void write_file()
    {
        //set_filename(DateTime.Now);
    }

    private void quit_LOG_Thread()
    {
        try
        {
            thread_state_flag = false;
            // attempt to join for 2000ms
            if (!LOG_Thread.Join(3000))
            {
                // force shutdown
                LOG_Thread.Abort();
            }
        }
        catch (Exception e)
        {
            Debug.Log(e);
            Debug.Log("Thread not able to close, this is not a critical exception.");
        }
    }

    public void set_filename(string name)
    {
        file_name = name;
    }

    public void set_filename(DateTime time)
    {
        file_name = path + LogTypeName +
                    String.Format("{0:_yyyy_MM_dd_hh_mm_ss}", time) + ".txt";
    }
}
