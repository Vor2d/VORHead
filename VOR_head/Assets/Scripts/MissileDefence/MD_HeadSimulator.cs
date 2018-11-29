using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MD_HeadSimulator : MonoBehaviour {

    [SerializeField] Transform Camera_TRANS;

    MD_DataController MDDC_script;

	// Use this for initialization
	void Start () {
        this.MDDC_script = 
            GameObject.Find("MD_DataController").GetComponent<MD_DataController>();


    }

    // Update is called once per frame
    void Update () {
        transform.position = Camera_TRANS.position;
        if (MDDC_script.using_VR)
        {
            transform.rotation = GeneralMethods.getVRrotation();
        }
    }
}
