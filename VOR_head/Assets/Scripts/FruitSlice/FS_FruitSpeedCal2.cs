using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FS_FruitSpeedCal2 : MonoBehaviour {

    //[SerializeField] private FS_CheckRayHit FSCRH_script;
    [SerializeField] private FS_Fruit FSF_script;

    private bool last_aimed_flag;

	// Use this for initialization
	void Start () {
        this.last_aimed_flag = false;
	}
	
	// Update is called once per frame
	void Update () {
		
        if(FSF_script.start_flag)
        {
            
        }

	}

    //Check the speed after exit, see if it is valid;
    private void check_speed2()
    {
        if(FSF_script.last_is_aimed_flag && !FSF_script.Is_aimed_flag)  //Exiting;
        {
            if (Mathf.Abs(GeneralMethods.getVRspeed().y) < FSF_script.FSGC_script.SliceSpeed)
            {

            }
        }
    }
}
