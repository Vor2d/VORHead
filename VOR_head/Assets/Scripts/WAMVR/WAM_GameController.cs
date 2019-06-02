using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WAMEC;

public class WAM_GameController : GeneralGameController
{
    [SerializeField] private WAMRC RC;

    private Animator GCAnimator;
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

        register_controller();
    }

    private void OnDestroy()
    {
        deregister_controller();
    }

    private void generate_mole_center()
    {
        GameObject mole_center_OBJ = 
                Instantiate(RC.MoleCenter_Prefab, RC.MoleCenterInidcator_TRANS.position, Quaternion.identity);
        mole_center_OBJ.GetComponent<WAM_MoleCenter>().init_mole_center(RC,
                setting_cache.Mole_gener_shape, setting_cache.Mole_frame_dist,
                setting_cache.Mole_frame_num, setting_cache.Mole_size,
                setting_cache.Mole_des_time,setting_cache.Mole_frame_size);
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
        GCAnimator.SetTrigger(WAMSD.AniNextStep_trigger);
    }
    
    private void register_controller()
    {
        RC.CI_script.IndexTrigger += whac;
    }

    private void deregister_controller()
    {
        RC.CI_script.IndexTrigger -= whac;
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
}
