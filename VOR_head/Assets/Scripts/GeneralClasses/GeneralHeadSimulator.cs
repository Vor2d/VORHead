using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralHeadSimulator : MonoBehaviour {

    [SerializeField] private string DC_name;

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
        HS_rotate();
    }

    private void HS_rotate()
    {
        if (PDC_script.using_VR)
        {
            transform.rotation = GeneralMethods.getVRrotation();
        }
    }

}
