using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FS_FruitSpeedCal2 : MonoBehaviour {

    //[SerializeField] private FS_CheckRayHit FSCRH_script;
    [SerializeField] private FS_Fruit FSF_script;

    private bool last_aimed_flag;
    private bool speed_cal_flag;
    private float slice_speed;

    // Use this for initialization
    void Awake () {
        this.last_aimed_flag = false;
        this.speed_cal_flag = false;
        this.slice_speed = FSF_script.FSRC.DC_script.GameSetting.SliceSpeed;
    }
	
	// Update is called once per frame
	void Update () {

        check_start();

        //Debug.Log("speed_cal_flag " + speed_cal_flag);

        check_speed2();

	}

    private void check_start()
    {
        if (FSF_script.Aim_changed)
        {
            if (!FSF_script.Is_aimed_flag)  //Exiting;
            {
                speed_cal_flag = true;
            }
        }
    }

    //Check the speed after exit until stop.
    private void check_speed2()
    {
        if(speed_cal_flag)
        {
            if (Mathf.Abs(GeneralMethods.getVRspeed().y) < slice_speed)
            {
                FSF_script.fruit_cutted();
                speed_cal_flag = false;
            }
        }

    }
}
