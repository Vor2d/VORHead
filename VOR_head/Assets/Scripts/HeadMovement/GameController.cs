using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.XR;
using System.IO;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {

    //Direction: 0 is left, 1 is right;

    public enum GazeTarget { DefaultTarget, HideDetector}

    //Obsolete;
    public Dictionary<string, string> GameModeToIndiText = new Dictionary<string, string>()
    {
        { "Training", "Back Training" },
        { "EyeTest","Test 1"},
        { "GazeTest","Test 2"},
        { "HC_FB_Learning","Learning 1"},
        { "Jump_Learning","Learning 2"},
    };

    //Obsoleted;
    [HideInInspector]
    public bool ShowResultFlag = false;
    [HideInInspector]
    public bool TurnSpeedWindow = false;
    [HideInInspector]
    public GameObject ResultTarget = null;

    [Header("Game Object")]
    //Game Object;
    public GameObject Target;
    public GameObject IndiText1;
    public GameObject HeadIndicator;
    public GameObject CameraParent;
    public GameObject LastPosIndocator;
    public GameObject HeadSimulator;
    public TextMesh TestText;
    public TextMesh TestText2;
    public GameObject CoilData;
    public GameObject HeadSParent;
    public GameObject LogSystem;
    public GameController_Setting GCS_script;

    //Hiden;
    public uint simulink_sample { get; set; }
    public int trial_iter { get; set; }
    public float turn_degree { get; set; }
    public int turn_direct { get; set; }
    public Vector2 last_rot_ang_dir { get; set; }
    public string Current_state { get; set; }
    public bool Hide_time_flag { get; set; }
    public bool Check_stop_flag { get; set; }
    public bool Error_time_flag { get; set; }
    public bool Check_speed_flag { get; set; }
    public bool Target_raycast_flag { get; set; }
    public bool Hide_raycast_flag { get; set; }
    public float Hide_timer { get; set; }
    public int section_number { get; set; }
    public int loop_iter { get; set; }
    public bool TargetTimerFlag { get; set; }


    //Scripts;
    private RayCast ray_cast_scrip;
    private Target tar_script;
    private ResultTarget restar_script;
    private CenterIndicator center_script;
    private DataController DC_script;
    private HeadSimulator HS_script;
    private ChangePosition tar_CP_script;
    private ChangePosition LPI_CP_script;
    private CoilData CD_script;
    private VRLogSystem VRLS_script;
    private JumpLogSystem JLS_script;
    //Objects;
    private Animator GCAnimator;
    //Variables;
    private float result_rotate;
    private float gaze_timer;
    private float head_speed_y;
    private float error_timer;
    private List<float> turn_data;
    private List<float> jump_data;
    private float stop_window_timer;
    private Vector2 current_rot_ang_dir;
    private float hide_gaze_timer;
    private float gaze_timer_rand;
    private float target_change_timer;
    //Flags;
    private bool head_speed_flag;
    private bool stopped_flag;
    private bool centered_flag;
    private bool collaberating_flag;
    

    // Use this for initialization
    void Start() {
        this.DC_script = GameObject.Find("DataController").GetComponent<DataController>();

        //this.ray_cast_scrip = Camera.main.GetComponent<RayCast>();
        this.ray_cast_scrip = HeadSimulator.GetComponent<RayCast>();
        this.gaze_timer = DC_script.SystemSetting.GazeTime;
        this.gaze_timer_rand = DC_script.SystemSetting.GazeTime;
        this.hide_gaze_timer = DC_script.SystemSetting.GazeTime;
        this.tar_script = Target.GetComponent<Target>();
        this.restar_script = ResultTarget.GetComponent<ResultTarget>();
        //this.SNJstatus = SNJSteps.ToReset;
        //this.center_rotatey = 0.0f;
        this.last_rot_ang_dir = new Vector2(0.0f,0.0f);
        this.current_rot_ang_dir = new Vector2(0.0f, 0.0f);
        this.GCAnimator = GetComponent<Animator>();
        this.head_speed_flag = false;
        this.Check_speed_flag = false;
        this.Hide_timer = DC_script.SystemSetting.HideTime;
        this.Hide_time_flag = false;
        this.Check_stop_flag = false;
        this.stopped_flag = false;
        this.head_speed_y = 0.0f;
        this.error_timer = DC_script.SystemSetting.ErrorTime;
        //this.indi_text1 = GameObject.Find("IndicatorText1");
        //this.head_indicator = GameObject.Find("HeadIndicator");
        this.turn_data = new List<float>(DC_script.Current_TI.Turn_data);
        this.trial_iter = -1;
        this.jump_data = new List<float>(DC_script.Current_TI.Jump_data);
        //this.HSC_script = GetComponent<HeadStateController>();
        //this.LPI_script = LastPosIndocator.GetComponent<LastPosIndicator>();
        this.stop_window_timer = DC_script.SystemSetting.StopWinodow;
        this.Target_raycast_flag = true;
        this.HS_script = HeadSimulator.GetComponent<HeadSimulator>();
        //this.HI_CP_script = HeadIndicator.GetComponent<ChangePosition>();
        this.tar_CP_script = Target.GetComponent<ChangePosition>();
        this.CD_script = CoilData.GetComponent<CoilData>();
        this.VRLS_script = LogSystem.GetComponent<VRLogSystem>();
        this.JLS_script = LogSystem.GetComponent<JumpLogSystem>();
        this.simulink_sample = 0;
        this.turn_degree = 0.1f;
        this.turn_direct = 0;
        this.collaberating_flag = false;
        this.Current_state = "";
        this.loop_iter = -1;
        this.Hide_raycast_flag = false;
        this.LPI_CP_script = LastPosIndocator.GetComponent<ChangePosition>();
        this.section_number = 0;
        this.TargetTimerFlag = false;
        this.target_change_timer = DC_script.SystemSetting.TargetChangeTime;

        IndiText1.GetComponent<TextMesh>().text = "";

        //Active another monitor;
        if (Display.displays.Length > 1)
            Display.displays[1].Activate();



    }

    //private void instant_DataController()
    //{
    //    if (GameObject.Find("DataController") == null)
    //    {
    //        GameObject DC_instant = Instantiate(DataController) as GameObject;
    //        DC_instant.name = "DataController";
    //    }
    //}

    // Update is called once per frame
    void Update() {

        if (Input.GetKeyDown(KeyCode.Z))
        {
            recenter_stage();
        }

        if(Input.GetKeyDown(KeyCode.L))
        {
            VRLS_script.toggle_Thread();
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            JLS_script.toggle_Log();
        }

        //if(Input.GetKeyDown(KeyCode.C))
        //{
        //    if(!collaberating_flag)
        //    {
        //        GCAnimator.SetTrigger("Collaborate");
        //    }
        //    else
        //    {
        //        restart_scene();
        //    }
        //}

        //TestText.text = (Screen.width).ToString();
        //TestText2.text = (Screen.dpi).ToString();

        if (Check_stop_flag)
        {
            if(DC_script.UsingCoilFlag)
            {
                head_speed_y = CD_script.currentHeadVelocity.z;
            }
            else
            {
                head_speed_y = GeneralMethods.getVRspeed().y;
            }
            
            if (Mathf.Abs(head_speed_y) < 5.0f)
            {
                //record_target();
                //stopped_flag = true;
                stop_window_timer -= Time.deltaTime;
            }
            else
            {
                stop_window_timer = DC_script.SystemSetting.StopWinodow;
            }
        }

        if (Error_time_flag)
        {
            error_timer -= Time.deltaTime;
        }


        if (ray_cast_scrip.hit_flag && Target_raycast_flag)
        {
            //Debug.Log("get_hit_flag");
            tar_script.turn_on_tmesh();
            gaze_timer -= Time.deltaTime;
        }
        else
        {
            tar_script.turn_off_tmesh();
            gaze_timer = gaze_timer_rand;
        }

        if(ray_cast_scrip.hit_hideDetector_flag && Hide_raycast_flag)
        {
            hide_gaze_timer -= Time.deltaTime;
        }
        else
        {
            hide_gaze_timer = gaze_timer_rand;
        }

        if (Hide_time_flag)
        {
            Hide_timer -= Time.deltaTime;
        }

        if(TargetTimerFlag)
        {
            target_change_timer -= Time.deltaTime;
        }
    }

    public void update_data_restart()
    {
        restart_scene();
    }

    public void ToReset()
    {
        //if (DC_script.HeadIndicatorChange)
        //{
        //    HeadIndicator.GetComponent<Renderer>().enabled = true;
        //}
        turn_degree = 0;
        tar_CP_script.changePosition(turn_degree, 0.0f, 0, 0);
        GCAnimator.SetTrigger("NextStep");
        //center_rotatey = turn_degree;
        last_rot_ang_dir = new Vector2(0.0f, 1);
        //LPI_script.changePosition(last_rot_ang_dir.x, (int)last_rot_ang_dir.y);
        LPI_CP_script.changePosition(last_rot_ang_dir.x, 0.0f, (int)last_rot_ang_dir.y , 0);
    }

    public void Gaze(GazeTarget gazeTarget)
    {
        //Debug.Log("gaze_timer" + gaze_timer);
        switch(gazeTarget)
        {
            case GazeTarget.DefaultTarget:
                if (gaze_timer <= 0)
                {
                    GCAnimator.SetTrigger("NextStep");
                    gaze_timer = gaze_timer_rand;
                }
                break;

            case GazeTarget.HideDetector:
                if(hide_gaze_timer <= 0)
                {
                    GCAnimator.SetTrigger("NextStep");
                    hide_gaze_timer = gaze_timer_rand;
                }
                break;
        }
    }

    public void ToCenterGaze()
    {
        gaze_timer_rand = DC_script.SystemSetting.GazeTime +
                UnityEngine.Random.Range(-DC_script.SystemSetting.RandomGazeTime,
                                            DC_script.SystemSetting.RandomGazeTime);
    }

    public void CenterGaze()
    {
        Gaze(GazeTarget.DefaultTarget);
    }

    public void ToTargetToGaze()
    {
        if ((DC_script.Current_GM.HideFlag && !DC_script.Current_GM.ShowTargetFlag)
                || DC_script.Current_GM.HideHeadIndicator)
        {
            Target_raycast_flag = false;
            Hide_raycast_flag = true;
        }

        gaze_timer_rand = DC_script.SystemSetting.GazeTime +
                UnityEngine.Random.Range(-DC_script.SystemSetting.RandomGazeTime,
                                            DC_script.SystemSetting.RandomGazeTime);

    }

    public void TargetToGaze()
    {
        if ((DC_script.Current_GM.HideFlag && !DC_script.Current_GM.ShowTargetFlag) 
                || DC_script.Current_GM.HideHeadIndicator)
        {
            Gaze(GazeTarget.HideDetector);
        }
        else
        {
            Gaze(GazeTarget.DefaultTarget);
        }
    }

    public void LeaveTargetGaze()
    {
        Target_raycast_flag = true;
        Hide_raycast_flag = false;
    }

    public void ToMoveTarget()
    {
        float turn_deg_temp = 0.1f;
        int turn_dir_temp = 0;
        try
        {
            get_degree_direct(turn_data, trial_iter, out turn_deg_temp, out turn_dir_temp);
        }
        catch(Exception e)
        {
            Debug.Log(e);
        }
        turn_degree = turn_deg_temp;
        turn_direct = turn_dir_temp;

        Debug.Log("turning " + turn_degree);

        if(DC_script.UsingCoilFlag)
        {
            float virtual_degree = 
                GeneralMethods.RealToVirtualy(DC_script.SystemSetting.Player_screen_cm,
                                                DC_script.SystemSetting.Screen_width_cm,
                                                turn_degree);
            tar_CP_script.changePosition(virtual_degree, 0.0f, turn_direct, 0);
            current_rot_ang_dir = new Vector2(virtual_degree, turn_direct);
        }
        if(DC_script.UsingVRFlag)
        {
            tar_CP_script.changePosition(turn_degree, 0.0f, turn_direct, 0);
            current_rot_ang_dir = new Vector2(turn_degree, turn_direct);
        }

        //Debug.Log("turning virtual " + virtual_degree);

        GCAnimator.SetTrigger("NextStep");
    }

    public void ToJumpBack()
    {
        float turn_deg_temp = 0.1f;
        int turn_dir_temp = 0;
        try
        {
            get_degree_direct(jump_data, trial_iter, out turn_deg_temp, out turn_dir_temp);
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
        turn_degree = turn_deg_temp;
        turn_direct = turn_dir_temp;

        Debug.Log("jumping " + turn_degree);
        float virtual_degree = 
            GeneralMethods.RealToVirtualy(DC_script.SystemSetting.Player_screen_cm, 
                                          DC_script.SystemSetting.Screen_width_cm,
                                          turn_degree);
        //Debug.Log("jumping virtual " + virtual_degree);
        tar_CP_script.changePosition(virtual_degree, 0.0f, turn_direct,0);
        //to_log_jumpdata();
        GCAnimator.SetTrigger("NextStep");
    }

    //public void SNJToGaze()
    //{
    //    if (gaze_timer <= 0)
    //    {
    //        GCAnimator.SetTrigger("NextStep");
    //        gaze_timer = DC_script.Current_GM.GazeTime;
    //    }
    //}

    public void Hide()
    {
        if (Hide_timer < 0.0f)
        {
            tar_script.turn_off_all_tmesh();
            GCAnimator.SetTrigger("NextStep");
            Hide_timer = DC_script.SystemSetting.HideTime;
        }
    }

    public void ToWaitForTurn()
    {
        //if (DC_script.HeadIndicatorChange)
        //{
        //    HeadIndicator.GetComponent<Renderer>().enabled = false;
        //}

        //HSC_script.reset_data();
    }

    public void WaitForTurn()
    {
        //Debug.Log("center " + center_rotatey);

        check_speed_no_window();

        //check_border();
    }

    private void check_speed_with_window()
    {
        //if (!HSC_script.check_from_center(center_rotatey))
        //{
        //    if (HSC_script.check_speed())
        //    {
        //        if (HSC_script.speedEvaluationRecord == 0)
        //        {
        //            GCAnimator.SetTrigger("NextStep");
        //        }
        //        else
        //        {
        //            GCAnimator.SetTrigger("Reset");
        //        }
        //    }
        //}
        //else
        //{
        //    GCAnimator.SetTrigger("Reset");
        //}
    }

    private void check_speed_no_window()
    {
        if(DC_script.UsingCoilFlag)
        {
            head_speed_y = CD_script.currentHeadVelocity.z;
        }
        if(DC_script.UsingVRFlag)
        {
            head_speed_y = GeneralMethods.getVRspeed().y;
        }
        if (Mathf.Abs(head_speed_y) > DC_script.SystemSetting.SpeedThreshold)
        {
            GCAnimator.SetTrigger("NextStep");
            return;
        }
        if (ray_cast_scrip.hit_border_flag)
        {
            Debug.Log("hit_border_flag");
            GCAnimator.SetTrigger("Reset");
        }

    }

    private void record_target()
    {
        //result_rotate = Camera.main.transform.localEulerAngles.y;
    }

    public void ToShowTarget()
    {
        tar_script.turn_on_tobjmesh();
        GCAnimator.SetTrigger("NextStep");
    }

    public void CheckStop()
    {
        if (stop_window_timer < 0.0f)
        {
            GCAnimator.SetTrigger("NextStep");
            Check_stop_flag = false;    //One more step to guarantee the state is closed;
            stop_window_timer = DC_script.SystemSetting.StopWinodow;
            //stopped_flag = false;
        }
    }

    public void ShowResult()
    {
        int direction = 0;
        if (result_rotate < 0.0f)
        {
            direction = 0;
        }
        else
        {
            direction = 1;
        }
        //restar_script.changePosition(result_rotate, direction);
        restar_script.turn_on_mesh();
        GCAnimator.SetTrigger("NextStep");
    }

    //private void active_SNJ_mode()
    //{
    //    HideFlag = false;
    //    JumpFlag = true;
    //    ShowTargetFlag = false;
    //    ShowResultFlag = true;
    //}

    //private void active_SNS_mode()
    //{
    //    HideFlag = true;
    //    JumpFlag = false;
    //    ShowTargetFlag = true;
    //    ShowResultFlag = true;
    //}

    //private void init_game_mode(GameMode game_mode)
    //{
    //    switch (game_mode)
    //    {
    //        case GameMode.ShowNStay:
    //            active_SNS_mode();
    //            break;
    //        case GameMode.ShowNJump:
    //            active_SNJ_mode();
    //            break;
    //        case GameMode.Custom:
    //            break;
    //    }
    //}

    public void ToShowError()
    {
        if(TurnSpeedWindow)
        {
            //IndiText1.GetComponent<TextMesh>().text = HSC_script.speedEvaluationMessage;
            IndiText1.GetComponent<Renderer>().enabled = true;

            if (DC_script.Current_GM.HideFlag)
            {
                tar_script.turn_on_tobjmesh();
            }
        }
        else
        {
            IndiText1.GetComponent<TextMesh>().text = "Too Slow!";
            IndiText1.GetComponent<Renderer>().enabled = true;
            if (DC_script.Current_GM.HideFlag)
            {
                tar_script.turn_on_tobjmesh();
            }
        }

        //Debug.Log("")
    }

    public void ShowError()
    {
        if(error_timer < 0.0f)
        {
            GCAnimator.SetTrigger("NextStep");
            Error_time_flag = false;    //One more setp;
            error_timer = DC_script.SystemSetting.ErrorTime;
        }
    }

    public void LeaveShowError()
    {
        IndiText1.GetComponent<Renderer>().enabled = false;
    }

    //public void to_next_trial()
    //{

    //}

    public void ToStartTrial()
    {
        if(ShowResultFlag)
        {
            restar_script.turn_off_mesh();
        }
        if (!DC_script.Current_GM.HideHeadIndicator)
        {
            HeadIndicator.GetComponent<MeshRenderer>().enabled = true;
        }
        else
        {
            HeadIndicator.GetComponent<MeshRenderer>().enabled = false;
        }

        if (trial_iter < 0)
        {
            last_rot_ang_dir = new Vector2(0.0f, 0.0f);
            LPI_CP_script.changePosition(last_rot_ang_dir.x, 0.0f, (int)last_rot_ang_dir.y, 0);
            trial_iter++;
            GCAnimator.SetTrigger("FirstLoopNext");
        }
        else
        {
            last_rot_ang_dir = current_rot_ang_dir;
            LPI_CP_script.changePosition(last_rot_ang_dir.x, 0.0f, (int)last_rot_ang_dir.y, 0);
            trial_iter++;
            if (trial_iter >= turn_data.Count)
            {
                loop_iter++;
                if (DC_script.Current_TI.Loop_number == -1
                        || loop_iter < DC_script.Current_TI.Loop_number)
                {
                    trial_iter = 0;
                    GCAnimator.SetTrigger("NextStep");
                }
                else 
                {
                    loop_iter = -1;
                    trial_iter = -1;
                    section_number++;
                    if (section_number < DC_script.Sections.Count)
                    {
                        GCAnimator.SetTrigger("UpdateDC");
                    }
                    else
                    {
                        GCAnimator.SetTrigger("Finished");
                    }
                }
            }
            else
            {
                GCAnimator.SetTrigger("NextStep");
            }
            
        }
        Debug.Log("Trial " + trial_iter);
        Debug.Log("Loop " + loop_iter);
        Debug.Log("Section " + section_number);
    }

    public void ToUpdateDC()
    {
        DC_script.Current_GM = DC_script.Sections[section_number].SectionGameMode;
        DC_script.Current_TI = DC_script.Sections[section_number].SectionTrialInfo;
        turn_data = DC_script.Current_TI.Turn_data;
        jump_data = DC_script.Current_TI.Jump_data;
        update_Animator();

        GCAnimator.SetTrigger("NextStep");
    }

    public void ToInit()
    {
        update_Animator();
        trial_iter = -1;
        ToReset();
    }

    public void recenter_stage()
    {
        if(DC_script.UsingCoilFlag)
        {
            HS_script.reset_originQ();
        }
        if(DC_script.UsingVRFlag)
        {
            InputTracking.Recenter();
        }
        
    }

    public void load_setting_scene()
    {
        SceneManager.LoadScene("HeadMSetting");
    }

    public void ToReturnPosition()
    {
        tar_CP_script.changePosition(last_rot_ang_dir.x, 0.0f, (int)last_rot_ang_dir.y,0);

        GCAnimator.SetTrigger("NextStep");
    }

    public void update_SS()
    {
        simulink_sample = CD_script.simulinkSample;
    }

    public void ToCollaborate()
    {
        collaberating_flag = true;
        Target_raycast_flag = false;
    }

    public void Collaborating()
    {
        if(Input.GetKeyDown(KeyCode.S))
        {
            GCAnimator.SetTrigger("NextStep");
        }
    }

    public void LeaveCollaborate()
    {
        Target_raycast_flag = true;
    }

    public void ToFinish()
    {
        IndiText1.GetComponent<TextMesh>().text = "Finished!";
        IndiText1.GetComponent<Renderer>().enabled = true;
    }

    public void ToHideHeadIndicator()
    {
        if(DC_script.Current_GM.HeadIndicatorChange)
        {
            HeadIndicator.GetComponent<MeshRenderer>().enabled = false;
        }
        
        GCAnimator.SetTrigger("NextStep");
    }

    public void ToShowHeadIndicator()
    {
        if (DC_script.Current_GM.HeadIndicatorChange)
        {
            HeadIndicator.GetComponent<MeshRenderer>().enabled = true;
        }

        GCAnimator.SetTrigger("NextStep");
    }

    private void get_degree_direct(List<float> list, int iter, 
                                    out float turn_deg, out int turn_dir)
    {
        float temp_degree = list[iter];
        turn_deg = Mathf.Abs(temp_degree);
        if (temp_degree < 0)
        {
            turn_dir = 0;
        }
        else
        {
            turn_dir = 1;
        }
    }

    private void restart_scene()
    {
        SceneManager.LoadScene("HeadMovement");
    }

    private void update_Animator()
    {
        GCAnimator.SetBool("HideFlag", DC_script.Current_GM.HideFlag);
        GCAnimator.SetBool("JumpFlag", DC_script.Current_GM.JumpFlag);
        GCAnimator.SetBool("ShowTargetFlag", DC_script.Current_GM.ShowTargetFlag);
        GCAnimator.SetBool("SkipCenter", DC_script.Current_GM.SkipCenterFlag);
        GCAnimator.SetBool("ChangeTargetByTimeFlag", DC_script.Current_GM.ChangeTargetByTime);
    }

    public void ToChangeTargetWaitTime()
    {
        TargetTimerFlag = true;
    }

    public void ChangeTargetWaitTime()
    {
        if(target_change_timer < 0.0f)
        {
            target_change_timer = DC_script.SystemSetting.TargetChangeTime;
            TargetTimerFlag = false;

            GCAnimator.SetTrigger("NextStep");
        }
    }

    public void ToSectionPause()
    {
        //string showing_message =
        //            GameModeToIndiText[DC_script.Current_GM.GameModeName.ToString()];
        //IndiText1.GetComponent<TextMesh>().text = showing_message;
        //IndiText1.GetComponent<MeshRenderer>().enabled = true;

        GCS_script.change_indicate_text(DC_script.Current_GM.GameModeName.ToString());
    }

    public void SectionPause ()
    {
        if(Input.GetKeyDown(KeyCode.S))
        {
            GCAnimator.SetTrigger("NextStep");
        }
    }

    public void LeaveSectionPause()
    {
        //IndiText1.GetComponent<MeshRenderer>().enabled = false;

        GCS_script.turn_off_text();
    }
}
