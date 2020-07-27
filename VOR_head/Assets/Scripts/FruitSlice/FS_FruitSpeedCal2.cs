using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FS_FruitSpeedCal2 : MonoBehaviour 
{
    private bool speed_cal_flag;
    private float slice_speed;

    // Use this for initialization
    void Awake () {
        this.speed_cal_flag = false;
        this.slice_speed = 0.0f;
    }

    private void Start()
    {
        slice_speed = FS_DataController.IS.GameSetting.SliceSpeed;
    }

    // Update is called once per frame
    void Update () {
        if(speed_cal_flag)
        {
            check_speed();
        }
        
	}

    //Check the speed after exit until stop.
    private void check_speed()
    {
        if (Mathf.Abs(GeneralMethods.getVRspeed().magnitude) < slice_speed)
        {
            FS_Fruit.IS.pre_cut_once();
            FS_Fruit.IS.stop_record_CP();
            speed_cal_flag = false;
         }
    }

    public void start_speed_cal()
    {
        speed_cal_flag = true;
    }

    public void stop_speed_cal()
    {
        speed_cal_flag = false;
    }
}
