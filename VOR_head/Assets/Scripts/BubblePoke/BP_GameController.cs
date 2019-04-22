using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using BP_EC;


public class BP_GameController : MonoBehaviour {

    private readonly char[] name_spliter = { '#' };

    private const string score_init_str = "Score: ";

    [SerializeField] private BP_RC BPRC;
    [SerializeField] private BP_AcuityController BPAC_script;

    public float BubbleIntervalTime = 3.0f;
    public float RandRangeX = 15.0f;
    public float RandRangeY = 15.0f;
    public float RandRangeZ1 = 5.0f;
    public float RandRangeZ2 = 15.0f;
    public int ScoreIncrease = 10;
    public float CharaMovingSpeed = 1.0f;
    public bool DrawPathFlag = true;
    public float PredictTime = 5.0f;
    public float PredictInterval = 0.1f;
    public int UsingSaveMethod = 1;
    public bool RandomTrailFlag = true;
    public Vector2 RandomPredictRange = Vector2.zero;
    public bool UsingSynchronizedBubble = true;
    [Header("FuelSystem")]
    public float InitFuelLevel = 10.0f;
    public float FuelConsuSpeed = 0.5f;
    public bool UsingFuelSystem = true;
    public float RefillAmount = 5.0f;
    [Header("Acuity")]
    public bool UsingAcuity = true;
    public bool UsingRandomAcuity = true;
    public int AcuityLength = 1;
    public AcuityMode GameAcuityMode = AcuityMode.OneByOne;

    public bool Bubble_ready_flag { get; private set; }
    private float bubble_inte_timer;
    private bool bubble_Itimer_flag;
    private Animator BPGCAnimator;
    private int score;
    private bool score_changed;
    private int total_trials;
    private int curr_trial;
    private int curr_station_index;
    private int chara_counter;
    private BP_StateInfo state_info;
    private bool check_bubble_flag;
    

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
        this.chara_counter = 0;
        this.state_info = new BP_StateInfo();
        this.Bubble_ready_flag = false;
        this.check_bubble_flag = false;

        if(UsingSynchronizedBubble)
        {
            StartCoroutine(check_bubble_ready());
        }
	}
	
	// Update is called once per frame
	void Update () {
		if(bubble_Itimer_flag)
        {
            bubble_inte_timer -= Time.deltaTime;
        }

        update_score();
	}

    public void ToInit()
    {
        init_game_para();
        BPGCAnimator.SetTrigger(BP_StrDefiner.AniNextStepTrigger_str);
    }

    private void init_game_para()
    {
        if(BPRC.DC_script.GameMode == BP_GameMode.UsingFile)
        {
            total_trials = BPRC.DC_script.trial_info.degree_info.Count;
            curr_trial = -1;
        }
    }

    [Obsolete("Bubble sponed in the TrailContriller")]
    public void ToInstantiateBubble()
    {
        switch(BPRC.DC_script.GameMode)
        {
            case BP_GameMode.UsingFile:
                curr_trial++;
                if (curr_trial < total_trials)
                {
                    instantiate_bubble_Ufile();
                    BPGCAnimator.SetTrigger(BP_StrDefiner.AniNextStepTrigger_str);
                }
                else
                {
                    BPGCAnimator.SetTrigger(BP_StrDefiner.AniFinishedTrigger_str);
                }
                break;
            case BP_GameMode.AlongPath:
                foreach(Transform Chara_TRANS in BPRC.Charators_TRANSs)
                {
                    instantiate_bubble_Apath(Chara_TRANS,PredictTime,PredictInterval);
                }
                BPGCAnimator.SetTrigger(BP_StrDefiner.AniNextStepTrigger_str);
                break;
        }

    }

    private void instantiate_bubble_Ufile()
    {
        Vector2 curr_degree = BPRC.DC_script.trial_info.degree_info[curr_trial];
        GameObject bubble_obj = Instantiate(BPRC.Bubble_Prefab,
                        GeneralMethods.PositionCal(10.0f, curr_degree.x, curr_degree.y),
                        new Quaternion());
        bubble_obj.GetComponent<Bubble>().start_bubble();
    }

    private void instantiate_bubble_random()
    {
        GameObject bubble_obj = 
                Instantiate(BPRC.Bubble_Prefab, rand_pos_generator(), new Quaternion());
        bubble_obj.GetComponent<Bubble>().start_bubble();
    }

    [Obsolete("")]
    private void instantiate_bubble_Apath(Transform chara_TRANS, float predict_time, 
                                                                float predict_interval)
    {
        Vector3 position = chara_pos_predict(chara_TRANS, predict_time, predict_interval);
        GameObject bubble_obj =
                Instantiate(BPRC.Bubble_Prefab, position, new Quaternion());
        bubble_obj.GetComponent<Bubble>().start_bubble(BPRC);
        BPRC.Bubble_TRANSs.Add(bubble_obj.transform);
    }

    [Obsolete("")]
    private Vector3 chara_pos_predict(Transform chara_TRANS, float predict_time, 
                                                                float predict_interval)
    {
        return chara_TRANS.GetComponent<BP_Charactor>().
                                            simulate_move(predict_time, predict_interval);
    }

    private Vector3 rand_pos_generator()
    {
        return new Vector3(UnityEngine.Random.Range(-RandRangeX, RandRangeX),
                            UnityEngine.Random.Range(-RandRangeY, RandRangeY),
                            UnityEngine.Random.Range(RandRangeZ1, RandRangeZ2));
    }

    public void ToWaitTime()
    {
        GeneralMethods.start_timer(ref bubble_inte_timer, ref bubble_Itimer_flag, 
                                    BubbleIntervalTime);
    }

    public void ToWaitTime(float timer)
    {
        GeneralMethods.start_timer(ref bubble_inte_timer, ref bubble_Itimer_flag, timer);
    }

    public void WaitTime()
    {
        if(GeneralMethods.check_timer(bubble_inte_timer, ref bubble_Itimer_flag))
        {
            BPGCAnimator.SetTrigger(BP_StrDefiner.AniNextStepTrigger_str);
        }
    }

    public void bubble_destroyed()
    {
        score += ScoreIncrease;
        score_changed = true;
    }

    public void bubble_destroyed(Transform chara_TRANS)
    {
        score += ScoreIncrease;
        score_changed = true;
        if(UsingFuelSystem)
        {
            refill_fuel(chara_TRANS);
        }
    }

    private void refill_fuel(Transform chara_TRANS)
    {
        foreach(Transform TC_TRANS in BPRC.TrailControllers_TRANSs)
        {
            Transform temp_CTRANS = TC_TRANS.GetComponent<BP_TrailController>().chara_TRANS;
            if (temp_CTRANS != chara_TRANS)
            {
                temp_CTRANS.GetComponent<BP_Charactor>().refill_fuel(RefillAmount);
            }
        }
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

    [Obsolete("See TrailController")]
    public void ToInstantiateCharactor1()
    {
        instantiate_TC(BPRC.PathPoseIs_TRANSs[0].position, Color.blue);
        instantiate_TC(BPRC.PathPoseIs_TRANSs[1].position, Color.green);

        BPGCAnimator.SetTrigger(BP_StrDefiner.AniNextStepTrigger_str);
    }

    private Transform instantiate_TC(Vector3 position,Color color)
    {
        Transform trailC_TRANS = Instantiate(BPRC.TrailController_Prefab, Vector3.zero,
                                    Quaternion.identity).transform;
        trailC_TRANS.GetComponent<BP_TrailController>().init_TC(BPRC, position, color);
        trailC_TRANS.GetComponent<BP_TrailController>().start_trail();
        BPRC.TrailControllers_TRANSs.Add(trailC_TRANS);
        
        return trailC_TRANS;
    }

    [Obsolete("See TrailController")]
    public void ToInstantiateCharactor()
    {
        Transform path0_TRANS =
                instantiate_path(BPRC.Path_Prefebs[0], BPRC.PathPoseIs_TRANSs[0].position);
        Transform path1_TRANS =
                instantiate_path(BPRC.Path_Prefebs[1], BPRC.PathPoseIs_TRANSs[1].position);

        Transform chara0_TRANS = instantiate_chara(BPRC.Charator_Prefebs[0], 
                                                    BPRC.PathPoseIs_TRANSs[0].position,
                                                    path0_TRANS);
        Transform chara1_TRANS = instantiate_chara(BPRC.Charator_Prefebs[1],
                                                    BPRC.PathPoseIs_TRANSs[1].position,
                                                    path1_TRANS);

        BPRC.Paths_TRANSs.Add(path0_TRANS);
        BPRC.Paths_TRANSs.Add(path1_TRANS);

        BPRC.Charators_TRANSs.Add(chara0_TRANS);
        BPRC.Charators_TRANSs.Add(chara1_TRANS);


        BPGCAnimator.SetTrigger(BP_StrDefiner.AniNextStepTrigger_str);
    }

    [Obsolete("See TrailController")]
    private Transform instantiate_path(GameObject prefab, Vector3 postition)
    {
        GameObject path_OBJ =
                    Instantiate(prefab, postition, Quaternion.identity);
        path_OBJ.GetComponent<BP_Path>().init_path(BPRC);

        return path_OBJ.transform;
    }

    [Obsolete("See TrailController")]
    private Transform instantiate_chara(GameObject prefab,Vector3 position, 
                                                        Transform target_TRANS)
    {
        GameObject charactor_OBJ = Instantiate(prefab, position, Quaternion.identity);
        charactor_OBJ.GetComponent<BP_Charactor>().init_chara(BPRC, target_TRANS);
        charactor_OBJ.GetComponent<BP_Charactor>().start_chara();

        return charactor_OBJ.transform;
    }

    [Obsolete("See TrailController")]
    public void ToSaveState()
    {
        save_state1();
        BPGCAnimator.SetTrigger(BP_StrDefiner.AniNextStepTrigger_str);
    }

    //private void save_state2()
    //{
    //    state_info.Chara_byte_list.Clear();
    //    foreach (Transform chara_TRANS in BPRC.Charators_TRANSs)
    //    {

    //    }
    //}

    [Obsolete("See TrailController")]
    private void save_state1()
    {
        state_info.Chara_list.Clear();
        foreach (Transform chara_TRANS in BPRC.Charators_TRANSs)
        {
            save_chara(chara_TRANS);
        }
    }

    [Obsolete("See TrailController")]
    private void save_chara(Transform chara_TRANS)
    {
        Transform temp_TRANS = Instantiate(chara_TRANS);
        temp_TRANS.name = chara_TRANS.name + "#backup";
        temp_TRANS.GetComponent<BP_Charactor>().
                        set_state(chara_TRANS.GetComponent<BP_Charactor>());
        temp_TRANS.gameObject.SetActive(false);
        state_info.Chara_list.Add(temp_TRANS);
    }

    [Obsolete("See TrailController")]
    public void ToReverseState()
    {
        ////Another approach;
        //destroy_previous();
        //spawn_saved();
        reset_triggers();
        //destroy_bubble();
        revers_state();

        BPGCAnimator.SetTrigger(BP_StrDefiner.AniNextStepTrigger_str);
    }

    [Obsolete("See TrailController")]
    private void destroy_previous()
    {
        foreach (Transform chara_TRANS in BPRC.Charators_TRANSs)
        {
            chara_TRANS.GetComponent<BP_Charactor>().force_destroy();
        }
        BPRC.Charators_TRANSs.Clear();
    }

    [Obsolete("See TrailController")]
    private void spawn_saved()
    {
        Transform new_chara = null;
        foreach (Transform chara_TRANS in state_info.Chara_list)
        {
            new_chara = Instantiate(chara_TRANS);
            BPRC.Charators_TRANSs.Add(new_chara);
            new_chara.name = chara_TRANS.name.Split(name_spliter)[0];
            new_chara.GetComponent<BP_Charactor>().
                                    set_state(chara_TRANS.GetComponent<BP_Charactor>());
            new_chara.gameObject.SetActive(true);
        }
    }

    [Obsolete("See TrailController")]
    private void revers_state()
    {
        int i = 0;
        foreach (Transform chara_TRANS in BPRC.Charators_TRANSs)
        {
            chara_TRANS.GetComponent<BP_Charactor>().set_state(state_info.Chara_list[i]);
            i++;
        }
    }

    private void destroy_bubble()
    {
        foreach(Transform bubble_TRANS in BPRC.Bubble_TRANSs.ToArray())
        {
            bubble_TRANS.GetComponent<Bubble>().force_destroy();
        }
    }

    public void bubble_collided()
    {

    }

    public void activated_Bcollided()
    {
        switch (GameAcuityMode)
        {
            case AcuityMode.OneByOne:
                BPAC_script.rotate_acuity();
                break;
        }
    }

    private void reset_triggers()
    {
        foreach (AnimatorControllerParameter parameter in BPGCAnimator.parameters)
        {
            try
            {
                BPGCAnimator.ResetTrigger(parameter.name);
            }
            catch(Exception e)
            {
                Debug.Log(e);
            }
            
        }
    }

    private IEnumerator check_bubble_ready()
    {
        check_bubble_flag = true;
        while (check_bubble_flag)
        {
            Bubble_ready_flag = true;
            foreach(Transform TC_TRANS in BPRC.TrailControllers_TRANSs)
            {
                if(!TC_TRANS.GetComponent<BP_TrailController>().Ready_flag)
                {
                    Bubble_ready_flag = false;
                    break;
                }
            }

            yield return new WaitForSeconds(0.1f);
        }
    }

    private void OnDestroy()
    {
        check_bubble_flag = false;
    }

    public void bubble_hitted()
    {
        switch(GameAcuityMode)
        {
            case AcuityMode.OneByOne:
                BPAC_script.rotate_acuity();
                break;
        }
    }

}


