using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MD_GameController : MonoBehaviour {

    private const string score_init_str = "Score: ";

    public MD_TargetRayCast MDTRC_script;
    public GameObject ExplodePrefab;
    public GameObject MissilePrefab;
    public GameObject ScoreText_OBJ;

    public bool City_destroied { get; set; }

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

    // Use this for initialization
    void Start () {
        this.City_destroied = false;
        this.city_number = 0;
        this.MD_GC_Animator = GetComponent<Animator>();
        this.missile_timer_flag = false;
        this.missile_timer = MissileInterTime;
        this.score = 0;
        this.score_changed_flag = true;

        update_cities();
    }
	
	// Update is called once per frame
	void Update () {
        if (missile_timer_flag)
        {
            missile_timer -= Time.deltaTime;
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
                    Instantiate(MissilePrefab, RandomPosGenerate(), Quaternion.identity);
        Missile Missile_Script = Missile.GetComponent<Missile>();
        int city_index = Random.Range(0, city_number);
        Missile_Script.set_target(cities[city_index].transform);
        Missile_Script.start_move();
        MD_GC_Animator.SetTrigger("NextStep");
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
            MD_GC_Animator.SetTrigger("NextStep");
        }
    }

    public void ExitWaitForNextMissile()
    {
        missile_timer_flag = false;
    }

    public void update_cities()
    {
        cities = GameObject.FindGameObjectsWithTag("City");
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
}
