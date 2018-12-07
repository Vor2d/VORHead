using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FS_FruitSpeedCal2 : MonoBehaviour {

    //[SerializeField] private FS_CheckRayHit FSCRH_script;
    [SerializeField] private FS_Fruit FSF_script;

    private bool last_aimed_flag;
    private bool speed_cal_flag;

	// Use this for initialization
	void Start () {
        this.last_aimed_flag = false;
        this.speed_cal_flag = false;
	}
	
	// Update is called once per frame
	void Update () {

        if (FSF_script.Aim_changed)
        {
            if (!FSF_script.Is_aimed_flag && FSF_script.CI_script.Index_trigger)  //Exiting;
            {
                speed_cal_flag = true;
            }
        }

        Debug.Log("speed_cal_flag " + speed_cal_flag);

        check_speed2();

	}

    //Check the speed after exit until stop.
    private void check_speed2()
    {
        if(speed_cal_flag)
        {
            if (Mathf.Abs(GeneralMethods.getVRspeed().y) < FSF_script.FSGC_script.SliceSpeed)
            {
                FSF_script.fruit_cutted();
                speed_cal_flag = false;
            }
        }

    }
}
