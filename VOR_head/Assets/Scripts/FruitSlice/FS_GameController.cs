using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class FS_GameController : GeneralGameController {

    [SerializeField] private FS_RC FSRC;    //Reference Controller;

    public string score_init_str = "Score: ";   //Score text prefix;

    public bool is_slicing { get; set; }
    private Animator FSGCAnimator;  //State machine animator;
    private int score;
    private int score_increase;

    // Use this for initialization
    void Start () {
        this.FSGCAnimator = GetComponent<Animator>();
        this.score = 0;
        this.is_slicing = true;
        this.score_increase = FSRC.DC_script.GameSetting.ScoreIncrPerCut;
    }
	
	// Update is called once per frame
	protected override void Update ()
    {

    }

    //Init State;
    public void ToInit()
    {
        FSGCAnimator.SetTrigger(FS_SD.AniNextStep_str);
    }

    public void fruit_destroyed()
    {
        score += score_increase;
        score_changed();
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

    private void score_changed()
    {
        FSRC.ScoreText_TRANS.GetComponent<TextMesh>().text = score_init_str + score.ToString();
    }
}
