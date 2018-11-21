using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BO_GameController : MonoBehaviour {

    [SerializeField] private GameObject TextIndicator1;
    [SerializeField] private GameObject TextIndicator2;
    [SerializeField] private Transform BOPad_TRANS;
    [SerializeField] private GameObject BOBall_Prefab;
    [SerializeField] private GameObject DebugText1;

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

    // Use this for initialization
    void Start () {
        this.start_count_timer = StartCountTime;
        this.BOGCAnimator = GetComponent<Animator>();
        this.start_count_flag = false;
        this.lost_wait_timer = LostWaitTime;
        this.lost_wait_flag = false;
        this.trial_iter = -1;

        TextIndicator2.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {

        //Debug.Log("pad position " + BOPad_TRANS.position);

        try
        {
            TextIndicator1.GetComponent<TextMesh>().text =
                            BOBall_TRANS.position.ToString();
        }
        catch { }
        try
        {
            DebugText1.GetComponent<TextMesh>().text =
                        BOBall_TRANS.GetComponent<BO_Ball>().boundary_timer.ToString("F2");
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

    public void BO_Init()
    {
        BOGCAnimator.SetTrigger("NextStep");
    }

    public void ToBO_Start()
    {
        lost_wait_flag = true;
        BOBall_TRANS = Instantiate(BOBall_Prefab, 
                                BOPad_TRANS.position + new Vector3(0.0f,0.0f,BallOffset),
                                new Quaternion()).transform;
        trial_iter++;
        if(trial_iter > 0)
        {
            TextIndicator2.SetActive(true);
            TextIndicator2.GetComponent<TextMesh>().text = "Ball Missed!";
        }
    }

    public void BO_Start()
    {
        if(lost_wait_timer < 0.0f)
        {
            lost_wait_timer = LostWaitTime;
            lost_wait_flag = false;
            TextIndicator2.SetActive(false);
            BOGCAnimator.SetTrigger("NextStep");
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
            BOGCAnimator.SetTrigger("NextStep");
        }
    }

    private void start_game()
    {
        BOBall_TRANS.GetComponent<BO_Ball>().start_ball();
    }

    public void restart_game()
    {
        BOGCAnimator.SetTrigger("ReStart");
    }

    public void LeaveGameProcess()
    {

    }
}
