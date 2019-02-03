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
    //[SerializeField] private GameObject WorldCanvas_GO;
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
    //[SerializeField] private Transform ExplodeOutline_TRANS;

    [Header("Variables")]
    public float InstRandomRange = 9.0f;
    public float InstY = 4.0f;
    public float InstZ = 10.0f;
    public float MissileInterTime = 3.0f;
    public float MissileSpeed = 0.2f;
    public int ScoreIncerase = 10;
    public float ExplodeRaduis = 3.0f;
    public float ExplodeTime = 1.5f;
    public bool UsingExplodeOutline = false;
    [SerializeField] private float SmallExplodeScale = 0.1f;
    [SerializeField] private float WaveInterTime = 2.0f;
    [SerializeField] private int AmmoOffSet = 0;
    [SerializeField] private bool ScoreNotNegative = true;
    [SerializeField] private bool UsingRandomSeed = false;
    [SerializeField] private int RandomSeed = 0;
    [SerializeField] private bool ReadDataFromFile = false;
    [SerializeField] private bool AutoLogVR = false;
    [SerializeField] private bool ReSummonCity = false;
    [Header("BonusSystem")]
    public bool UsingBonusSystem = false;
    public int[] BonusScores;
    [Header("PunishSystem")]
    public bool UsingPunishSystem = false;
    public int GetHitPunish = 0;
    public int GetDestroyPunish = 0;
    [Header("InfiniteWaves")]
    [SerializeField] private bool InfiniteWaves = false;
    [SerializeField] private float MSDifficultyIncrease = 0.1f;
    [SerializeField] private float MRDifficultyIncrease = 0.1f;
    [SerializeField] private int MissileNumberEachWave;


    public bool City_destroied { get; set; }
    public bool Menu_gazing_flag { get; set; }

    private MD_DataController MDDC_script;
    private GameObject[] alive_cities;
    private GameObject[] total_cities;
    private float missile_timer;
    private bool missile_timer_flag;
    private Animator MD_GC_Animator;
    private int city_number;
    private int score;
    private bool score_changed_flag;
    private bool first_camera_on;
    private bool start_flag;
    private int wave_number;
    private MD_WaveInfo wave_info;
    private int current_MTI_number; //missile to instantiate;
    private float wave_inter_timer;
    private bool wave_inter_flag;
    private int current_missile_number;
    private bool ammo_changed_flag;
    private bool city_destroyed_this_wave;

    // Use this for initialization
    void Start () {
        if(MDDC_script == null)
        {
            MDDC_script = GameObject.Find(MD_StrDefiner.DataController_name).
                                            GetComponent<MD_DataController>();
        }

        load_game_data();

        this.City_destroied = false;
        this.city_number = 0;
        this.MD_GC_Animator = GetComponent<Animator>();
        this.missile_timer_flag = false;
        this.missile_timer = MissileInterTime;
        this.score = 0;
        this.score_changed_flag = true;
        this.first_camera_on = false;
        this.start_flag = false;
        this.wave_number = -1;
        this.wave_info = new MD_WaveInfo();
        this.current_MTI_number = -1;
        this.wave_inter_timer = WaveInterTime;
        this.wave_inter_flag = false;
        this.current_missile_number = -1;
        this.ammo_changed_flag = true;
        this.Menu_gazing_flag = false;
        this.city_destroyed_this_wave = false;

        wave_info.set_data(MD_WaveDefiner.WaveInfo_list);
        update_cities();
        this.total_cities = alive_cities;
        set_random();
        log_VR();
        register_controller();
    }
	
	// Update is called once per frame
	protected override void Update ()
    {
        base.Update();

        if(missile_timer_flag)
        {
            missile_timer -= GeneralGameController.GameDeltaTime;
        }
        if(wave_inter_flag)
        {
            wave_inter_timer -= GeneralGameController.GameDeltaTime;
        }

        //if(Input.GetKeyDown(KeyCode.JoystickButton1))
        //{
        //    restart();
        //}

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
    }

    private void de_register_controller()
    {
        CI_script.Button_B -= recenter;
    }

    private void recenter()
    {
        UnityEngine.XR.InputTracking.Recenter();
    }

    private void load_game_data()
    {
        if(ReadDataFromFile)
        {
            try
            {
                MissileInterTime = 
                        float.Parse(MDDC_script.GameData_Dict[MD_GameData.MissileRate]);
            }
            catch { }
            try
            {
                MissileSpeed =
                        float.Parse(MDDC_script.GameData_Dict[MD_GameData.MissileSpeed]);
            }
            catch { }
            try
            {
                AmmoOffSet =
                        int.Parse(MDDC_script.GameData_Dict[MD_GameData.AmmoOffset]);
            }
            catch { }
            try
            {
                MSDifficultyIncrease =
                    float.Parse(MDDC_script.GameData_Dict[MD_GameData.MSDifficultyIncrease]);
            }
            catch { }
            try
            {
                MRDifficultyIncrease =
                    float.Parse(MDDC_script.GameData_Dict[MD_GameData.MRDifficultyIncrease]);
            }
            catch { }
            try
            {
                MissileNumberEachWave =
                        int.Parse(MDDC_script.GameData_Dict[MD_GameData.MissileNumberEachWave]);
            }
            catch { }
        }
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
        if(city_number > 0)
        {
            GameObject Missile_OBJ =
            Instantiate(MissilePrefabs[Random.Range(0, MissilePrefabs.Length)],
                        RandomPosGenerate(), Quaternion.identity);
            Missile Missile_Script = Missile_OBJ.GetComponent<Missile>();
            int city_index = Random.Range(0, city_number);
            Missile_Script.set_target(alive_cities[city_index].transform);
            Missile_Script.start_move(MissileSpeed);
        }
    }

    private Vector3 RandomPosGenerate()
    {
        return new Vector3(Random.Range(-InstRandomRange, InstRandomRange),
                            InstY, InstZ);
    }

    public void ToWaitForNextMissile()
    {
        missile_timer_flag = true;
    }

    public void WaitForNextMissile()
    {
        if(missile_timer <= 0.0f)
        {
            missile_timer_flag = false;
            missile_timer = MissileInterTime;
            MD_GC_Animator.SetTrigger(MD_StrDefiner.AnimatorNextStepTrigger_str);
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
        city_number = alive_cities.Length;
        //Debug.Log("city_number " + city_number);
        check_game_over();
    }

    private void check_game_over()
    {
        if (city_number <= 0)
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
        explode.GetComponent<ExplodeGroup>().
                        start_explode_group(ExplodeTime,ExplodeRaduis,UsingBonusSystem,
                                            UsingExplodeOutline);
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
        //WorldCanvas_GO.SetActive(true);
        toggle_camera();
        start_flag = true;
        //Aim_script.state_one_flag = true;
        WaveText_TRANS.GetComponent<MeshRenderer>().enabled = false;
        if(UsingExplodeOutline)
        {
            //ExplodeOutline_TRANS.GetComponent<MeshRenderer>().enabled = true;
        }
        //TutorialTest_TRANS.GetComponent<MeshRenderer>().enabled = true;
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
        if(!InfiniteWaves)
        {
            if (wave_number < wave_info.WaveNumber)
            {
                current_MTI_number = wave_info.WaveInfoList[wave_number];
                //MD_GC_Animator.SetTrigger(MD_StrDefiner.AnimatorNextStepTrigger_str);
                WaveText_TRANS.GetComponent<TextMesh>().text =
                        MD_StrDefiner.WaveTextPreStr + (wave_number + 1);
                WaveText_TRANS.GetComponent<MeshRenderer>().enabled = true;
                wave_inter_flag = true;
                AS_script.set_ammo(current_MTI_number + AmmoOffSet);
                ammo_changed_flag = true;

                show_score_text();
            }
            else
            {
                MD_GC_Animator.SetTrigger(MD_StrDefiner.AnimatorFinishTrigger_str);
            }
        }
        else
        {
            if (!city_destroyed_this_wave)
            {
                increase_difficulty();
            }
            else
            {
                decrease_difficulty();
            }
            current_MTI_number = MissileNumberEachWave;
            WaveText_TRANS.GetComponent<TextMesh>().text =
                        MD_StrDefiner.WaveTextPreStr + (wave_number + 1);
            WaveText_TRANS.GetComponent<MeshRenderer>().enabled = true;
            wave_inter_flag = true;
            AS_script.set_ammo(current_MTI_number + AmmoOffSet);
            ammo_changed_flag = true;
            show_score_text();
        }

        if(ReSummonCity)
        {
            
            foreach(GameObject city in total_cities)
            {
                //Debug.Log("already_destroied " + city.name + city.GetComponent<City>().already_destroied);
                city.GetComponent<City>().resummon();
                update_cities();
            }
        }

    }

    private void increase_difficulty()
    {
        MissileInterTime -= MissileInterTime * MRDifficultyIncrease;
        missile_timer = MissileInterTime;
        MissileSpeed += MissileSpeed * MSDifficultyIncrease;
        MissileNumberEachWave += (int)(MissileNumberEachWave * MRDifficultyIncrease);
        //Debug.Log("MissileInterTime " + MissileInterTime);
        //Debug.Log("MissileSpeed " + MissileSpeed);
    }

    private void decrease_difficulty()
    {
        MissileInterTime += MissileInterTime * MRDifficultyIncrease;
        missile_timer = MissileInterTime;
        MissileSpeed -= MissileSpeed * MSDifficultyIncrease;
        MissileNumberEachWave -= (int)(MissileNumberEachWave * MSDifficultyIncrease);
        city_destroyed_this_wave = false;
    }

    private void show_score_text()
    {
        ScoreText_TRANS.GetComponent<MeshRenderer>().enabled = true;
    }

    private void hide_score_text()
    {
        ScoreText_TRANS.GetComponent<MeshRenderer>().enabled = false;
    }

    public void StartWave()
    {
        if(wave_inter_timer < 0.0f)
        {
            wave_inter_timer = WaveInterTime;
            wave_inter_flag = false;
            WaveText_TRANS.GetComponent<MeshRenderer>().enabled = false;
            MD_GC_Animator.SetTrigger(MD_StrDefiner.AnimatorNextStepTrigger_str);

            hide_score_text();
        }
    }



    public void check_missile_number()
    {
        StartCoroutine(delayed_check_missile_number(0.1f));
    }

    public IEnumerator delayed_check_missile_number(float time)
    {
        yield return new WaitForSeconds(time);
        current_missile_number =
                GameObject.FindGameObjectsWithTag(MD_StrDefiner.Missile_tag).Length;
    }

    public void WaitMissileClear()
    {
        if (current_missile_number == 0)
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
        if(!Menu_gazing_flag)
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
        if(UsingRandomSeed)
        {
            Random.InitState(RandomSeed);
        }
    }

    public string var_to_string()
    {
        string result_str = "";

        result_str += "Missile Rate " + MissileInterTime.ToString("F2") + " ";
        result_str += "Missile Speed " + MissileSpeed.ToString("F2") + " ";

        return result_str;
    }

    public void ToGameOver()
    {
        WaveText_TRANS.GetComponent<TextMesh>().text = MD_StrDefiner.GameOverStr;
        WaveText_TRANS.GetComponent<MeshRenderer>().enabled = true;
        ScoreText_TRANS.GetComponent<MeshRenderer>().enabled = true;
    }
}
