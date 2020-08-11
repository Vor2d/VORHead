using System;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Animator))]
public class FS_GameController : GeneralGameController {

    [SerializeField] private string ScoreTextPre_str;   //Score text prefix;
    [SerializeField] private string ScoreTextPost_str;   //Score text postfix;

    public bool Start_flag { get; private set; }
    public int Trial_index { get { return curr_trial_index; } }

    private Animator FSGCAnimator;  //State machine animator;
    [Obsolete("Score is in player")]
    private int score;
    [Obsolete("Score is in player")]
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
        this.curr_trial_index = 0;
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
        FS_RC.IS.CI_script.LeftAction += panel_toggle_left;
        FS_RC.IS.CI_script.RightAction += panel_toggle_right;
        FS_RC.IS.CI_script.IndexTrigger += panel_select;
    }

    private void deregister_controller()
    {
        FS_RC.IS.CI_script.Button_Y -= recenter_VR;
        FS_RC.IS.CI_script.Button_X -= trace_back_button;
        FS_RC.IS.CI_script.LeftAction -= panel_toggle_left;
        FS_RC.IS.CI_script.RightAction -= panel_toggle_right;
        FS_RC.IS.CI_script.IndexTrigger -= panel_select;
    }

    [Obsolete("Handled in FS_Fruit")]
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
        string score_str = "";
        score_str += ScoreTextPre_str;
        score_str += FS_Player.IS.Total_star.ToString();
        score_str += ScoreTextPost_str;
        FS_RC.IS.ScoreText_TRANS.GetComponent<TextMesh>().text = score_str;
    }

    public void start_game()
    {
        Start_flag = true;
        FSGCAnimator.SetTrigger(FS_SD.AniStart_str);
    }
    
    [Obsolete("")]
    private void next_trial_but()
    {
        FSGCAnimator.SetTrigger(FS_SD.AniNextTrial_str);
    }

    private void trace_back_button()
    {
        FS_Fruit.IS.trace_back_act();
    }

    //Init State;
    public void ToInit()
    {
        init_player();
        score_changed();
    }

    private void init_player()
    {
        FS_RC.IS.player = FS_DataController.IS.player;
        FS_RC.IS.player.init_levels(FS_RC.IS.Level_infos.Count);
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
        //FS_RC.IS.Fruit_Ani.SetTrigger(FS_SD.AniNextTrial_str);
        FS_Fruit.IS.selected();
        FS_LevelSelectPanel.IS.enter_game();
    }

    private void panel_toggle_left()
    {
        FS_LevelSelectPanel.IS.toggle_left_act();
    }

    private void panel_toggle_right()
    {
        FS_LevelSelectPanel.IS.toggle_right_act();
    }

    private void panel_select()
    {
        FS_LevelSelectPanel.IS.select_act();
    }

    public void ToSelectTrial()
    {
        update_player_unlock();
        FS_LevelSelectPanel.IS.start_select_panel();
        turn_on_score_text();
    }

    public void LeaveSelectTrial()
    {
        turn_off_score_text();
    }

    private void update_player_unlock()
    {
        check_n_unlock_level(FS_RC.IS.player);
    }

    private void check_n_unlock_level(FS_Player player)
    {
        int player_star = player.get_refreshed_SC_STA().Item2;
        FS_TrialGroup level_info = null;
        for(int i = 0;i< FS_RC.IS.Level_infos.Count;i++)
        {
            level_info = FS_RC.IS.Level_infos[i];
            if (level_info.Stars_to_unlock <= player_star) { player.unlock_level(i); }
        }
    }

    public void trial_selected(int trial_index)
    {
        if (!check_valid_select(trial_index)) { return; }
        else
        {
            curr_trial_index = trial_index;
            FSGCAnimator.SetTrigger(FS_SD.AniNextStep_str);
        }
    }

    private bool check_valid_select(int trial_index)
    {
        return FS_RC.IS.player.get_level_lock_stat(trial_index);
    }

    public void trial_finished(int score, int star)
    {
        update_player_SC_STA(score, star);
        score_changed();
        FSGCAnimator.SetTrigger(FS_SD.AniNextTrial_str);
    }

    public void update_player_SC_STA(int score, int star)
    {
        FS_RC.IS.player.update_level_info(curr_trial_index, score, star);
    }

    private void turn_on_score_text()
    {
        FS_RC.IS.ScoreText_TRANS.GetComponent<MeshRenderer>().enabled = true;
        FS_RC.IS.ScoreStar_TRANS.GetComponent<SpriteRenderer>().enabled = true;
    }

    private void turn_off_score_text()
    {
        FS_RC.IS.ScoreText_TRANS.GetComponent<MeshRenderer>().enabled = false;
        FS_RC.IS.ScoreStar_TRANS.GetComponent<SpriteRenderer>().enabled = false;
    }

    #region Static

    public static int Start_cal(float curr_scr, float max_scr)
    {
        int max_star = FS_Setting.IS.MaxStar;
        float scr_per_star = max_scr / (float)max_star;
        float shift = scr_per_star * FS_Setting.IS.StarCalOffset;
        float ratio = (curr_scr + shift) / max_scr;
        int star = Mathf.FloorToInt((float)max_star * ratio);
        return star;
    }

    #endregion
}
