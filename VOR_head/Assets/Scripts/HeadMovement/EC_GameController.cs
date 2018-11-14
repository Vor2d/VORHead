using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EC_GameController : MonoBehaviour {

    private const string ECTrial_path = "ECTrial.txt";

    public GameObject TargetOBJ;
    public GameObject IndicatorText1;
    public Camera GameCamera;
    public Camera UICamera;

    public float StairingTime = 5.0f;
    public bool EnableAnim = true;
    public float LowerBound = 0.3f;

    public bool Stairing_flag { get; set; }

    private float stair_timer;
    private List<float> EC_trials;
    private Animator ECGCAnimator;
    private int trial_iter;
    private ChangePosition TCP_script;
    private Vector3 original_scale;
    private Transform target_crossTrans;
    private bool start_flag;
    private DataController DC_script;

    // Use this for initialization
    void Start() {

        if(DC_script == null)
        {
            this.DC_script = GameObject.Find("DataController").GetComponent<DataController>();
        }

        this.stair_timer = StairingTime;
        this.Stairing_flag = false;
        this.EC_trials = new List<float>();
        this.ECGCAnimator = GetComponent<Animator>();
        this.trial_iter = -1;
        this.TCP_script = TargetOBJ.GetComponent<ChangePosition>();
        this.target_crossTrans = TargetOBJ.transform.Find("TCrossBar");
        this.original_scale = target_crossTrans.localScale;
        this.start_flag = false;
        this.GameCamera.targetDisplay = Int32.Parse(DC_script.SystemSetting.Camera1_display);
        this.UICamera.targetDisplay = Int32.Parse(DC_script.SystemSetting.Camera2_display);
    }

    // Update is called once per frame
    void Update() {
        if (Stairing_flag)
        {
            stair_timer -= Time.deltaTime;
        }
        if(Input.GetKeyDown(KeyCode.S))
        {
            start_flag = true;
        }
    }

    public void read_trials()
    {
        Debug.Log("Loading ECTrial_path " + ECTrial_path);
        try
        {
            StreamReader reader = new StreamReader(ECTrial_path);
            while (!reader.EndOfStream)
            {
                EC_trials.Add(float.Parse(reader.ReadLine()));
            }
        }
        catch (Exception e) { Debug.Log(e); }
        Debug.Log("Loading ECTrial_path finished");
    }

    public void ECGC_Init()
    {
        if(start_flag)
        {
            start_flag = false;
            ECGCAnimator.SetTrigger("NextStep");
        }
    }

    public void start_trial()
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
        float degree_data = EC_trials[trial_iter];
        TCP_script.changePosition(Mathf.Abs(degree_data), 0.0f,
                                    (degree_data < 0) ? 0 : 1, 0);
        ECGCAnimator.SetTrigger("NextStep");
    }

    public void ToECGC_WaitTime()
    {
        Stairing_flag = true;
    }

    public void wait_time()
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

    public void finised()
    {
        IndicatorText1.GetComponent<MeshRenderer>().enabled = true;
        IndicatorText1.GetComponent<TextMesh>().text = "Finished";
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
