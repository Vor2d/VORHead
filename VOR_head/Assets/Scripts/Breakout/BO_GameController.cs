﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BO_GameController : MonoBehaviour {

    public const string ball_Oname_str = "BO_Ball";
    public const string nextS_trigger_str = "NextStep";
    public const string restart_trigger_str = "ReStart";
    public const string start_trigger_str = "Start";
    public const string brick_tag_str = "BO_Brick";
    public const string sceneM_Oname_str = "SceneManager";
    public const string ballM_text_str = "Ball Missed!";

    [SerializeField] private GameObject TextIndicator1;
    [SerializeField] private GameObject TextIndicator2;
    [SerializeField] private Transform BOPad_TRANS;
    [SerializeField] private GameObject BOBall_Prefab;
    [SerializeField] public GameObject DebugText1;
    [SerializeField] private Transform PosManuIndicator;
    [SerializeField] private Transform PosPlayIndicator;
    [SerializeField] private Transform CameraParent_TRANS;
    [SerializeField] private Transform Body_TRANS;
    [SerializeField] private RightController RC_script;
    [SerializeField] private Transform FakePad;

    [SerializeField] private float StartCountTime = 3.0f;
    [SerializeField] private float BallOffset = 1.0f;
    [SerializeField] private float LostWaitTime = 1.0f;

    private float start_count_timer;
    private bool start_count_flag;
    private Animator BOGCAnimator;
    private Transform BOBall_TRANS;
    private float lost_wait_timer;
    private bool lost_wait_flag;
    private int trial_iter;
    private bool brick_updated;
    private bool check_start;

    // Use this for initialization
    void Start () {
        this.start_count_timer = StartCountTime;
        this.BOGCAnimator = GetComponent<Animator>();
        this.start_count_flag = false;
        this.lost_wait_timer = LostWaitTime;
        this.lost_wait_flag = false;
        this.trial_iter = -1;
        this.brick_updated = true;
        this.check_start = false;

        TextIndicator2.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {

        //Debug.Log("pad position " + BOPad_TRANS.position);
        if(Input.GetKeyDown(KeyCode.JoystickButton1))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        try
        {
            TextIndicator1.GetComponent<TextMesh>().text =
                            BOBall_TRANS.position.ToString();
        }
        catch { }
        try
        {
            //DebugText1.GetComponent<TextMesh>().text =
            //            BOBall_TRANS.GetComponent<BO_Ball>().boundary_timer.ToString("F2");
        }
        catch { }


        if (start_count_flag)
        {
            start_count_timer -= Time.deltaTime;
        }

        if(lost_wait_flag)
        {
            lost_wait_timer -= Time.deltaTime;
        }

	}

    public void BO_ToInit()
    {
        //BOGCAnimator.SetTrigger("NextStep");
        check_start = true;
        init_var();
    }

    public void ToBO_Start()
    {
        lost_wait_flag = true;
        BOBall_TRANS = Instantiate(BOBall_Prefab, 
                                BOPad_TRANS.position + new Vector3(0.0f,0.0f,BallOffset),
                                new Quaternion()).transform;
        BOBall_TRANS.name = ball_Oname_str;
        trial_iter++;
        if(trial_iter > 0)
        {
            TextIndicator2.SetActive(true);
            TextIndicator2.GetComponent<TextMesh>().text = ballM_text_str;
        }
    }

    public void BO_Start()
    {
        if(lost_wait_timer < 0.0f)
        {
            lost_wait_timer = LostWaitTime;
            lost_wait_flag = false;
            TextIndicator2.SetActive(false);
            BOGCAnimator.SetTrigger(nextS_trigger_str);
        }
    }

    public void ToStartCountDown()
    {
        start_count_flag = true;
        TextIndicator2.SetActive(true);
    }

    public void StartCountDown()
    {
        TextIndicator2.GetComponent<TextMesh>().text = ((int)start_count_timer).ToString();
        BOBall_TRANS.position = BOPad_TRANS.position + new Vector3(0.0f, 0.0f, BallOffset);
        if(start_count_timer < 0.0f)
        {
            start_game();
            start_count_timer = StartCountTime;
            start_count_flag = false;
            TextIndicator2.SetActive(false);
            BOGCAnimator.SetTrigger(nextS_trigger_str);
        }
    }

    private void start_game()
    {
        BOBall_TRANS.GetComponent<BO_Ball>().start_ball();
    }

    public void restart_game()
    {
        BOGCAnimator.SetTrigger(restart_trigger_str);
    }

    public void brick_destroied()
    {
        update_brick();
    }

    public void update_brick()
    {
        foreach (GameObject brick_OBJ in GameObject.FindGameObjectsWithTag(brick_tag_str))
        {
            brick_OBJ.GetComponent<BO_Brick>().update_shadow();
        }
    }

    private void init_var()
    {
        //CameraParent_TRANS.transform.position = PosManuIndicator.position;
        Body_TRANS.position = PosManuIndicator.position;
        RC_script.turn_on_controller();
        //BOPad_TRANS.GetComponent<BO_Pad>().move_with_raycast = false;
        BOPad_TRANS.gameObject.SetActive(false);
        FakePad.gameObject.SetActive(true);
        Debug.Log("init_var");
    }

    public void start_button()
    {
        if (check_start)
        {
            to_game();
            BOGCAnimator.SetTrigger(start_trigger_str);
        }
    }

    public void back_button()
    {
        Debug.Log("back_button");
        GameObject.Find(sceneM_Oname_str).GetComponent<MySceneManager>().to_start_scene();
    }

    private void to_game()
    {
        //CameraParent_TRANS.transform.position = PosPlayIndicator.position;
        Body_TRANS.position = PosPlayIndicator.position;
        RC_script.turn_off_controller();
        //BOPad_TRANS.GetComponent<BO_Pad>().move_with_raycast = true;
        BOPad_TRANS.gameObject.SetActive(true);
        FakePad.gameObject.SetActive(false);
        Debug.Log("to_game");
    }



}
