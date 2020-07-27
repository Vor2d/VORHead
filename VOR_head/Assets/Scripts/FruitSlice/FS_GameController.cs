using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Animator))]
public class FS_GameController : GeneralGameController {

    [SerializeField] private const string ScoreTextPre_str = "Score: ";   //Score text prefix;

    public bool Start_flag { get; private set; }

    private Animator FSGCAnimator;  //State machine animator;
    private int score;
    private int score_increase;
    private int curr_trial_index;

    public static FS_GameController IS { get; set; }

    private void Awake()
    {
        IS = this;

        this.FSGCAnimator = null;
        this.score = 0;
        this.Start_flag = false;
        this.score_increase = 0;
        this.curr_trial_index = -1;
    }

    // Use this for initialization
    void Start ()
    {
        FSGCAnimator = GetComponent<Animator>();
        score_increase = FS_DataController.IS.GameSetting.ScoreIncrPerCut;
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
        FS_RC.IS.CI_script.Button_Y += recenter_VR;
        FS_RC.IS.CI_script.Button_X += trace_back_button;
    }

    private void deregister_controller()
    {
        FS_RC.IS.CI_script.Button_Y -= recenter_VR;
        FS_RC.IS.CI_script.Button_X -= trace_back_button;
    }


    public void fruit_destroyed()
    {
        score += score_increase;
        score_changed();
    }

    //public void restart()
    //{
    //    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    //}

    public void quit_game()
    {
        FS_DataController.IS.MSM_script.to_start_scene();
    }

    private void score_changed()
    {
        FS_RC.IS.ScoreText_TRANS.GetComponent<TextMesh>().text = ScoreTextPre_str + score.ToString();
    }

    public void start_game()
    {
        Start_flag = true;
        FSGCAnimator.SetTrigger(FS_SD.AniStart_str);
    }

    private void next_trial_but()
    {
        FSGCAnimator.SetTrigger(FS_SD.AniNextTrial_str);
    }

    private void trace_back_button()
    {
        FS_Fruit.IS.trace_back_act();
    }

    #region Animator
    //Init State;
    public void ToInit()
    {
        
    }

    public void ToStartGame()
    {
        FS_RC.IS.Fruit_TRANS.GetComponent<FS_Fruit>().start_fruit();
        FSGCAnimator.SetTrigger(FS_SD.AniNextStep_str);
    }

    public void ToInGame()
    {

    }

    public void ToLoadTrial()
    {
        load_trial();
        FSGCAnimator.SetTrigger(FS_SD.AniNextStep_str);
    }

    private void load_trial()
    {
        curr_trial_index++;
        Vector2[] poss = mesh_size_cal(curr_trial_index);
        FS_Fruit.IS.load_trial(poss, FS_DataController.IS.FruitTextures[curr_trial_index]);
        FS_RC.IS.Fruit_Ani.SetTrigger(FS_SD.AniNextTrial_str);
    }

    /// <summary>
    /// Calculate the rectangles for the mesh;
    /// </summary>
    /// <param name="tex_index"></param>
    /// <returns>{upper left point, down right point}</returns>
    private Vector2[] mesh_size_cal(int tex_index)
    {
        Vector2[] poss = new Vector2[2];
        Texture2D texture = FS_DataController.IS.FruitTextures[tex_index];
        float h_w_ratio = (float)texture.height / (float)texture.width;
        float mesh_h = 0.0f, mesh_w = 0.0f;
        float frame_size = FS_DataController.IS.GameSetting.FruitFrameSize;
        if (h_w_ratio >= 1.0f)
        {
            mesh_h = frame_size;
            mesh_w = frame_size / h_w_ratio;
        }
        else
        {
            mesh_h = frame_size * h_w_ratio;
            mesh_w = frame_size;
        }
        poss[0] = new Vector2(mesh_w / 2.0f * -1.0f, mesh_h / 2.0f);
        poss[1] = new Vector2(mesh_w / 2.0f, mesh_h / 2.0f * -1.0f);
        //Debug.Log("!!!!! " + poss[0].ToString() + poss[1].ToString());
        return poss;
    }
    #endregion

}
