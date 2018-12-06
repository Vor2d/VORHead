using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FS_Fruit : MonoBehaviour {

    public bool Is_aimed_flag { get; set; }
    public bool sliced_flag { get; set; }
    public bool start_flag { get; set; }
    public bool last_is_aimed_flag { get; set; }

    public FS_GameController FSGC_script;

    
    public bool speed_cal;

    // Use this for initialization
    void Awake () {
        this.start_flag = false;
        this.FSGC_script =
                    GameObject.Find("FS_GameController").GetComponent<FS_GameController>();
	}

    private void Start()
    {
        this.Is_aimed_flag = false;
        this.last_is_aimed_flag = false;
        this.speed_cal = false;
        this.sliced_flag = false;

        start_bubble();
    }

    // Update is called once per frame
    void Update () {
		
        if(start_flag)
        {
            //check_speed1();

            last_is_aimed_flag = Is_aimed_flag;
        }

	}

    public void start_bubble()
    {
        start_flag = true;
    }

    private void fruit_sliced()
    {
        sliced_flag = true;
        FSGC_script.fruit_destroyed();
        Destroy(gameObject);
    }

    //Calculte speed when enter, must above the threshold at all time;
    private void check_speed1()
    {
        if (FSGC_script.is_slicing)
        {
            if (!last_is_aimed_flag && Is_aimed_flag)    //Enter trigger;
            {
                speed_cal = true;
            }
            else if (last_is_aimed_flag && Is_aimed_flag)    //In trigger;
            {
                if (Mathf.Abs(GeneralMethods.getVRspeed().y) < FSGC_script.SliceSpeed)
                {
                    speed_cal = false;
                }
            }
            else if (last_is_aimed_flag && !Is_aimed_flag)   //Exit trigger;
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

}
