using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BO_Ball : MonoBehaviour {

	// Use this for initialization
	void Start () {

        GetComponent<Rigidbody>().AddForce(Vector3.forward * 1000);

	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
