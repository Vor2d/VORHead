using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FS_Fruit : MonoBehaviour {

    public bool Sliced_flag { get; set; }
    public bool Start_flag { get; set; }
    public bool Last_is_aimed_flag { get; set; }
    public bool Is_aimed_flag { get; set; }
    public bool Aim_changed { get; set; }

    public FS_CheckRayHit FSCRH_script;
    [SerializeField] public FS_RC FSRC;

    public bool speed_cal;

    private bool inner_sliced_flag;

    // Use this for initialization
    void Awake () {
        this.Start_flag = false;
	}

    private void Start()
    {
        this.Is_aimed_flag = false;
        this.Last_is_aimed_flag = false;
        this.speed_cal = false;
        this.Sliced_flag = false;
        this.inner_sliced_flag = false;

        start_fruit();
    }

    // Update is called once per frame
    void Update () {
		
        if(Start_flag)
        {
            //check_speed1();
            check_start_aim();

            if (inner_sliced_flag)
            {
                Sliced_flag = true;
                inner_sliced_flag = false;
            }
            else
            {
                Sliced_flag = false;
            }
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

    public void start_fruit(FS_RC _FSRC)
    {
        Start_flag = true;
        FSRC = _FSRC;
    }

    [Obsolete("Use fruit_cutted")]
    private void fruit_sliced()
    {
        Sliced_flag = true;
        FSRC.GC_script.fruit_destroyed();
        Destroy(gameObject);
    }

    //Calculte speed when enter, must above the threshold at all time;
    //private void check_speed1()
    //{
    //    if (FSRC.GC_script.is_slicing)
    //    {
    //        if (!Last_is_aimed_flag && Is_aimed_flag)    //Enter trigger;
    //        {
    //            speed_cal = true;
    //        }
    //        else if (Last_is_aimed_flag && Is_aimed_flag)    //In trigger;
    //        {
    //            if (Mathf.Abs(GeneralMethods.getVRspeed().y) < FSRC.GC_script.SliceSpeed)
    //            {
    //                speed_cal = false;
    //            }
    //            else
    //            {
    //                speed_cal = true;
    //            }
    //        }
    //        else if (Last_is_aimed_flag && !Is_aimed_flag)   //Exit trigger;
    //        {
    //            if (speed_cal)
    //            {
    //                //fruit_sliced();
    //                fruit_cutted();
    //            }
    //        }
    //    }
    //    else
    //    {
    //        speed_cal = false;
    //    }
    //}

    public void fruit_cutted()
    {
        inner_sliced_flag = true;
    }

}
