using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BP_InputManager : MonoBehaviour {

    //public BP_GameController BPGC_script;

    public bool Key_pressed { get; set; }

	// Use this for initialization
	void Start () {
        this.Key_pressed = false;
	}
	
	// Update is called once per frame
	void Update () {
        Key_pressed = false;
        if(Input.GetKeyDown(KeyCode.JoystickButton0))
        {
            Key_pressed = true;
        }

	}
}
