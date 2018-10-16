using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimInticator : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void turn_on_mesh()
    {
        GetComponent<MeshRenderer>().enabled = true;
    }

    public void turn_off_mesh()
    {
        GetComponent<MeshRenderer>().enabled = false;
    }
}
