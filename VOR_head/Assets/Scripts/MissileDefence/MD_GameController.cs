using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SpatialTracking;
using UnityEngine.SceneManagement;

public class MD_GameController : MonoBehaviour {

    public const string score_init_str = "Score: ";
    public const string nextS_trigger_str = "NextStep";
    public const string start_trigger_str = "Start";
    //public const string rightCG_Oname_str = "RightControllerGroup";  //Right controller Group object name;
    //public const string rightC_Oname_str = "RightController";    //Right controller object name;
    public const string city_tag = "City";
    public const string sceneM_Oname_str = "SceneManager";  //SceneManager object name;

    [SerializeField] private Transform Camera1I_TRANS;
    [SerializeField] private Transform Camera2I_TRANS;
    [SerializeField] private Transform CameraParent_TRANS;
    [SerializeField] private Transform Body_TRANS;
    [SerializeField] private Aim Aim_script;
    [SerializeField] private RightController RC_script;
    [SerializeField] private Transform AimT_TRANS;


    public MD_TargetRayCast MDTRC_script;
    public GameObject ExplodePrefab;
    public GameObject[] MissilePrefabs;
    public GameObject ScoreText_OBJ;

    public bool City_destroied { get; set; }

    [SerializeField] private float SmallExplodeScale = 0.1f;
    public float InstRandomRange = 9.0f;
    public float InstY = 4.0f;
    public float InstZ = 10.0f;
    public float MissileInterTime = 3.0f;
    public int ScoreIncerase = 10;

    private GameObject[] cities;
    private float missile_timer;
    private bool missile_timer_flag;
    private Animator MD_GC_Animator;
    private int city_number;
    private int score;
    private bool score_changed_flag;
    private bool first_camera_on;
    private bool start_flag;

    // Use this for initialization
    void Start () {
        this.City_destroied = false;
        this.city_number = 0;
        this.MD_GC_Animator = GetComponent<Animator>();
        this.missile_timer_flag = false;
        this.missile_timer = MissileInterTime;
        this.score = 0;
        this.score_changed_flag = true;
        this.first_camera_on = false;
        this.start_flag = false;

        

        update_cities();
    }
	
	// Update is called once per frame
	void Update () {
        if (missile_timer_flag)
        {
            missile_timer -= Time.deltaTime;
        }

        if(Input.GetKeyDown(KeyCode.JoystickButton1))
        {
            restart();
        }

        update_score();
    }

    private void LateUpdate()
    {
        if(City_destroied)
        {
            update_cities();
            City_destroied = false;
        }
    }

    public void ToInstantiatetMissile()
    {
        GameObject Missile =
                    Instantiate(MissilePrefabs[Random.Range(0,MissilePrefabs.Length)],
                                RandomPosGenerate(), Quaternion.identity);
        Missile Missile_Script = Missile.GetComponent<Missile>();
        int city_index = Random.Range(0, city_number);
        Missile_Script.set_target(cities[city_index].transform);
        Missile_Script.start_move();
        MD_GC_Animator.SetTrigger(nextS_trigger_str);
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
            MD_GC_Animator.SetTrigger(nextS_trigger_str);
        }
    }

    public void ExitWaitForNextMissile()
    {
        missile_timer_flag = false;
    }

    public void update_cities()
    {
        List<GameObject> temp_cities = new List<GameObject>();
        foreach(GameObject city in GameObject.FindGameObjectsWithTag(city_tag))
        {
            if(city.GetComponent<City>().Health > 0)
            {
                temp_cities.Add(city);
            }
        }

        cities = temp_cities.ToArray();
        city_number = cities.Length;
        Debug.Log("city_number " + city_number);
    }

    public void IE_with_raycast()
    {
        instantiate_explode(MDTRC_script.Hit_position);
    }

    private void instantiate_explode(Vector3 target_pos)
    {
        GameObject explode =
                    Instantiate(ExplodePrefab, target_pos, Quaternion.identity);
        explode.GetComponent<Explode>().start_exp();
    }

    private void instantiate_explode(Vector3 target_pos,float scale)
    {
        GameObject explode =
                    Instantiate(ExplodePrefab, target_pos, Quaternion.identity);
        explode.GetComponent<Explode>().set_radius_scale(scale);
        explode.GetComponent<Explode>().start_exp();
    }

    public void missile_destroyed()
    {
        score += ScoreIncerase;
        score_changed_flag = true;
    }

    private void update_score()
    {
        if(score_changed_flag)
        {
            ScoreText_OBJ.GetComponent<TextMesh>().text = score_init_str + score.ToString();
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

    public void back_button()
    {
        GameObject.Find(sceneM_Oname_str).GetComponent<MySceneManager>().to_start_scene();
    }

    public void start_button()
    {
        if(start_flag)
        {
            toggle_camera();
            MD_GC_Animator.SetTrigger(start_trigger_str);
            Aim_script.state_one_flag = false;
        }
        
    }

    public void ToInit()
    {
        toggle_camera();
        start_flag = true;
        Aim_script.state_one_flag = true;
    }

    private void restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void state_one_Iexplosion()
    {
        instantiate_explode(AimT_TRANS.position,SmallExplodeScale);
    }


}
