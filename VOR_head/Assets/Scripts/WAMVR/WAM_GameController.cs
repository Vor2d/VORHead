using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WAMEC;

public class WAM_GameController : GeneralGameController
{
    [SerializeField] private WAMRC RC;

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
    //Cache;
    private WAM_DataController DC_cahce;
    private WAMSetting setting_cache;
    private WAM_MoleCenter MC_cache;

    // Start is called before the first frame update
    void Start()
    {
        this.DC_cahce = RC.DC_script;
        this.setting_cache = WAMSetting.Instance;
        this.GCAnimator = GetComponent<Animator>();
        this.MC_cache = null;
        this.check_mole_flag = false;
        this.acuity_ready_flag = false;
        this.head_stop_timer = setting_cache.Head_stop_time;
        this.check_stop_flag = false;
        this.check_CD_flag = false;
        this.up_timer = 0.0f;
        this.right_timer = 0.0f;
        this.down_timer = 0.0f;
        this.left_timer = 0.0f;

        register_controller();
        if(setting_cache.Controller_mode == ControllerModes.time_delay)
        {
            check_CD_flag = true;
            up_timer = setting_cache.Controller_Dtime;
            right_timer = setting_cache.Controller_Dtime;
            down_timer = setting_cache.Controller_Dtime;
            left_timer = setting_cache.Controller_Dtime;
        }
    }

    protected override void Update()
    {
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

    private void check_stop()
    {
        if(GeneralMethods.getVRspeed().magnitude < setting_cache.Head_stop_speed)
        {
            head_stop_timer -= Time.deltaTime;
            if(head_stop_timer < 0)
            {
                check_stop_flag = false;
                head_stop_timer = setting_cache.Head_stop_time;
                if(setting_cache.Acuity_process == ACuityProcess.post)
                {
                    MC_cache.generate_acuity();
                }
            }
        }
        else
        {
            head_stop_timer = setting_cache.Head_stop_time;
        }
    }

    private void check_head()
    {
        if(GeneralMethods.getVRspeed().magnitude > setting_cache.Head_speed)
        {
            acuity_ready_flag = false;
            check_stop_flag = true;
        }
    }

    private void generate_mole_center()
    {
        GameObject mole_center_OBJ = 
                Instantiate(RC.MoleCenter_Prefab, RC.MoleCenterInidcator_TRANS.position, Quaternion.identity);
        mole_center_OBJ.GetComponent<WAM_MoleCenter>().init_mole_center(RC,
                                        setting_cache.Mole_gener_shape, setting_cache.Mole_frame_dist,
                                        setting_cache.Mole_frame_num, setting_cache.Mole_size,
                                        setting_cache.Mole_des_time,setting_cache.Mole_frame_size,
                                        setting_cache.Mole_frame_dist2,setting_cache.Mole_frame_num2,
                                        setting_cache.Gener_list,setting_cache.Using_acuity,
                                        setting_cache.Acuity_rela_size,setting_cache.Acuity_type,
                                        setting_cache.Acuity_flash_time,setting_cache.Min_distance);
        mole_center_OBJ.GetComponent<WAM_MoleCenter>().generate_mole_frame();
        RC.MoleCenter_TRANS = mole_center_OBJ.transform;
        MC_cache = mole_center_OBJ.GetComponent<WAM_MoleCenter>();
    }

    public void GC_Init()
    {
        generate_mole_center();
        GCAnimator.SetTrigger(WAMSD.AniNextStep_trigger);
    }

    public void ToStartGame()
    {
        GCAnimator.SetTrigger(WAMSD.AniNextStep_trigger);
    }

    public void ToSpawnMole()
    {
        MC_cache.generate_mole(setting_cache.Mole_gener_type);
        if(setting_cache.Using_acuity)
        {
            acuity_ready_flag = true;
            check_stop_flag = false;
        }
        GCAnimator.SetTrigger(WAMSD.AniNextStep_trigger);
    }
    
    private void register_controller()
    {
        if(!setting_cache.Using_acuity)
        {
            RC.CI_script.IndexTrigger += whac;
        }
        else if(setting_cache.Controller_mode == ControllerModes.instant)
        {
            RC.CI_script.ForwardAction += whac_up;
            RC.CI_script.RightAction += whac_right;
            RC.CI_script.BackAction += whac_down;
            RC.CI_script.LeftAction += whac_left;
        }
        RC.CI_script.Button_B += recenter_VR;
    }

    private void deregister_controller()
    {
        if (!setting_cache.Using_acuity)
        {
            RC.CI_script.IndexTrigger -= whac;
        }
        else if (setting_cache.Controller_mode == ControllerModes.instant)
        {
            RC.CI_script.ForwardAction -= whac_up;
            RC.CI_script.RightAction -= whac_right;
            RC.CI_script.BackAction -= whac_down;
            RC.CI_script.LeftAction -= whac_left;
        }
        RC.CI_script.Button_B -= recenter_VR;
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
        RC.DC_script.MSM_script.to_start_scene();
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
        if (check_CDdir(RC.CI_script.Forward_flag, ref up_timer)) { whac_up(); }
        if (check_CDdir(RC.CI_script.Right_flag, ref right_timer)) { whac_right(); }
        if (check_CDdir(RC.CI_script.Back_flag, ref down_timer)) { whac_down(); }
        if (check_CDdir(RC.CI_script.Left_flag, ref left_timer)) { whac_left(); }
    }

    private bool check_CDdir(bool flag,ref float timer)
    {
        if (flag)
        {
            timer -= Time.deltaTime;
            if (timer < 0)
            {
                timer = setting_cache.Controller_Dtime;
                return true;
            }
        }
        else
        {
            timer = setting_cache.Controller_Dtime;
        }
        return false;
    }
}
