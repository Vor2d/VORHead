using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugController : MonoBehaviour {

    //Debug Tests;
    public float RotateDegree = 0.0f;

    const string HT_init_text = "Head Rotation: ";
    const string ST_init_text = "Current State: ";
    const string VRLST_init_text = "VR Logging: ";    //VRLoggingState init text;
    const string JLS_init_text = "Jump Logging: ";    //JumpLoggingState init text;
    const string LTN_init_text = "Loop and Trial and Section Iterator: ";

    public GameController GC_script;
    //public GameController_Setting GC_S_script;
    public HeadSimulator HS_script;
    public VRLogSystem VRLS_script;
    public JumpLogSystem JLS_script;

    public Text Headrr_Text;
    public Text State_Text;
    public Text VRLoggingState_Text;
    public Text JumpLoggingState_Text;
    public Text LoopTrialNumber_Text;

    private DataController DC_script;

	// Use this for initialization
	void Start () {
        DC_script = GameObject.Find("DataController").GetComponent<DataController>();
	}
	
	// Update is called once per frame
	void Update () {
        //Debug.Log("GC_script.loop_iter" + GC_script.loop_iter);

        Headrr_Text.text = HT_init_text + HS_script.TrueHeadRR.ToString("F2")
                            + "\t" + GC_script.turn_degree.ToString("F2");
        State_Text.text = ST_init_text + GC_script.Current_state;
        VRLoggingState_Text.text = VRLST_init_text + VRLS_script.thread_state_flag;
        JumpLoggingState_Text.text = JLS_init_text + JLS_script.log_state_flag;
        LoopTrialNumber_Text.text = LTN_init_text + GC_script.loop_iter.ToString()
                                            + "\t & \t" + GC_script.trial_iter.ToString()
                                            + "\t & \t" + GC_script.section_number
                                            + "\t" + DC_script.Current_GM.GameModeName;
    }
}
