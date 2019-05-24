using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System;
using System.IO;
using System.Text;

public class AutoLogSystem : MonoBehaviour {

    const string path = "Log/AutoLog/";

    [SerializeField] private VRLogSystem VRLog_script;
    [SerializeField] private AcuityLogSystem ALS_script;

    public bool VRAutoLog_On = false;
    public bool JumpAutoLog_On = true;
    public bool AcuityAutoLog_ON = false;
    public float JAL_Time = 30.0f;
    public float JAL_Changefile_Time = 90.0f;
    public float VRAL_Time = 300.0f;
    public float AAL_Time = 300.0f;

    private string folder_path;
    public bool VRthread_state_flag { get; set; }
    public bool Jumpthread_state_flag { get; set; }
    public bool Acuitythread_state_flag { get; set; }
    public StringBuilder VRAutoLog_string { get; set; }
    public StringBuilder JumpAutoLog_string { get; set; }
    public StringBuilder AcuityAutoLog_string { get; set; }
    private string jump_file_name;
    private string VR_file_name;
    private string Acuity_file_name;
    private DataController DC_script;
    private float jump_timer;
    private float jump_changefile_timer;
    private float VR_timer;
    private float Acuity_timer;
    private int acuity_str_index;

    private Thread VRAutoLog_Thread;
    private Thread JumpAutoLog_Thread;
    private Thread AcuityAutoLog_Thread;

    // Use this for initialization
    void Start () {
        this.DC_script = GameObject.Find("DataController").GetComponent<DataController>();

        this.folder_path = path + String.Format("{0:_yyyy_MM_dd}", DateTime.Today) + "/";
        this.VRthread_state_flag = false;
        this.Jumpthread_state_flag = false;
        this.Acuitythread_state_flag = false;
        this.JumpAutoLog_string = new StringBuilder();
        this.AcuityAutoLog_string = new StringBuilder();
        this.jump_file_name = folder_path + "JumpAutoLog_" +
                            String.Format("{0:_yyyy_MM_dd_hh_mm_ss}", DateTime.Now) + ".txt";
        this.VR_file_name = folder_path + "VRAutoLog_" +
                            String.Format("{0:_yyyy_MM_dd_hh_mm_ss}", DateTime.Now) + ".txt";
        this.Acuity_file_name = folder_path + "AcuityAutoLog_" +
                    String.Format("{0:_yyyy_MM_dd_hh_mm_ss}", DateTime.Now) + ".txt";
        this.jump_timer = JAL_Time;
        this.jump_changefile_timer = JAL_Changefile_Time;
        this.VR_timer = VRAL_Time;
        this.Acuity_timer = AAL_Time;
        this.acuity_str_index = 0;

        this.JumpAutoLog_Thread = new Thread(write_jumpauto_file);
        this.AcuityAutoLog_Thread = new Thread(write_acuityauto_file);

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

        //if(VRAutoLog_On)
        //{
        //    VRLog_script.turn_on_Thread();
        //}
    }
	
	// Update is called once per frame
	void Update () {
        jump_auto_log();
        VR_auto_log();
        Acuity_auto_log();
	}

    private void jump_auto_log()
    {
        //Jump Log;
        jump_timer -= Time.deltaTime;
        jump_changefile_timer -= Time.deltaTime;

        if (JumpAutoLog_On && jump_timer < 0 && !Jumpthread_state_flag)
        {
            jump_timer = JAL_Time;
            JumpAutoLog_Thread = new Thread(write_jumpauto_file);
            JumpAutoLog_Thread.Start();
        }

        if (jump_changefile_timer <= 0.0f)
        {
            jump_file_name = folder_path + "JumpAutoLog_" +
                    String.Format("{0:_yyyy_MM_dd_hh_mm_ss}", DateTime.Now) + ".txt";
            jump_changefile_timer = JAL_Changefile_Time;
        }
    }

    private void VR_auto_log()
    {
        VR_timer -= Time.unscaledDeltaTime;
        if (VRAutoLog_On && VR_timer < 0 && !VRthread_state_flag)
        {
            VR_timer = VRAL_Time;
            VRAutoLog_Thread = new Thread(start_VR_log);
            VRAutoLog_Thread.Start();
        }
    }

    private void start_VR_log()
    {
        VRthread_state_flag = true;
        VRLog_script.write_file();
        VRthread_state_flag = false;
    }

    private void Acuity_auto_log()
    {
        Acuity_timer -= Time.unscaledDeltaTime;
        if(AcuityAutoLog_ON && Acuity_timer < 0 && !Acuitythread_state_flag)
        {
            Acuity_timer = AAL_Time;
            AcuityAutoLog_Thread = new Thread(write_acuityauto_file);
            AcuityAutoLog_Thread.Start();
        }
    }

    private void write_acuityauto_file()
    {
        Acuitythread_state_flag = true;
        StreamWriter file;
        try
        {
            if (!File.Exists(Acuity_file_name))
            {
                file = new StreamWriter(Acuity_file_name);
            }
            else
            {
                file = File.AppendText(Acuity_file_name);
            }
            file.WriteLine(ALS_script.Log_SB.ToString(acuity_str_index,
                                            ALS_script.Log_SB.Length - acuity_str_index));
            acuity_str_index = ALS_script.Log_SB.Length;

            file.Close();
        }
        catch(Exception e)
        {
            Debug.Log("Error in accessing file: " + e);
        }
        Acuitythread_state_flag = false;
    }

    private void write_jumpauto_file()
    {
        Jumpthread_state_flag = true;
        StreamWriter file;
        try
        {
            // create log file if it does not already exist. Otherwise open it for appending new trial
            if (!File.Exists(jump_file_name))
            {
                file = new StreamWriter(jump_file_name);
                file.WriteLine("Settings: "+DC_script.SystemSetting.VarToString());
            }
            else
            {
                file = File.AppendText(jump_file_name);
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

    private void OnApplicationQuit()
    {
        quit_Thread();
    }

    private void quit_Thread()
    {
        write_jumpauto_file();
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
            Debug.Log("unable to close the connection thread upon application exit. This is not a critical exception.");
        }

        try
        {
            // attempt to join for 500ms
            if (!VRAutoLog_Thread.Join(2000))
            {
                // force shutdown
                VRAutoLog_Thread.Abort();
            }
        }
        catch (Exception e)
        {
            Debug.Log(e);
            Debug.Log("unable to close the connection thread upon application exit. This is not a critical exception.");
        }

        try
        {
            // attempt to join for 500ms
            if (!AcuityAutoLog_Thread.Join(2000))
            {
                // force shutdown
                AcuityAutoLog_Thread.Abort();
            }
        }
        catch (Exception e)
        {
            Debug.Log(e);
            Debug.Log("unable to close the connection thread upon application exit. This is not a critical exception.");
        }
    }

}
