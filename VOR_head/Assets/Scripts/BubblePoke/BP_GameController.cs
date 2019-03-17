using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BP_GameController : MonoBehaviour {

    private const string score_init_str = "Score: ";

    [SerializeField] private BP_RC BPRC;

    public float BubbleIntervalTime = 3.0f;
    public float RandRangeX = 15.0f;
    public float RandRangeY = 15.0f;
    public float RandRangeZ1 = 5.0f;
    public float RandRangeZ2 = 15.0f;
    public int ScoreIncrease = 10;
    public float CharaMovingSpeed = 1.0f;

    private float bubble_inte_timer;
    private bool bubble_Itimer_flag;
    private Animator BPGCAnimator;
    private int score;
    private bool score_changed;
    private int total_trials;
    private int curr_trial;
    private int curr_station_index;

	// Use this for initialization
	void Start () {
        this.bubble_inte_timer = BubbleIntervalTime;
        this.bubble_Itimer_flag = false;
        this.BPGCAnimator = GetComponent<Animator>();
        this.score = 0;
        this.score_changed = true;
        this.total_trials = 0;
        this.curr_trial = 0;
        this.curr_station_index = 0;
	}
	
	// Update is called once per frame
	void Update () {
		if(bubble_Itimer_flag)
        {
            bubble_inte_timer -= Time.deltaTime;
        }

        update_score();
        move_charator();
	}

    public void ToInit()
    {
        init_game_para();
        BPGCAnimator.SetTrigger(BP_StrDefiner.AniNextStepTrigger_str);
    }

    private void init_game_para()
    {
        if(BPRC.BPDC_script.GameMode == BP_GameMode.UsingFile)
        {
            total_trials = BPRC.BPDC_script.trial_info.degree_info.Count;
            curr_trial = -1;
        }
    }

    public void ToInstantiateBubble()
    {
        if (BPRC.BPDC_script.GameMode == BP_GameMode.UsingFile)
        {
            curr_trial++;
            if (curr_trial < total_trials)
            {
                instantiate_bubble();
                BPGCAnimator.SetTrigger(BP_StrDefiner.AniNextStepTrigger_str);
            }
            else
            {
                BPGCAnimator.SetTrigger(BP_StrDefiner.AniFinishedTrigger_str);
            }
        }

        
    }

    private void instantiate_bubble()
    {
        GameObject bubble_obj = null;
        if (BPRC.BPDC_script.GameMode == BP_GameMode.UsingFile)
        {
            Vector2 curr_degree = BPRC.BPDC_script.trial_info.degree_info[curr_trial];
            bubble_obj = Instantiate(BPRC.BubblePrefab,
                            GeneralMethods.PositionCal(10.0f, curr_degree.x, curr_degree.y), 
                            new Quaternion());
        }
        else if(BPRC.BPDC_script.GameMode == BP_GameMode.Random)
        {
            bubble_obj =
                    Instantiate(BPRC.BubblePrefab, rand_pos_generator(), new Quaternion());
        }

        bubble_obj.GetComponent<Bubble>().start_bubble();

        Debug.Log("bubble " + bubble_obj.transform.position);
    }

    private Vector3 rand_pos_generator()
    {
        return new Vector3(Random.Range(-RandRangeX, RandRangeX),
                            Random.Range(-RandRangeY, RandRangeY),
                            Random.Range(RandRangeZ1, RandRangeZ2));
    }

    public void ToWaitTime()
    {
        bubble_Itimer_flag = true;
    }

    public void WaitTime()
    {
        if(bubble_inte_timer < 0.0f)
        {
            bubble_inte_timer = BubbleIntervalTime;

            BPGCAnimator.SetTrigger(BP_StrDefiner.AniNextStepTrigger_str);
        }
    }

    public void bubble_destroyed()
    {
        score += ScoreIncrease;
        score_changed = true;
    }

    private void update_score()
    {
        if(score_changed)
        {
            BPRC.ScoreText_TRANS.GetComponent<TextMesh>().text = 
                                            score_init_str + score.ToString();
            score_changed = false;
        }
    }

    private void move_charator()
    {
        BPRC.Charator_TRANS.Translate(
            (BPRC.Stations_TRANS[curr_station_index].position - BPRC.Charator_TRANS.position).
                normalized * Time.deltaTime * CharaMovingSpeed, Space.World);
        if(Vector3.Distance(BPRC.Charator_TRANS.position, 
                            BPRC.Stations_TRANS[curr_station_index].position) < 0.3f)
        {
            curr_station_index++;
            if(curr_station_index >= BPRC.Stations_TRANS.Length)
            {
                stop_moving_charator();
            }
        }
    }

    private void stop_moving_charator()
    {

    }
}


