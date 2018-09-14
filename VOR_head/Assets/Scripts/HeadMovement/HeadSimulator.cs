﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class HeadSimulator : MonoBehaviour {

    //public Quaternion OriginalheadQ { get; set; }
    public Vector3 rrotate_degree {get;set;}
    public CoilData CD_script;

    private DataController DC_script;
    private float player_screen_cm;
    private float screen_width_cm;
    private Quaternion current_headQ;

    // Use this for initialization
    void Start () {
        this.DC_script = GameObject.Find("DataController").GetComponent<DataController>();

        this.player_screen_cm = DC_script.Player_screen_cm;
        this.screen_width_cm = DC_script.Screen_width_cm;
        //this.OriginalheadQ = new Quaternion(0.0f, 0.0f, 0.0f, 1.0f);
        this.rrotate_degree = new Vector3();
    }
	
	// Update is called once per frame
	void Update () {
        Quaternion coil_rotation = CD_script.currentHeadOrientation;

        //coil_rotation = new Quaternion(0.0f, 0.03f, -0.05f, 1.0f);

        current_headQ = coil_rotation * Quaternion.Inverse(DC_script.Head_origin);

        rrotate_degree = CRotaQuatToRRotaDegr(current_headQ);

        rrotate_degree = rrotate_degree * DC_script.Gain;

        //Debug.Log("rrotate_degree " + rrotate_degree);

        transform.localEulerAngles =
            GeneralMethods.RealToVirtual(DC_script.Player_screen_cm, DC_script.Screen_width_cm,
                                            rrotate_degree.x, rrotate_degree.y);
    }

    public void reset_originQ()
    {
        DC_script.Head_origin = CD_script.currentHeadOrientation;
    }

    //Coil rotation Quaternion to Reallife rotation Degree
    private Vector3 CRotaQuatToRRotaDegr(Quaternion coil_rotation)
    {
        

        Quaternion axis_conv_crotation = new Quaternion(coil_rotation.y, -coil_rotation.z,
                                                        coil_rotation.x, coil_rotation.w);

        Vector3 rotate_degree = axis_conv_crotation.eulerAngles;

        return GeneralMethods.normalize_degree(rotate_degree);
    }
}
