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
        this.PDC_script =
            GameObject.Find(DC_name).GetComponent<ParentDataController>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Camera_TRANS.position;
        if (PDC_script.using_VR)
        {
            transform.rotation = GeneralMethods.getVRrotation();
        }
    }

}
