using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using WAMEC;

public class WAM_GameController : GeneralGameController
{
    [SerializeField] private bool ShowTooSlow;
    [SerializeField] private float ShowTooSlowTime;
    public bool Use_self_mesh;

    private Animator GCAnimator;
    private bool check_mole_flag;
    private bool acuity_ready_flag;
    private float head_stop_timer;
    private bool check_stop_flag;
    private bool check_CD_flag;
    private float up_timer;
    private float right_timer;
    private float down_timer;
    private float left_timer;
    private bool choose_acuity_flag;
    private int score;
    private int curr_lvl;
    private int curr_trial;
    public int Check_stop_instance { get; set; }
    private bool accept_check_stop { get { return Check_stop_instance == 0; } }
    //Cache;
    private WAM_MoleCenter MC_cache;

    public static WAM_GameController IS { get; private set; }

    private void Awake()
    {
        IS = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        this.GCAnimator = GetComponent<Animator>();
        this.MC_cache = null;
        this.check_mole_flag = false;
        this.acuity_ready_flag = false;
        this.head_stop_timer = WAMSetting.IS.Head_stop_time;
        this.check_stop_flag = false;
        this.check_CD_flag = false;
        this.up_timer = 0.0f;
        this.right_timer = 0.0f;
        this.down_timer = 0.0f;
        this.left_timer = 0.0f;
        this.choose_acuity_flag = false;
        this.score = 0;
        this.Check_stop_instance = 0;
        this.curr_lvl = -1;
        this.curr_trial = -1;

        register_controller();
        if(WAMSetting.IS.Controller_mode == ControllerModes.time_delay
            || WAMSetting.IS.Controller_mode == ControllerModes.TD_PJ)
        {
            check_CD_flag = true;
            up_timer = WAMSetting.IS.Controller_Dtime;
            right_timer = WAMSetting.IS.Controller_Dtime;
            down_timer = WAMSetting.IS.Controller_Dtime;
            left_timer = WAMSetting.IS.Controller_Dtime;
        }
        if(WAMSetting.IS.Controller_mode == ControllerModes.post_judge
            || WAMSetting.IS.Controller_mode == ControllerModes.TD_PJ)
        {
            choose_acuity_flag = true;
        }

        adjust_BG_grid();
        init_fishnet();
    }

    protected override void Update()
    {
        update_check_stop();
        if(acuity_ready_flag)
        {
            check_head();
        }
        if(check_stop_flag)
        {
            check_stop();
        }
        if(check_CD_flag)
        {
            check_cont_delay();
        }
    }

    private void OnDestroy()
    {
        deregister_controller();
    }

    private void init_fishnet()
    {
        WAMRC.IS.Fishnet_TRANS.GetComponent<WAM_Fishnet>().init(WAMSetting.IS.Net_size);
    }

    private void adjust_BG_grid()
    {
        WAMRC.IS.BG_Grid_TRANS.localScale = new Vector3(WAMSetting.IS.BGGrid_HScale,
            WAMSetting.IS.BGGrid_VScale, 0.0f);
    }

    private void update_check_stop()
    {
        if(WAMSetting.IS.Stop_on_bubble)
        {
            Check_stop_instance = Check_stop_instance < 0 ? 0 : Check_stop_instance;
            check_stop_flag = Check_stop_instance > 0;
        }
    }

    private void check_stop()
    {
        if(GeneralMethods.getVRspeed().magnitude < WAMSetting.IS.Head_stop_speed)
        {
            head_stop_timer -= Time.deltaTime;
            if(head_stop_timer < 0)
            {
                Check_stop_instance = 0;
                check_stop_flag = false;
                head_stop_timer = WAMSetting.IS.Head_stop_time;
                head_stopped();
            }
        }
        else
        {
            head_stop_timer = WAMSetting.IS.Head_stop_time;
        }
    }

    private void head_stopped()
    {
        if (WAMSetting.IS.Acuity_process == AcuityProcess.post)
        {
            MC_cache.generate_acuity();
        }
    }

    private void check_head()
    {
        if(GeneralMethods.getVRspeed().magnitude > WAMSetting.IS.Head_speed)
        {
            acuity_ready_flag = false;
            check_stop_flag = true;
        }
    }

    private void register_controller()
    {
        if(!WAMSetting.IS.Using_acuity)
        {
            WAMRC.IS.CI_script.IndexTrigger += whac;
        }
        else
        {
            switch(WAMSetting.IS.Controller_mode)
            {
                case ControllerModes.instant:
                    WAMRC.IS.CI_script.ForwardAction += whac_up;
                    WAMRC.IS.CI_script.RightAction += whac_right;
                    WAMRC.IS.CI_script.BackAction += whac_down;
                    WAMRC.IS.CI_script.LeftAction += whac_left;
                    break;
                case ControllerModes.post_judge:
                    WAMRC.IS.CI_script.ForwardAction += choose_Aup;
                    WAMRC.IS.CI_script.RightAction += choose_Aright;
                    WAMRC.IS.CI_script.BackAction += choose_Adown;
                    WAMRC.IS.CI_script.LeftAction += choose_Aleft;
                    break;
                case ControllerModes.TD_PJ:
                    WAMRC.IS.CI_script.ForwardAction += choose_Aup;
                    WAMRC.IS.CI_script.RightAction += choose_Aright;
                    WAMRC.IS.CI_script.BackAction += choose_Adown;
                    WAMRC.IS.CI_script.LeftAction += choose_Aleft;
                    break;
            }

        }
        WAMRC.IS.CI_script.Button_B += recenter_VR;
        WAMRC.IS.CI_script.Button_A += go_next_level;
    }

    private void deregister_controller()
    {
        if (!WAMSetting.IS.Using_acuity)
        {
            WAMRC.IS.CI_script.IndexTrigger -= whac;
        }
        else
        {
            switch (WAMSetting.IS.Controller_mode)
            {
                case ControllerModes.instant:
                    WAMRC.IS.CI_script.ForwardAction -= whac_up;
                    WAMRC.IS.CI_script.RightAction -= whac_right;
                    WAMRC.IS.CI_script.BackAction -= whac_down;
                    WAMRC.IS.CI_script.LeftAction -= whac_left;
                    break;
                case ControllerModes.post_judge:
                    WAMRC.IS.CI_script.ForwardAction -= choose_Aup;
                    WAMRC.IS.CI_script.RightAction -= choose_Aright;
                    WAMRC.IS.CI_script.BackAction -= choose_Adown;
                    WAMRC.IS.CI_script.LeftAction -= choose_Aleft;
                    break;
                case ControllerModes.TD_PJ:
                    WAMRC.IS.CI_script.ForwardAction -= choose_Aup;
                    WAMRC.IS.CI_script.RightAction -= choose_Aright;
                    WAMRC.IS.CI_script.BackAction -= choose_Adown;
                    WAMRC.IS.CI_script.LeftAction -= choose_Aleft;
                    break;
            }

        }
        WAMRC.IS.CI_script.Button_B -= recenter_VR;
        WAMRC.IS.CI_script.Button_A -= go_next_level;
    }

    private void go_next_level()
    {
        GCAnimator.SetTrigger(WAMSD.AniNextLevel_trigger);
    }
    
    private void whac()
    {
        if(MC_cache != null)
        {
            MC_cache.whac();
        }
    }

    public void start_game()
    {
        GCAnimator.SetTrigger(WAMSD.AniStart_trigger);
    }

    public void quit_game()
    {
        WAM_DataController.IS.MSM_script.to_start_scene();
    }

    public void ToCheckMole()
    {
        check_mole_flag = true;
    }

    public void CheckMole()
    {
        if(check_mole_flag && MC_cache.mole_TRANSs.Count == 0)
        {
            check_mole_flag = false;
            GCAnimator.SetTrigger(WAMSD.AniNextStep_trigger);
        }
    }

    private void whac_up()
    {
        MC_cache.whac_acuity((int)Controller_Input.FourDirInput.up);
    }
    private void whac_right()
    {
        MC_cache.whac_acuity((int)Controller_Input.FourDirInput.right);
    }
    private void whac_down()
    {
        MC_cache.whac_acuity((int)Controller_Input.FourDirInput.down);
    }
    private void whac_left ()
    {
        MC_cache.whac_acuity((int)Controller_Input.FourDirInput.left);
    }

    private void check_cont_delay()
    {
        if (check_CDdir(WAMRC.IS.CI_script.Forward_flag, ref up_timer)) { whac_up(); }
        if (check_CDdir(WAMRC.IS.CI_script.Right_flag, ref right_timer)) { whac_right(); }
        if (check_CDdir(WAMRC.IS.CI_script.Back_flag, ref down_timer)) { whac_down(); }
        if (check_CDdir(WAMRC.IS.CI_script.Left_flag, ref left_timer)) { whac_left(); }
    }

    private bool check_CDdir(bool flag,ref float timer)
    {
        if (flag)
        {
            timer -= Time.deltaTime;
            if (timer < 0)
            {
                timer = WAMSetting.IS.Controller_Dtime;
                return true;
            }
        }
        else
        {
            timer = WAMSetting.IS.Controller_Dtime;
        }
        return false;
    }

    private void choose_Aup()
    {
        choose_acuity((int)Controller_Input.EightDirInput.up);
    }

    private void choose_Aright()
    {
        choose_acuity((int)Controller_Input.EightDirInput.right);
    }

    private void choose_Adown()
    {
        choose_acuity((int)Controller_Input.EightDirInput.down);
    }

    private void choose_Aleft()
    {
        choose_acuity((int)Controller_Input.EightDirInput.left);
    }

    private void choose_acuity(int direction)
    {
        MC_cache.choose_acuity(direction);
    }

    public void wrong_whac()
    {

    }

    public void success_whac(Transform MT = null, int dir = (int)Controller_Input.FourDirInput.empty)
    {
        score += 10;
        update_score_text();
        if (WAMSetting.IS.Use_Fishnet) { WAMRC.IS.Fishnet_TRANS.GetComponent<WAM_Fishnet>().start_net(MT, dir); }
    }

    public void mole_reached()
    {
        if(WAMSetting.IS.Check_too_slow && !check_stop_flag)
        {
            too_slow();
        }
    }

    private void too_slow()
    {
        if(ShowTooSlow)
        {
            WAMRC.IS.Text1_TRANS.GetComponent<TextMesh>().text = "Too Slow!";
            GeneralMethods.show_obj(WAMRC.IS.Text1_TRANS, ShowTooSlowTime);
        }
        MC_cache.too_slow();
    }

    private void update_score_text()
    {
        WAMRC.IS.ScoreText_TRANS.GetComponent<TextMesh>().text = "Score: " + score;
    }

    #region Animator

    public void ToInit()
    {
        //generate_mole_center();
        //GCAnimator.SetTrigger(WAMSD.AniNextStep_trigger);
        update_score_text();
    }

    private void generate_mole_center()
    {
        GameObject mole_center_OBJ = Instantiate(WAMRC.IS.MoleCenter_Prefab,
                                        WAMRC.IS.MoleCenterInidcator_TRANS.position,
                                        Quaternion.identity);
        mole_center_OBJ.GetComponent<WAM_MoleCenter>().init_mole_center();
        mole_center_OBJ.GetComponent<WAM_MoleCenter>().generate_mole_frame();
        WAMRC.IS.MoleCenter_TRANS = mole_center_OBJ.transform;
        MC_cache = mole_center_OBJ.GetComponent<WAM_MoleCenter>();
    }

    public void ToStartGame()
    {
        GCAnimator.SetTrigger(WAMSD.AniNextStep_trigger);
    }

    public void ToSpawnMole()
    {
        MC_cache.generate_mole(WAMSetting.IS.Mole_gener_type,use_Smesh: Use_self_mesh);
        if (WAMSetting.IS.Using_acuity)
        {
            acuity_ready_flag = true;
            check_stop_flag = false;
        }
        GCAnimator.SetTrigger(WAMSD.AniNextStep_trigger);
    }

    public void ToStartLevel()
    {
        clear_level();
        curr_lvl++;
        generate_mole_center();
        GCAnimator.SetTrigger(WAMSD.AniNextStep_trigger);
    }

    private void clear_level()
    {
        if (WAMRC.IS.MoleCenter_TRANS != null) 
        { Destroy(WAMRC.IS.MoleCenter_TRANS.gameObject); }
    }

    public void ToStartTrial()
    {
        curr_trial++;
        update_trial();
        GCAnimator.SetTrigger(WAMSD.AniNextStep_trigger);
    }

    private void update_trial()
    {

    }

    #endregion

}
