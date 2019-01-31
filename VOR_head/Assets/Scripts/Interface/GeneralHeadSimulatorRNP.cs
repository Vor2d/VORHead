using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralHeadSimulatorRNP : MonoBehaviour {

    [SerializeField] private string DC_name;
    [SerializeField] private Transform Camera_TRANS;

    private ParentDataController PDC_script;

    // Use this for initialization
    void Start()
    {
        try
        {
            this.PDC_script =
                GameObject.Find(DC_name).GetComponent<ParentDataController>();
        }
        catch { Debug.Log("Can not find object! " + DC_name); }

    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Camera_TRANS.position;
        if (PDC_script != null && PDC_script.using_VR)
        {
            transform.rotation = GeneralMethods.getVRrotation();
        }
    }

}
