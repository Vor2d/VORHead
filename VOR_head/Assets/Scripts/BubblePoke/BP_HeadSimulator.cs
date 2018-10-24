using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BP_HeadSimulator : MonoBehaviour {

    private BP_DataController BPDC_script;

    // Use this for initialization
    void Start()
    {
        this.BPDC_script =
            GameObject.Find("BP_DataController").GetComponent<BP_DataController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (BPDC_script.Using_VR_flag)
        {
            transform.rotation = GeneralMethods.getVRrotation();
        }
    }
}
