using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Threading;

public class EC_GameController : MonoBehaviour {

    public GameObject TargetOBJ;
    public GameObject IndicatorText1;
    public Camera GameCamera;
    public Camera UICamera;
    [SerializeField] private CoilData CD_script;

    public float StairingTime = 5.0f;
    public bool EnableAnim = true;
    public float LowerBound = 0.3f;

    public bool Stairing_flag { get; set; }
    public Vector2 Curr_target { get; private set; }

    private float stair_timer;
    private List<Vector2> EC_trials;
    private Animator ECGCAnimator;
    private int trial_iter;
    private ChangePosition TCP_script;
    private Vector3 original_scale;
    private Transform target_crossTrans;
    private bool start_flag;
    private DataController DC_script;
    private List<KeyValuePair<Vector2, Vector2>> Left_eye_data;   //target degrees and eye degrees;
    private List<KeyValuePair<Vector2, Vector2>> Right_eye_data;
    private bool calibration_finished_flag;

    private Thread CalibrationThread;

    // Use this for initialization
    void Start() {

        if(DC_script == null)
        {
            this.DC_script = GameObject.Find("DataController").GetComponent<DataController>();
        }

        this.stair_timer = StairingTime;
        this.Stairing_flag = false;
        this.EC_trials = new List<Vector2>(DC_script.Eye_TI.Turn_data);
        this.ECGCAnimator = GetComponent<Animator>();
        this.trial_iter = -1;
        this.TCP_script = TargetOBJ.GetComponent<ChangePosition>();
        this.target_crossTrans = TargetOBJ.transform.Find("TCrossBar");
        this.original_scale = target_crossTrans.localScale;
        this.start_flag = false;
        this.GameCamera.targetDisplay = Int32.Parse(DC_script.SystemSetting.Camera1_display);
        this.UICamera.targetDisplay = Int32.Parse(DC_script.SystemSetting.Camera2_display);
        this.Left_eye_data = new List<KeyValuePair<Vector2, Vector2>>();
        this.Right_eye_data = new List<KeyValuePair<Vector2, Vector2>>();
        this.CalibrationThread = new Thread(calibrate);
        this.calibration_finished_flag = false;
    }

    // Update is called once per frame
    void Update() {

        //CD_script.Left_eye_voltage = Curr_target;
        //CD_script.Right_eye_voltage = Curr_target*2;

        if (Stairing_flag && start_flag)
        {
            stair_timer -= Time.deltaTime;
            record_data();
            //Debug.Log("stair_timer " + stair_timer);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            start_flag = true;
        }
    }

    public void ToInit()
    {

    }

    public void Init()
    {
        if(start_flag)
        {
            ECGCAnimator.SetTrigger("NextStep");
        }
    }

    public void StartTrial()
    {
        trial_iter++;
        if(trial_iter >= EC_trials.Count)
        {
            ECGCAnimator.SetTrigger("Finished");
        }
        else
        {
            ECGCAnimator.SetTrigger("NextStep");
        }
    }

    public void change_position()
    {
        float turn_degre_x = EC_trials[trial_iter].x;
        int turn_direc_x = turn_degre_x < 0 ? 0 : 1;
        float turn_degre_y = EC_trials[trial_iter].y;
        int turn_direc_y = turn_degre_y < 0 ? 0 : 1;
        Curr_target = new Vector2(turn_degre_x, turn_degre_y);
        turn_degre_x = Mathf.Abs(turn_degre_x);
        turn_degre_y = Mathf.Abs(turn_degre_y);
        if (DC_script.using_coil)
        {
            Vector3 virtual_degree = new Vector3();
            if (!DC_script.SystemSetting.Using_curved_screen)
            {
                virtual_degree = GeneralMethods.
                                RealToVirtual(DC_script.SystemSetting.Player_screen_cm,
                                                DC_script.SystemSetting.Screen_width_cm,
                                                turn_degre_y,turn_direc_x);
            }
            else
            {
                virtual_degree = GeneralMethods.
                        RealToVirtual_curved(DC_script.SystemSetting.Player_screen_cm,
                                                DC_script.SystemSetting.Screen_width_cm,
                                                turn_degre_y,turn_degre_x);
            }
            TCP_script.changePosition(virtual_degree.y, virtual_degree.x, 
                                        turn_direc_x, turn_direc_y);
            
        }
        ECGCAnimator.SetTrigger("NextStep");
        
    }

    public void ToWaitTime()
    {
        Stairing_flag = true;
    }

    public void WaitTime()
    {
        if(EnableAnim)
        {
            target_crossTrans.localScale = original_scale * scale_cal();
        }
        if(stair_timer <= 0.0f)
        {
            ECGCAnimator.SetTrigger("NextStep");
            stair_timer = StairingTime;
            Stairing_flag = false;
        }
    }

    private void record_data()
    {
        Left_eye_data.Add(new KeyValuePair<Vector2, Vector2>(
                EC_trials[trial_iter], CD_script.Left_eye_voltage));
        Right_eye_data.Add(new KeyValuePair<Vector2, Vector2>(
                EC_trials[trial_iter], CD_script.Right_eye_voltage));
    }

    public void ToFinish()
    {
        IndicatorText1.GetComponent<MeshRenderer>().enabled = true;
        IndicatorText1.GetComponent<TextMesh>().text = "Calibrating";
        CalibrationThread.Start();
    }

    private void calibrate()
    {
        DC_script.Eye_info.calibration(Left_eye_data,Right_eye_data);
        calibration_finished_flag = true;
    }

    public void Finish()
    {
        if(calibration_finished_flag)
        {
            calibration_finished_flag = false;
            IndicatorText1.GetComponent<TextMesh>().text = "Calibration Finished";
            Debug.Log(DC_script.Eye_info.var_to_string());
        }
    }

    private float scale_cal()
    {
        return (stair_timer / StairingTime * (1 - LowerBound)) + LowerBound;
    }

    public void ToMainSceneButton()
    {
        SceneManager.LoadScene("HeadMovement");
    }
}
