using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller_Rotate : MonoBehaviour {

    [SerializeField] Controller_Input CI_script;
    [SerializeField] Transform MainBody;

    private bool triggered;

	// Use this for initialization
	void Start () {
        this.triggered = false;
	}
	
	// Update is called once per frame
	void Update () {
		
        if(!triggered)
        {
            if(CI_script.Left_flag)
            {
                MainBody.Rotate(new Vector3(0.0f, -30.0f, 0.0f));
                triggered = true;
            }
            else if (CI_script.Right_flag)
            {
                MainBody.Rotate(new Vector3(0.0f, 30.0f, 0.0f));
                triggered = true;
            }
        }

        if(!CI_script.Left_flag && !CI_script.Right_flag)
        {
            triggered = false;
        }

	}
}
