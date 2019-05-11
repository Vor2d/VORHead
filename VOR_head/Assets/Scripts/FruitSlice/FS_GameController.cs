using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Animator))]
public class FS_GameController : GeneralGameController {

    [SerializeField] private FS_RC FSRC;    //Reference Controller;

    [SerializeField] private string ScoreTextPre_str = "Score: ";   //Score text prefix;

    public bool Start_flag { get; private set; }
    private Animator FSGCAnimator;  //State machine animator;
    private int score;
    private int score_increase;

    private void Awake()
    {
        this.FSGCAnimator = null;
        this.score = 0;
        this.Start_flag = false;
        this.score_increase = 0;
    }

    // Use this for initialization
    void Start ()
    {
        FSGCAnimator = GetComponent<Animator>();
        score_increase = FSRC.DC_script.GameSetting.ScoreIncrPerCut;
        register_controller();
    }
	
	// Update is called once per frame
	protected override void Update ()
    {

    }

    private void OnDestroy()
    {
        deregister_controller();
    }

    private void register_controller()
    {
        FSRC.CI_script.Button_B += recenter_VR;
    }

    private void deregister_controller()
    {
        FSRC.CI_script.Button_B -= recenter_VR;
    }


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

    #region Animator
    //Init State;
    public void ToInit()
    {
        
    }

    public void ToStartGame()
    {
        FSRC.Fruit_TRANS.GetComponent<FS_Fruit>().start_fruit();
        FSGCAnimator.SetTrigger(FS_SD.AniNextStep_str);
    }

    public void ToInGame()
    {

    }
    #endregion

}
