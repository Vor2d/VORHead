using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BO_Pad : MonoBehaviour {

    [SerializeField] private BO_CheckRayHit BOCRH_script;

    //public bool move_with_raycast { get; set; }

	// Use this for initialization
	void Start () {
        //this.move_with_raycast = false;
	}
	
	// Update is called once per frame
	void Update () {
        if (BOCRH_script.Hit_position.z != 0.0f && !BO_GameController.IS.Game_paused)
        {
            transform.position = BOCRH_script.Hit_position;
        }
	}

}
