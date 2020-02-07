using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Threading;
using HMTS_enum;

namespace HMTS_enum
{
    public enum EyeFitMode { P2P, continuously, self_detect }
}

public class EC_GameController : MonoBehaviour {

    public GameObject TargetOBJ;
    public Camera GameCamera;
    public Camera UICamera;
    public Camera GameCamera2;
    [SerializeField] private CoilData CD_script;
    [SerializeField] private HeadSimulator HS_script;
    [SerializeField] private Transform IndicatorText1_TRANS;
    [SerializeField] private NN1Tread NN1Left_Thread;
    [SerializeField] private NN1Tread NN1Right_Thread;
    [SerializeField] private Text LErrorText;
    [SerializeField] private Text RErrorText;

    public float StairingTime = 5.0f;
    public bool EnableAnim = true;
    public float LowerBound = 0.3f;
    [SerializeField] float TrainingBound = 0.02f;
    [SerializeField] float NNInitWaitTime = 5.0f;

    public bool Stairing_flag { get; set; }
    public Vector2 Curr_target { get; private set; }
    public DataController DC_script { get; private set; }
    private float stair_timer;
    private List<Vector2> EC_trials;
    private Animator ECGCAnimator;
    private int trial_iter;
    private ChangePosition TCP_script;
    private Vector3 original_scale;
    private Transform target_crossTrans;
    private bool start_flag;
    private List<KeyValuePair<Vector2, Vector2>> Left_eye_data;   //target degrees and eye degrees;
    private List<KeyValuePair<Vector2, Vector2>> Right_eye_data;
    private bool calibration_finished_flag;
    private bool record_Cdata_flag;
    private AForge.Neuro.ActivationNetwork NN1_Left;
    private AForge.Neuro.ActivationNetwork NN1_Right;
    private float NN_init_wait_timer;
    private bool NN_init_wati_flag;

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
        this.record_Cdata_flag = false;
        this.NN_init_wait_timer = NNInitWaitTime;
        this.NN_init_wati_flag = false;
    }

    // Update is called once per frame
    void Update() {
        if (Stairing_flag && start_flag)
        {
            stair_timer -= Time.deltaTime;
            record_data_P2P();
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            start_flag = true;
        }
        if(record_Cdata_flag)
        {
            record_data_continuously();
        }
        if(NN_init_wati_flag)
        {
            NN_init_wait_timer -= Time.deltaTime;
        }

        LErrorText.text = NN1Left_Thread.Error_rate.ToString("F10");
        RErrorText.text = NN1Right_Thread.Error_rate.ToString("F10");
    }



    public void ToInit()
    {
        set_anim_bool();
        IndicatorText1_TRANS.GetComponent<TextMesh>().text = "Paused";
        IndicatorText1_TRANS.GetComponent<MeshRenderer>().enabled = true;
    }

    private void set_anim_bool()
    {
        switch(DC_script.FitMode)
        {
            case EyeFitMode.P2P:
                ECGCAnimator.SetBool("UsingP2P", true);
                ECGCAnimator.SetBool("UsingContinuously", false);
                ECGCAnimator.SetBool("UsingSelfDetect", false);
                break;
            case EyeFitMode.continuously:
                ECGCAnimator.SetBool("UsingP2P", false);
                ECGCAnimator.SetBool("UsingContinuously", true);
                ECGCAnimator.SetBool("UsingSelfDetect", false);
                break;
            case EyeFitMode.self_detect:
                ECGCAnimator.SetBool("UsingP2P", false);
                ECGCAnimator.SetBool("UsingContinuously", false);
                ECGCAnimator.SetBool("UsingSelfDetect", true);
                break;
        }
    }

    public void Init()
    {
        if(start_flag)
        {
            IndicatorText1_TRANS.GetComponent<MeshRenderer>().enabled = false;
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

    private void record_data_P2P()
    {
        Left_eye_data.Add(new KeyValuePair<Vector2, Vector2>(
                EC_trials[trial_iter], CD_script.Left_eye_voltage));
        Right_eye_data.Add(new KeyValuePair<Vector2, Vector2>(
                EC_trials[trial_iter], CD_script.Right_eye_voltage));
    }

    public void ToFinish()
    {
        IndicatorText1_TRANS.GetComponent<MeshRenderer>().enabled = true;
        IndicatorText1_TRANS.GetComponent<TextMesh>().text = "Calibrating";
        CalibrationThread.Start();
    }

    private void calibrate()
    {
        DC_script.Eye_info.calibration(Left_eye_data,Right_eye_data,
                                        DC_script.FitMode,DC_script.FitFunction,
                                        NN1_Left,NN1_Right);
        calibration_finished_flag = true;
    }

    public void Finish()
    {
        if(calibration_finished_flag)
        {
            calibration_finished_flag = false;
            IndicatorText1_TRANS.GetComponent<TextMesh>().text = "Calibration Finished";
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

    public void ToContiuously()
    {
        switch(DC_script.FitMode)
        {
            case EyeFitMode.continuously:
                record_Cdata_flag = true;
                break;
            case EyeFitMode.self_detect:
                break;
        }
    }

    private void start_train()
    {
        NN1Left_Thread.start_training();
        NN1Right_Thread.start_training();
    }

    private void record_data_continuously()
    {
        Left_eye_data.Add(new KeyValuePair<Vector2, Vector2>(
                                new Vector2(-HS_script.TrueHeadRR.y, -HS_script.TrueHeadRR.x), 
                                CD_script.Left_eye_voltage));
        Right_eye_data.Add(new KeyValuePair<Vector2, Vector2>(
                                new Vector2(-HS_script.TrueHeadRR.y, -HS_script.TrueHeadRR.x),
                                CD_script.Right_eye_voltage));
    }

    public void Continuously()
    {
        switch(DC_script.FitMode)
        {
            case EyeFitMode.continuously:
                if (Input.GetKeyDown(KeyCode.D))
                {
                    record_Cdata_flag = false;
                    ECGCAnimator.SetTrigger("Finished");
                }
                break;
            case EyeFitMode.self_detect:
                if (NN1Left_Thread.Error_rate <= TrainingBound && 
                    NN1Right_Thread.Error_rate <= TrainingBound)
                {
                    stop_train();
                    ECGCAnimator.SetTrigger("Finished");
                }
                break;
        }
    }

    private void stop_train()
    {
        NN1Left_Thread.stop_trainning();
        NN1Right_Thread.stop_trainning();
        NN1_Left = NN1Left_Thread.get_network();
        NN1_Right = NN1Right_Thread.get_network();
    }

    public void ToNNInitWait()
    {
        NN_init_wati_flag = true;
        start_train();
    }

    public void NNInitWait()
    {
        if(NN_init_wait_timer < 0.0f)
        {
            NN_init_wait_timer = NNInitWaitTime;
            NN_init_wati_flag = false;
            ECGCAnimator.SetTrigger("NextStep");
        }
    }

    public NN1Tread get_NN1thread()
    {
        return NN1Left_Thread;
    }
}


