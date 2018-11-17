using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Indicator : MonoBehaviour {

	// Use this for initialization
	void Start () {

#if UNITY_STANDALONE
        GetComponent<MeshRenderer>().enabled = false;
#endif

#if UNITY_EDITOR
        GetComponent<MeshRenderer>().enabled = false;
#endif

    }

    // Update is called once per frame
    void Update () {
		
	}
}
