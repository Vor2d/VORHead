using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class FS_GameController : GeneralGameController {

    private const string score_init_str = "Score: ";

    [SerializeField] private FS_RC FSRC;

    [SerializeField] private GameObject DebugText1;

    public bool is_slicing { get; set; }

    public float SliceSpeed = 50.0f;

    public float FruitIntervalTime = 3.0f;
    public float RandRangeX = 15.0f;
    public float RandRangeY = 15.0f;
    public float RandRangeZ1 = 5.0f;
    public float RandRangeZ2 = 15.0f;
    public int ScoreIncrease = 10;

    private float fruit_inte_timer;
    private bool fruit_Itimer_flag;
    private Animator FSGCAnimator;
    private int score;
    private bool score_changed;

    // Use this for initialization
    void Start () {
        this.fruit_inte_timer = FruitIntervalTime;
        this.fruit_Itimer_flag = false;
        this.FSGCAnimator = GetComponent<Animator>();
        this.score = 0;
        this.score_changed = true;
        this.is_slicing = true;
    }
	
	// Update is called once per frame
	protected override void Update () {
		//if(fruit_Itimer_flag)
  //      {
  //          fruit_inte_timer -= Time.deltaTime;
  //      }

        update_score();

        //slice();

        DebugText1.GetComponent<TextMesh>().text =
                                GeneralMethods.getVRspeed().ToString("F2");
        //DebugText1.GetComponent<TextMesh>().text =
        //                        Debug_OBJ1.GetComponent<FS_Fruit>().speed_cal.ToString();

    }

    public void ToInit()
    {
        FSGCAnimator.SetTrigger(FS_SD.AniNextStep_str);
    }

    public void ToInstantiateFruit()
    {
        instantiate_fruit();

        FSGCAnimator.SetTrigger(FS_SD.AniNextStep_str);
    }

    private void instantiate_fruit()
    {
        GameObject fruit_obj =
                    Instantiate(FSRC.Fruit_Prefab, rand_pos_generator(), new Quaternion());
        fruit_obj.GetComponent<FS_Fruit>().start_fruit();
    }

    private Vector3 rand_pos_generator()
    {
        return new Vector3(UnityEngine.Random.Range(-RandRangeX, RandRangeX),
                            UnityEngine.Random.Range(-RandRangeY, RandRangeY),
                            UnityEngine.Random.Range(RandRangeZ1, RandRangeZ2));
    }

    public void ToWaitTime()
    {
        fruit_Itimer_flag = true;
    }

    public void WaitTime()
    {
        if(fruit_inte_timer < 0.0f)
        {
            fruit_inte_timer = FruitIntervalTime;

            FSGCAnimator.SetTrigger(FS_SD.AniNextStep_str);
        }
    }

    public void fruit_destroyed()
    {
        score += ScoreIncrease;
        score_changed = true;
    }

    private void update_score()
    {
        if(score_changed)
        {
            FSRC.ScoreText_TRANS.GetComponent<TextMesh>().text = 
                                    score_init_str + score.ToString();
            score_changed = false;
        }
    }

    [Obsolete("Not using controller to cut")]
    private void slice()
    {
        if(Input.GetAxis(FS_SD.SecondIndTrigger_name) > 0.5f)
        {
            is_slicing = true;
        }
        else
        {
            is_slicing = false;
        }
    }

    public void restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ToStartGame()
    {
        FSGCAnimator.SetTrigger(FS_SD.AniNextStep_str);
    }

    public void quit_game()
    {
        FSRC.DC_script.MSM_script.to_start_scene();
    }
}
