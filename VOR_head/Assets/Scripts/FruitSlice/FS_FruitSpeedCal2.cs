using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FS_FruitSpeedCal2 : MonoBehaviour 
{
    [SerializeField] private bool UsingSpeedThreshold;
    private bool speed_cal_flag;
    private float slice_speed;
    private float speed_threshold;
    private bool speed_reached;

    // Use this for initialization
    void Awake () {
        this.speed_cal_flag = false;
        this.slice_speed = 0.0f;
        this.speed_threshold = 0.0f;
        this.speed_reached = false;
    }

    private void Start()
    {
        slice_speed = FS_Setting.IS.SliceSpeed;
        speed_threshold = FS_Setting.IS.SpeedThreshold;
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
        float speed = Mathf.Abs(GeneralMethods.getVRspeed().magnitude);
        if (!speed_reached && speed >= speed_threshold)
        { speed_reached = true; }
        if (speed < slice_speed)
        {
            if (UsingSpeedThreshold && !speed_reached) { FS_Fruit.IS.cut_too_slow(); }
            else { FS_Fruit.IS.pre_cut_once(); }
            FS_Fruit.IS.stop_record_CP();
            speed_cal_flag = false;
            speed_reached = false;
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
