using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FS_FruitSpeedCal2 : MonoBehaviour {

    [SerializeField] private FS_Fruit F_script;

    private bool speed_cal_flag;
    private float slice_speed;

    // Use this for initialization
    void Awake () {
        this.speed_cal_flag = false;
        this.slice_speed = 0.0f;
    }

    private void Start()
    {
        slice_speed = F_script.FSRC.DC_script.GameSetting.SliceSpeed;
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
        if (Mathf.Abs(GeneralMethods.getVRspeed().y) < slice_speed)
        {
            F_script.fruit_cutted();
            speed_cal_flag = false;
         }
    }

    public void start_speed_cal()
    {
        speed_cal_flag = true;
    }
}
