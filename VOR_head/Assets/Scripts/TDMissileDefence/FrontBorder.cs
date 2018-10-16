using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrontBorder : MonoBehaviour {



	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void get_hit()
    {
        transform.parent.GetComponent<TDMD_Player>().get_hit();
    }
}
