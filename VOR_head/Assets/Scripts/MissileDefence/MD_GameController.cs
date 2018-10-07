using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MD_GameController : MonoBehaviour {

    public bool City_destroied { get; set; }

    public float InstRandomRange = 9.0f;
    public float InstY = 4.0f;
    public float InstZ = 10.0f;
    public float MissileInterTime = 3.0f;

    public GameObject MissilePrefab;

    private GameObject[] cities;
    private float missile_timer;
    private bool missile_timer_flag;
    private Animator MD_GC_Animator;
    private int city_number;

    // Use this for initialization
    void Start () {
        this.City_destroied = false;
        this.city_number = 0;
        this.MD_GC_Animator = GetComponent<Animator>();
        this.missile_timer_flag = false;
        this.missile_timer = MissileInterTime;

        update_cities();
    }
	
	// Update is called once per frame
	void Update () {
        if (missile_timer_flag)
        {
            missile_timer -= Time.deltaTime;
        }
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
}
