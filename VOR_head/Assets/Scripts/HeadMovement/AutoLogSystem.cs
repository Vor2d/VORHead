using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System;
using System.IO;
using System.Text;

public class AutoLogSystem : MonoBehaviour {

    const string path = "Log/AutoLog/";

    public bool VRAutoLog_On = false;
    public bool JumpAutoLog_On = true;
    public float JAL_time = 30.0f;
    public float JAL_changefile_time = 90.0f;

    private string folder_path;
    //public bool VRthread_state_flag { get; set; }
    public bool Jumpthread_state_flag { get; set; }
    public StringBuilder VRAutoLog_string { get; set; }
    public StringBuilder JumpAutoLog_string { get; set; }
    //public bool VRWriteLock { get; set; }
    //public bool JumpWriteLock { get; set; }
    private string file_name;
    private DataController DC_script;
    private float jump_timer;
    private float jump_changefile_timer;

    private Thread VRAutoLog_Thread;
    private Thread JumpAutoLog_Thread;

    // Use this for initialization
    void Start () {
        this.DC_script = GameObject.Find("DataController").GetComponent<DataController>();

        this.folder_path = path + String.Format("{0:_yyyy_MM_dd}", DateTime.Today) + "/";
        //this.VRthread_state_flag = false;
        this.Jumpthread_state_flag = false;
        this.VRAutoLog_string = new StringBuilder();
        this.JumpAutoLog_string = new StringBuilder();
        //this.VRWriteLock = false;
        //this.JumpWriteLock = false;
        this.file_name = folder_path + "JumpAutoLog_" +
                            String.Format("{0:_yyyy_MM_dd_hh_mm_ss}", DateTime.Now) + ".txt";
        this.jump_timer = JAL_time;
        this.jump_changefile_timer = JAL_changefile_time;

        this.JumpAutoLog_Thread = new Thread(write_jumpauto_file);

        if (!Directory.Exists(folder_path))
        {
            try
            {
                Directory.CreateDirectory(folder_path);
            }
            catch (Exception e)
            {
                Debug.Log("Folder creation failed, " + e);
            }
        }
    }
	
	// Update is called once per frame
	void Update () {
        jump_timer -= Time.deltaTime;
        jump_changefile_timer -= Time.deltaTime;

        if(JumpAutoLog_On && jump_timer <= 0.0f && !Jumpthread_state_flag)
        {
            //Debug.Log("start thread");
            jump_timer = JAL_time;
            JumpAutoLog_Thread = new Thread(write_jumpauto_file);
            JumpAutoLog_Thread.Start();
        }

        if(jump_changefile_timer <= 0.0f)
        {
            file_name = folder_path + "JumpAutoLog_" +
                    String.Format("{0:_yyyy_MM_dd_hh_mm_ss}", DateTime.Now) + ".txt";
            jump_changefile_timer = JAL_changefile_time;
        }
	}

    private void write_jumpauto_file()
    {
        Jumpthread_state_flag = true;
        StreamWriter file;
        try
        {
            // create log file if it does not already exist. Otherwise open it for appending new trial
            if (!File.Exists(file_name))
            {
                file = new StreamWriter(file_name);
                file.WriteLine("Settings: "+DC_script.SystemSetting.VarToString());
            }
            else
            {
                file = File.AppendText(file_name);
            }

            StringBuilder temp_string = new StringBuilder(JumpAutoLog_string.ToString());
            JumpAutoLog_string = new StringBuilder();

            file.WriteLine(temp_string);
            file.Close();
        }
        catch (System.Exception e)
        {
            Debug.Log("Error in accessing file: " + e);
        }
        Jumpthread_state_flag = false;
        //Debug.Log("thread done");
    }

    private void OnDestroy()
    {
        quit_Thread();
    }

    private void quit_Thread()
    {
        try
        {
            // attempt to join for 500ms
            if (!JumpAutoLog_Thread.Join(2000))
            {
                // force shutdown
                JumpAutoLog_Thread.Abort();
            }
        }
        catch (Exception e)
        {
            Debug.Log(e);
            Debug.Log("[polhemus] PlStream was unable to close the connection thread upon application exit. This is not a critical exception.");
        }
    }

}
