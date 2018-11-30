using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aim : MonoBehaviour {

    public MD_GameController MDGC_script;

    public bool state_one_flag { get; set; }

	// Use this for initialization
	void Start () {
        this.state_one_flag = true;
	}
	
	// Update is called once per frame
	void Update () {

        //if(OVRInput.Get(OVRInput.Button.One))
        //{
        //    Debug.Log("OVRInput.Get(OVRInput.Button.One)");
        //    MDGC_script.IE_with_raycast();
        //}

        if(Input.GetKeyDown(KeyCode.JoystickButton0))
        {
            if(state_one_flag)
            {
                MDGC_script.state_one_Iexplosion();
            }
            else
            {
                MDGC_script.IE_with_raycast();
            }
        }
    }
}
