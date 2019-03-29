using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class HeadSimulator : MonoBehaviour {

    //public Quaternion OriginalheadQ { get; set; }
    public Vector3 RRotateDegree {get;set;}
    public Vector3 TrueHeadRR { get; set; }
    public CoilData CD_script;
    public Vector3 VirtualRDeg { get; set; }
    [SerializeField] private Transform HeadIndicator_TRANS;

    private DataController DC_script;
    private float player_screen_cm;
    private float screen_width_cm;
    private Quaternion current_headQ;
    private ChangePosition CP_script;

    // Use this for initialization
    void Start () {
        this.DC_script = GameObject.Find("DataController").GetComponent<DataController>();

        this.player_screen_cm = DC_script.SystemSetting.Player_screen_cm;
        this.screen_width_cm = DC_script.SystemSetting.Screen_width_cm;
        this.RRotateDegree = new Vector3();
        this.TrueHeadRR = new Vector3();
        this.VirtualRDeg = new Vector3();
        this.CP_script = HeadIndicator_TRANS.GetComponent<ChangePosition>();
    }
	
	// Update is called once per frame
	void Update () {

        if(DC_script.using_coil)
        {
            Quaternion coil_rotation = CD_script.currentHeadOrientation;

            current_headQ = coil_rotation * Quaternion.Inverse(DC_script.Head_origin);

            //////////
            //TrueHeadRR = CRotaQuatToRRotaDegr(current_headQ);

            RRotateDegree = TrueHeadRR * DC_script.Current_GM.Gain;

            if (!DC_script.SystemSetting.Using_curved_screen)
            {
                VirtualRDeg = GeneralMethods.
                                    RealToVirtual(DC_script.SystemSetting.Player_screen_cm,
                                                    DC_script.SystemSetting.Screen_width_cm,
                                                    RRotateDegree.x,
                                                    RRotateDegree.y);
                transform.localEulerAngles = VirtualRDeg;
            }
            else
            {
                VirtualRDeg = GeneralMethods.
                            RealToVirtual_curved(DC_script.SystemSetting.Player_screen_cm,
                                                    DC_script.SystemSetting.Screen_width_cm,
                                                    RRotateDegree.x,
                                                    RRotateDegree.y);
                transform.localEulerAngles = VirtualRDeg;
            }
            CP_script.changePosition(VirtualRDeg.y, VirtualRDeg.x);
        }
        if(DC_script.using_VR)
        {
            transform.rotation = GeneralMethods.getVRrotation();
            TrueHeadRR = GeneralMethods.normalize_degree(transform.rotation.eulerAngles);
        }
        
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
