using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SpatialTracking;
using UnityEngine.SceneManagement;
using TMPro;

[RequireComponent(typeof(AmmoSystem))]
public class MD_GameController : GeneralGameController {

    [SerializeField] private Transform Camera1I_TRANS;
    [SerializeField] private Transform Camera2I_TRANS;
    [SerializeField] private Transform CameraParent_TRANS;
    [SerializeField] private Transform Body_TRANS;
    [SerializeField] private Aim Aim_script;
    [SerializeField] private RightController RC_script;
    [SerializeField] private Transform AimT_TRANS;
    [SerializeField] private Transform ScoreText_TRANS;
    [SerializeField] private Transform WaveText_TRANS;
    [SerializeField] private AmmoSystem AS_script;
    [SerializeField] private Transform AmmoText_TRANS;
    [SerializeField] private Transform OOAText_TRANS;
    [SerializeField] private MD_TargetRayCast MDTRC_script;
    [SerializeField] private GameObject ExplodePrefab;
    [SerializeField] private GameObject[] MissilePrefabs;
    [SerializeField] private Transform MenuTMP_TRANS;
    [SerializeField] private Transform LogSystem_TRANS;
    [SerializeField] private Controller_Input CI_script;

    [Header("Variables")]
    public float InstRandomRange = 9.0f;
    public float InstY = 4.0f;
    public float InstZ = 10.0f;
    public float WaveInterTime = 2.0f;
    public int ScoreIncerase = 10;
    [SerializeField] private float SmallExplodeScale = 0.1f;
    [SerializeField] private bool ScoreNotNegative = true;
    [SerializeField] private bool AutoLogVR = false;
    [SerializeField] private bool ReSummonCity = false;
    [Header("BonusSystem")]
    public bool UsingBonusSystem = false;
    public int[] BonusScores;
    [Header("PunishSystem")]
    public bool UsingPunishSystem = false;
    public int GetHitPunish = 0;
    public int GetDestroyPunish = 0;

    public MD_DataController MDDC_script { get; private set; }
    private Animator MD_GC_Animator;
    private GameObject[] total_cities;
    private MD_WaveInfo wave_info;
    private Transform reload_collider_TRANS;
    #region(ThisWaveInfo)
    private int wave_number;
    private int difficulty_ratio;
    private int missile_number_this_wave;
    private float missile_Itime_this_wave;
    private float missile_speed_this_wave;
    private int ammo_this_wave;
    #endregion
    #region(RunTimeInfo)
    public bool City_destroied { get; set; }
    public bool Menu_gazing_flag { get; set; }
    public bool Reload_gazing_flag { get; set; }
    private bool first_camera_on;
    private bool start_flag;
    private GameObject[] alive_cities;
    private int alive_city_number;
    private bool missile_timer_flag;
    private float missile_timer;
    private bool score_changed_flag;
    private int score;
    private bool wave_inter_flag;
    private float wave_inter_timer;
    private int current_alive_missile_number;
    private bool ammo_changed_flag;
    private int current_MTI_number; //missile to instantiate;
    private bool missile_number_checked;
    private bool city_destroyed_this_wave;
    #endregion

    private void Awake()
    {
        if (MDDC_script == null)
        {
            MDDC_script = GameObject.Find(MD_StrDefiner.DataController_name).
                                            GetComponent<MD_DataController>();
        }
    }

    // Use this for initialization
    void Start ()
    {

        this.City_destroied = false;
        this.alive_city_number = 0;
        this.MD_GC_Animator = GetComponent<Animator>();
        this.missile_timer_flag = false;
        this.missile_timer = MDDC_script.MissileInterTime;
        this.score = 0;
        this.score_changed_flag = true;
        this.first_camera_on = false;
        this.start_flag = false;
        this.wave_number = -1;
        this.wave_info = new MD_WaveInfo();
        this.current_MTI_number = -1;
        this.wave_inter_timer = WaveInterTime;
        this.wave_inter_flag = false;
        this.current_alive_missile_number = -1;
        this.ammo_changed_flag = true;
        this.Menu_gazing_flag = false;
        this.city_destroyed_this_wave = false;
        this.ammo_this_wave = 0;
        this.missile_number_this_wave = 0;
        this.Reload_gazing_flag = false;
        this.reload_collider_TRANS = null;
        this.missile_number_checked = false;
        this.missile_Itime_this_wave = 0.0f;
        this.difficulty_ratio = 0;
        this.missile_speed_this_wave = 0.0f;
        
        wave_info.set_data(MD_WaveDefiner.WaveInfo_list);
        set_random();
        register_controller();
    }
	
	// Update is called once per frame
	protected override void Update ()
    {
        //Debug.Log("city number " + alive_city_number);
        //Debug.Log("City_tag " + MD_StrDefiner.City_tag);

        base.Update();

        if(missile_timer_flag)
        {
            missile_timer -= GeneralGameController.GameDeltaTime;
        }
        if(wave_inter_flag)
        {
            wave_inter_timer -= GeneralGameController.GameDeltaTime;
        }

        update_score_text();
        update_ammo_text();
    }

    private void LateUpdate()
    {
        if(City_destroied)
        {
            city_destroied();
            update_cities();
            City_destroied = false;
        }
    }

    private void OnDestroy()
    {
        de_register_controller();
    }

    private void register_controller()
    {
        CI_script.Button_B += recenter;
        if(MDDC_script.UsingReloadSystem)
        {
            CI_script.IndexTrigger += reload_ammo;
        }
    }

    private void de_register_controller()
    {
        CI_script.Button_B -= recenter;
        if (MDDC_script.UsingReloadSystem)
        {
            CI_script.IndexTrigger -= reload_ammo;
        }
    }

    public void recenter()
    {
        UnityEngine.XR.InputTracking.Recenter();
    }

    private void log_VR()
    {
        if(AutoLogVR)
        {
            LogSystem_TRANS.GetComponent<VRLogSystem>().toggle_Thread();
        }
    }
        

    public void ToInstantiatetMissile()
    {
        current_MTI_number--;
        if(current_MTI_number < 0)
        {
            MD_GC_Animator.SetTrigger(MD_StrDefiner.AnimatorStartWaveTrigger_str);
            return;
        }
        instantiate_missiles();
        MD_GC_Animator.SetTrigger(MD_StrDefiner.AnimatorNextStepTrigger_str);
    }

    private void instantiate_missiles()
    {
        if(alive_city_number > 0)
        {
            GameObject Missile_OBJ =
            Instantiate(MissilePrefabs[Random.Range(0, MissilePrefabs.Length)],
                        RandomPosGenerate(), Quaternion.identity);
            Missile Missile_Script = Missile_OBJ.GetComponent<Missile>();
            int city_index = Random.Range(0, alive_city_number);
            Missile_Script.set_target(alive_cities[city_index].transform);
            Missile_Script.start_move(missile_speed_this_wave);
        }
    }

    private Vector3 RandomPosGenerate()
    {
        return new Vector3(Random.Range(-InstRandomRange, InstRandomRange),
                            InstY, InstZ);
    }

    public void ToWaitForNextMissile()
    {
        if(!MDDC_script.OneMissilePreTime)
        {
            missile_timer_flag = true;
        }
        else
        {
            check_missile_number();
        }
    }

    public void WaitForNextMissile()
    {
        if(!MDDC_script.OneMissilePreTime)
        {
            if (missile_timer <= 0.0f)
            {
                missile_timer_flag = false;
                missile_timer = missile_Itime_this_wave;
                MD_GC_Animator.SetTrigger(MD_StrDefiner.AnimatorNextStepTrigger_str);
            }
        }
        else
        {
            if(missile_number_checked && current_alive_missile_number <= 0)
            {
                MD_GC_Animator.SetTrigger(MD_StrDefiner.AnimatorNextStepTrigger_str);
            }
        }
    }

    public void ExitWaitForNextMissile()
    {
        missile_timer_flag = false;
    }

    private void city_destroied()
    {
        city_destroyed_this_wave = true;
        if(UsingPunishSystem)
        {
            score -= GetDestroyPunish;
            score_changed_flag = true;
        }
    }

    public void update_cities()
    {
        List<GameObject> temp_cities = new List<GameObject>();
        foreach(GameObject city in GameObject.FindGameObjectsWithTag(MD_StrDefiner.City_tag))
        {
            if(city.GetComponent<City>().curr_health > 0)
            {
                temp_cities.Add(city);
            }
        }
        alive_cities = temp_cities.ToArray();
        alive_city_number = alive_cities.Length;
        check_game_over();
    }

    private void check_game_over()
    {
        if (alive_city_number <= 0)
        {
            MD_GC_Animator.SetTrigger(MD_StrDefiner.AnimatorGameOverTrigger_str);
        }
    }

    public void IE_with_raycast()
    {
        instantiate_explode(MDTRC_script.TB_hit_position);
    }

    private void instantiate_explode(Vector3 target_pos)
    {
        GameObject explode =
                    Instantiate(ExplodePrefab, target_pos, Quaternion.identity);
        explode.GetComponent<ExplodeGroup>().start_explode_group(MDDC_script.ExplodeTime,
                                                            MDDC_script.ExplodeRaduis,
                                                            UsingBonusSystem,
                                                            MDDC_script.UsingExplodeOutline);

    }

    //private void instantiate_explode(Vector3 target_pos,float scale)
    //{
    //    GameObject explode =
    //                Instantiate(ExplodePrefab, target_pos, Quaternion.identity);
    //    explode.GetComponent<Explode>().set_radius_scale(scale);
    //    explode.GetComponent<Explode>().
    //                        start_exp(ExplodeTime, ExplodeRaduis, UsingBonusSystem);
    //}

    public void missile_destroyed()
    {
        score += ScoreIncerase;
        score_changed_flag = true;
        //StartCoroutine(check_missile_number());
    }

    private void update_score_text()
    {
        if(score_changed_flag)
        {
            if(score < 0)
            {
                score = 0;
            }
            ScoreText_TRANS.GetComponent<TextMesh>().text = 
                        MD_StrDefiner.ScoreTextPreStr + score.ToString();
            score_changed_flag = false;
        }
        
    }

    private void toggle_camera()
    {
        if(first_camera_on)
        {
            Body_TRANS.position = Camera2I_TRANS.position;
            Body_TRANS.rotation = Camera2I_TRANS.rotation;
            Body_TRANS.localScale = Camera2I_TRANS.localScale;
            RC_script.turn_off_controller();

            first_camera_on = false;
            //Camera.main.GetComponent<TrackedPoseDriver>().trackingType =
            //                    TrackedPoseDriver.TrackingType.RotationOnly;
        }
        else
        {
            Body_TRANS.position = Camera1I_TRANS.position;
            Body_TRANS.rotation = Camera1I_TRANS.rotation;
            Body_TRANS.localScale = Camera1I_TRANS.localScale;
            RC_script.turn_on_controller();

            first_camera_on = true;
            //Camera.main.GetComponent<TrackedPoseDriver>().trackingType =
            //                TrackedPoseDriver.TrackingType.RotationAndPosition;
        }
    }

    public void back_to_start_scene()
    {
        Time.timeScale = 1.0f;
        GameObject.Find(GeneralStrDefiner.SceneManagerGO_name).
                        GetComponent<MySceneManager>().to_start_scene();
    }

    public void start_game()
    {
        if(start_flag)
        {
            toggle_camera();
            MD_GC_Animator.SetTrigger(MD_StrDefiner.AnimatorStartTrigger_str);
            Aim_script.state_one_flag = false;
        }
        
    }

    public void ToInit()
    {
        update_cities();
        total_cities = alive_cities;
        log_VR();

        toggle_camera();
        start_flag = true;
        WaveText_TRANS.GetComponent<MeshRenderer>().enabled = false;

        ammo_this_wave = 1;
    }

    public void restart()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void state_one_Iexplosion()
    {
        //instantiate_explode(AimT_TRANS.position,SmallExplodeScale);
    }

    public void ToStart()
    {
        //WorldCanvas_GO.SetActive(false);
        MD_GC_Animator.SetTrigger(MD_StrDefiner.AnimatorNextStepTrigger_str);
        //TutorialTest_TRANS.GetComponent<MeshRenderer>().enabled = false;
    }

    public void ToStartWave()
    {
        wave_number++;
        if(!MDDC_script.InfiniteWaves)
        {
            if (wave_number < wave_info.WaveNumber)
            {
                wave_inter_flag = true;
                missile_refill_wave_info(wave_number);
                show_interwave_text();
            }
            else
            {
                MD_GC_Animator.SetTrigger(MD_StrDefiner.AnimatorFinishTrigger_str);
            }
        }
        else
        {
            wave_inter_flag = true;
            missile_refill_infinity();
            show_interwave_text();
        }
        ammo_refill();
        resummon_city();
    }

    private void missile_refill_wave_info(int wave_number)
    {
        missile_number_this_wave = wave_info.WaveInfoList[wave_number];
        current_MTI_number = missile_number_this_wave;
    }

    private void missile_refill_infinity()
    {
        if (!city_destroyed_this_wave)
        {
            increase_difficulty();
        }
        else
        {
            decrease_difficulty();
        }
        current_MTI_number = missile_number_this_wave;
    }

    private void show_interwave_text()
    {
        WaveText_TRANS.GetComponent<TextMesh>().text =
                        MD_StrDefiner.WaveTextPreStr + (wave_number + 1);
        WaveText_TRANS.GetComponent<MeshRenderer>().enabled = true;
        show_score_text();
    }

    private void ammo_refill()
    {
        if(MDDC_script.UsingAutoAmmoNumber)
        {
            ammo_this_wave = missile_number_this_wave + MDDC_script.AmmoOffSet;
        }
        else
        {
            ammo_this_wave = MDDC_script.AmmoConstant;
        }
        AS_script.set_ammo(ammo_this_wave);
        ammo_changed_flag = true;
    }

    private void increase_difficulty()
    {
        difficulty_ratio++;
        apply_difficulty_ratio();
    }

    private void decrease_difficulty()
    {
        difficulty_ratio--;
        difficulty_ratio = difficulty_ratio <= 0 ? 0 : difficulty_ratio;
        apply_difficulty_ratio();
        city_destroyed_this_wave = false;
    }

    private void apply_difficulty_ratio()
    {
        missile_Itime_this_wave = MDDC_script.MissileInterTime *
                            (MDDC_script.MRDifficultyIncrease * difficulty_ratio + 1);
        missile_timer = missile_Itime_this_wave;
        missile_speed_this_wave = MDDC_script.MissileSpeed *
                                    (MDDC_script.MSDifficultyIncrease * difficulty_ratio + 1);
        missile_number_this_wave = (int)(MDDC_script.MissileNumberEachWave *
                                    (MDDC_script.MRDifficultyIncrease * difficulty_ratio + 1));
    }

    private void show_score_text()
    {
        ScoreText_TRANS.GetComponent<MeshRenderer>().enabled = true;
    }

    private void hide_score_text()
    {
        ScoreText_TRANS.GetComponent<MeshRenderer>().enabled = false;
    }

    private void resummon_city()
    {
        if (ReSummonCity)
        {

            foreach (GameObject city in total_cities)
            {
                city.GetComponent<City>().resummon();
                update_cities();
            }
        }
    }

    public void StartWave()
    {
        if(wave_inter_timer < 0.0f)
        {
            wave_inter_timer = WaveInterTime;
            wave_inter_flag = false;
            hide_interwave_text();
            MD_GC_Animator.SetTrigger(MD_StrDefiner.AnimatorNextStepTrigger_str);
        }
    }

    private void hide_interwave_text()
    {
        WaveText_TRANS.GetComponent<MeshRenderer>().enabled = false;
        hide_score_text();
    }

    public void check_missile_number()
    {
        StartCoroutine(delayed_check_missile_number(0.1f));
    }

    public IEnumerator delayed_check_missile_number(float time)
    {
        missile_number_checked = false;
        yield return new WaitForSeconds(time);
        current_alive_missile_number =
                GameObject.FindGameObjectsWithTag(MD_StrDefiner.Enemy_tag).Length;
        missile_number_checked = true;
    }

    public void WaitMissileClear()
    {
        if (current_alive_missile_number == 0)
        {
            MD_GC_Animator.SetTrigger(MD_StrDefiner.AnimatorNextStepTrigger_str);
        }
    }

    public void City_hitted()
    {
        //StartCoroutine(check_missile_number());
        if(UsingPunishSystem)
        {
            score -= GetHitPunish;
            score_changed_flag = true;
        }
    }

    private void update_ammo_text()
    {
        if(ammo_changed_flag)
        {
            ammo_changed_flag = false;
            AmmoText_TRANS.GetComponent<TextMeshPro>().text =
                        MD_StrDefiner.AmmoTextPreStr + AS_script.Current_ammo;
        }
    }

    public bool ammo_spend()
    {
        if(!Menu_gazing_flag && !Reload_gazing_flag)
        {
            if (AS_script.ammo_spend())
            {
                ammo_changed_flag = true;
                return true;
            }
            else
            {
                if (!OOAText_TRANS.GetComponent<MeshRenderer>().enabled)
                {
                    StartCoroutine(outofammo_warning());
                }
            }
        }
        return false;
    }

    private IEnumerator outofammo_warning()
    {
        OOAText_TRANS.GetComponent<MeshRenderer>().enabled = true;
        yield return new WaitForSeconds(1.0f);
        OOAText_TRANS.GetComponent<MeshRenderer>().enabled = false;
    }

    public void add_bonus_score(int bonus_score)
    {
        score += bonus_score;
        score_changed_flag = true;
    }

    public void ToFinish()
    {
        WaveText_TRANS.GetComponent<TextMesh>().text = "Congrats! You finish the game!";
        WaveText_TRANS.GetComponent<MeshRenderer>().enabled = true;
        ScoreText_TRANS.GetComponent<MeshRenderer>().enabled = true;
    }

    public void pause_game()
    {
        //GeneralGameController.GameTimeScale = 0.0f;
        Time.timeScale = 0.0f;
    }

    public void resume_game()
    {
        Time.timeScale = 1.0f;
    }

    private void set_random()
    {
        if(MDDC_script.UsingRandomSeed)
        {
            Random.InitState(MDDC_script.RandomSeed);
        }
    }

    public void ToGameOver()
    {
        WaveText_TRANS.GetComponent<TextMesh>().text = MD_StrDefiner.GameOverStr;
        WaveText_TRANS.GetComponent<MeshRenderer>().enabled = true;
        ScoreText_TRANS.GetComponent<MeshRenderer>().enabled = true;
    }

    public void reload_gazing(Transform reload_coll_TRANS)
    {
        reload_collider_TRANS = reload_coll_TRANS;
        reload_collider_TRANS.GetComponentInParent<ReloadGroup>().activate();
    }

    public void reload_ammo()
    {
        if(MDDC_script.UsingReloadSystem && Reload_gazing_flag)
        {
            if (MDDC_script.UsingReloadAutoNumber)
            {
                AS_script.set_ammo(ammo_this_wave + MDDC_script.ReloadAmmoOffset);
            }
            else
            {
                AS_script.set_ammo(MDDC_script.ReloadAmmoNumber);
            }
            ammo_changed_flag = true;

            if(reload_collider_TRANS != null)
            {
                reload_collider_TRANS.GetComponentInParent<ReloadGroup>().reload_action();
            }
        }
    }

    public void start_tutorial()
    {

    }
}
