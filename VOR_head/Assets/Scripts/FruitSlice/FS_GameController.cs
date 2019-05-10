using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class FS_GameController : GeneralGameController {

    [SerializeField] private FS_RC FSRC;    //Reference Controller;

    [SerializeField] private string ScoreTextPre_str = "Score: ";   //Score text prefix;

    public bool Start_flag { get; private set; }
    private Animator FSGCAnimator;  //State machine animator;
    private int score;
    private int score_increase;

    // Use this for initialization
    void Start () {
        this.FSGCAnimator = GetComponent<Animator>();
        this.score = 0;
        this.score_increase = FSRC.DC_script.GameSetting.ScoreIncrPerCut;
        this.Start_flag = false;
    }
	
	// Update is called once per frame
	protected override void Update ()
    {

    }

    #region Animator
    //Init State;
    public void ToInit()
    {
        
    }

    public void ToStartGame()
    {
        FSGCAnimator.SetTrigger(FS_SD.AniNextStep_str);
        FSRC.Fruit_TRANS.GetComponent<FS_Fruit>().start_fruit();
    }

    public void ToInGame()
    {

    }
    #endregion

    public void fruit_destroyed()
    {
        score += score_increase;
        score_changed();
    }

    public void restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }


    public void quit_game()
    {
        FSRC.DC_script.MSM_script.to_start_scene();
    }

    private void score_changed()
    {
        FSRC.ScoreText_TRANS.GetComponent<TextMesh>().text = ScoreTextPre_str + score.ToString();
    }

    public void start_game()
    {
        Start_flag = true;
        FSGCAnimator.SetTrigger(FS_SD.AniStart_str);
    }



}
