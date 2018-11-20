using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BO_Pad : MonoBehaviour {

    [SerializeField] private BO_CheckRayHit BOCRH_script;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if(BOCRH_script.Hit_position.z != 0.0f)
        {
            transform.position = BOCRH_script.Hit_position;
        }
	}
}
