using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MD_Target : MonoBehaviour {

    public MD_TargetRayCast MDTRC_script;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        transform.position = MDTRC_script.Hit_position;

        transform.forward = (new Vector3(0.0f, 1.0f, 0.0f));

	}
}
