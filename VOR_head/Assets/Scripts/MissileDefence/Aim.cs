using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aim : MonoBehaviour {

    public MD_GameController MDGC_script;

	// Use this for initialization
	void Start () {
		
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
            //Debug.Log("Input.GetKeyDown(KeyCode.JoystickButton0)");
            MDGC_script.IE_with_raycast();
        }
    }
}
