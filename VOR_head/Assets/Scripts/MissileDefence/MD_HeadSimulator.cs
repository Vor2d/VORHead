using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MD_HeadSimulator : MonoBehaviour {

    [SerializeField] private Transform Camera_TRANS;
    [SerializeField] private AmmoSystem AS_script;
    [SerializeField] private Transform HeadIndicator1_TRANS;
    [SerializeField] private Transform HeadIndicator2_TRANS;
    [SerializeField] private MD_GameController MDGC_script;

    private bool head_indicator1_on;

    // Use this for initialization
    void Start () {
        this.head_indicator1_on = false;
    }

    // Update is called once per frame
    void Update () {
        transform.position = Camera_TRANS.position;
        if (MDGC_script.MDDC_script.using_VR)
        {
            transform.localRotation = GeneralMethods.getVRrotation();
        }
        toggle_head_indicator();
    }

    private void toggle_head_indicator()
    {
        if(MDGC_script.MDDC_script.UsingDualHeadIndicator)
        {
            if(!head_indicator1_on && (AS_script.Current_ammo <= 0))
            {
                switch_to_HI1();
            }
            else if(head_indicator1_on && (AS_script.Current_ammo > 0))
            {
                switch_to_HI2();
            }
        }
    }

    public void switch_to_HI1()
    {
        head_indicator1_on = true;
        HeadIndicator1_TRANS.GetComponent<MeshRenderer>().enabled = true;
        HeadIndicator2_TRANS.GetComponent<MeshRenderer>().enabled = false;
    }

    public void switch_to_HI2()
    {
        head_indicator1_on = false;
        HeadIndicator1_TRANS.GetComponent<MeshRenderer>().enabled = false;
        HeadIndicator2_TRANS.GetComponent<MeshRenderer>().enabled = true;
    }

}
