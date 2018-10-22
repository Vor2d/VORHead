using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TDMD_HeadSimulator : MonoBehaviour {

    TDMD_DataController TDMDDC_script;

    // Use this for initialization
    void Start () {
        this.TDMDDC_script =
            GameObject.Find("TDMD_DataController").GetComponent<TDMD_DataController>();
    }
	
	// Update is called once per frame
	void Update () {
        if (TDMDDC_script.Using_VR_flag)
        {
            transform.rotation = GeneralMethods.getVRrotation();
        }
    }
}
