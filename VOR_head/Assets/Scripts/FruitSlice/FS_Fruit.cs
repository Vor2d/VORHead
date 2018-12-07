using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FS_Fruit : MonoBehaviour {

    public bool Sliced_flag { get; set; }
    public bool Start_flag { get; set; }
    public bool Last_is_aimed_flag { get; set; }
    public bool Is_aimed_flag { get; set; }
    public bool Aim_changed { get; set; }

    [SerializeField] private FS_CheckRayHit FSCRH_script;
    [SerializeField] private GameObject StopIndicator_Prefab;
    public FS_GameController FSGC_script;
    public Controller_Input CI_script;

    public bool speed_cal;

    // Use this for initialization
    void Awake () {
        this.Start_flag = false;
        this.FSGC_script =
                    GameObject.Find("FS_GameController").GetComponent<FS_GameController>();
	}

    private void Start()
    {
        this.Is_aimed_flag = false;
        this.Last_is_aimed_flag = false;
        this.speed_cal = false;
        this.Sliced_flag = false;

        start_fruit();
    }

    // Update is called once per frame
    void Update () {
		
        if(Start_flag)
        {
            //check_speed1();
            check_start_aim();
        }

	}

    private void check_start_aim()
    {
        Last_is_aimed_flag = Is_aimed_flag;
        Is_aimed_flag = FSCRH_script.check_ray_to_start();
        if (Is_aimed_flag != Last_is_aimed_flag)
        {
            Aim_changed = true;
        }
        else
        {
            Aim_changed = false;
        }
    }

    public void start_fruit()
    {
        Start_flag = true;
    }

    private void fruit_sliced()
    {
        Sliced_flag = true;
        FSGC_script.fruit_destroyed();
        Destroy(gameObject);
    }

    //Calculte speed when enter, must above the threshold at all time;
    private void check_speed1()
    {
        if (FSGC_script.is_slicing)
        {
            if (!Last_is_aimed_flag && Is_aimed_flag)    //Enter trigger;
            {
                speed_cal = true;
            }
            else if (Last_is_aimed_flag && Is_aimed_flag)    //In trigger;
            {
                if (Mathf.Abs(GeneralMethods.getVRspeed().y) < FSGC_script.SliceSpeed)
                {
                    speed_cal = false;
                }
            }
            else if (Last_is_aimed_flag && !Is_aimed_flag)   //Exit trigger;
            {
                if (speed_cal)
                {
                    fruit_sliced();
                }
            }
        }
        else
        {
            speed_cal = false;
        }
    }

    public void fruit_cutted()
    {
        Vector3 hit_point = FSCRH_script.check_ray_to_plane();
        if(hit_point != Vector3.zero)
        {
            //Debug.Log("hit_point " + hit_point);
            Instantiate(StopIndicator_Prefab, hit_point, new Quaternion());
            Sliced_flag = true;
        }
    }
}
