using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DebugController : MonoBehaviour
{

    //Debug Tests;
    public float RotateDegree = 0.0f;

    const string HT_init_text = "Head Rotation: ";
    const string ST_init_text = "Current State: ";
    const string VRLST_init_text = "VR Logging: ";    //VRLoggingState init text;
    const string JLS_init_text = "Jump Logging: ";    //JumpLoggingState init text;
    const string LTN_init_text = "Loop and Trial and Section Iterator: ";
    const string Acuity_init_text = "Acuity (Index/Right/Wrong/Size/TargetSize/Max_LH" +
        "/SRN/DRN): ";
    const string AcuityDelay_init_text = "Acuity (DelayIndex/CurrDelay/DelayRight/Converge/RepeatIndex" +
        "/TargetAD/AD_LR/AD_RR/HTC): ";

    public bool UsingDebug;
    public GameController GC_script;
    public HeadSimulator HS_script;
    public VRLogSystem VRLS_script;
    public JumpLogSystem JLS_script;
    public AcuityLogSystem ALS_script;
    public CoilData CD_script;
    public Transform DebugTarget_TRANS;
    public Vector3 DebugTargetDegree;
    public Vector3 RealToVirtualTest;
    public Text RealToVirtualTest_Text;
    public Text Debug_taget_Text;
    public Text Headrr_Text;
    public Text State_Text;
    public Text VRLoggingState_Text;
    public Text JumpLoggingState_Text;
    public Text LoopTrialNumber_Text;
    public bool UsingEyeSimulator;
    public Text AcuityState_Text;
    public Text AcuityDelay_Text;
    public Camera camera1;
    public Camera camera2;
    public Camera camera3;
    public Canvas canvas1;
    public Transform target_TRANS;
    [SerializeField] private TextMesh DebugText1;
    [SerializeField] private TextMesh DebugText2;
    [SerializeField] private TextMeshPro DebugTMP1;
    [SerializeField] private TextMeshPro DebugTMP2;
    [SerializeField] private Transform HeadIndicator_TRANS;

    public static DebugController Instance;

    private DataController DC_script;
    private Dictionary<string, string> text1Hash;

    private void Awake()
    {
        Instance = this;
        text1Hash = new Dictionary<string, string>();
    }

    // Use this for initialization
    void Start()
    {
        DC_script = GameObject.Find("DataController").GetComponent<DataController>();

        init_debug();
    }

    // Update is called once per frame
    void Update()
    {
        show_info();

        debug_group();
    }

    private void LateUpdate()
    {
        if (UsingEyeSimulator)
        {
            eye_test();
        }
    }

    private void init_debug()
    {
        if (UsingDebug)
        {
            //GC_script.test1();
        }
    }

    private void gener_text1()
    {
        string DT1 = "";
        //DT1 += camera1.transform.eulerAngles.ToString("F4");
        //DT1 += camera2.transform.eulerAngles.ToString("F4");
        //DT1 += camera3.transform.eulerAngles.ToString("F4");
        //DebugText1.text = DT1;

        DT1 = "";
        DT1 += target_TRANS.position.ToString("F4");
        DT1 += HeadIndicator_TRANS.position.ToString("F4");
        DebugText1.text = DT1;

        //foreach (string text in text1Hash.Values)
        //{
        //    DebugText1.text += text;
        //}
    }

    private void debug_group()
    {
        if (UsingDebug)
        {
            string text2 = "";
            foreach (string ds in GameController.Debug_str)
            {
                text2 += ds + "////";
            }
            //DebugTMP2.text = text2;
            //DebugTMP1.text = GC_script.back_cal().ToString("F3");

            gener_text1();
        }
    }

    private void show_info()
    {
        Headrr_Text.text = HT_init_text + HS_script.TrueHeadRR.ToString("F2")
                    + "\t" + GC_script.turn_degree_x.ToString("F2");
        State_Text.text = ST_init_text + GC_script.Current_state;
        VRLoggingState_Text.text = VRLST_init_text + VRLS_script.thread_state_flag;
        JumpLoggingState_Text.text = JLS_init_text + JLS_script.log_state_flag
            + " / AcuityLog: " + ALS_script.thread_state_flag;
        LoopTrialNumber_Text.text = LTN_init_text + GC_script.loop_iter.ToString() +
                                            "\t & \t" + GC_script.trial_iter.ToString() +
                                            "\t & \t" + GC_script.section_number +
                                            "\t" + DC_script.Current_GM.GameModeName;
        AcuityState_Text.text = Acuity_init_text + (GC_script.AcuityState[0]).ToString("F0") +
                                        " / " + (GC_script.AcuityState[1]).ToString("F0") +
                                        " / " + (GC_script.AcuityState[2]).ToString("F0") +
                                        " / " + (GC_script.AcuityState[3]).ToString("F0") +
                                        " / " + (GC_script.AcuityState[10]).ToString("F0") +
                                        " / " + (GC_script.AcuityState[11]).ToString("F3") +
                                        " / " + (GC_script.AcuityState[15]).ToString() +
                                        " / " + (GC_script.AcuityState[16]).ToString();
        AcuityDelay_Text.text = AcuityDelay_init_text + (GC_script.AcuityState[4]).ToString("F0") +
                                " / " + (GC_script.AcuityState[5]).ToString("F3") +
                                " / " + (GC_script.AcuityState[6]).ToString("F0") +
                                " / " + (GC_script.AcuityState[7]).ToString("F0") +
                                " / " + (GC_script.AcuityState[8]).ToString("F0") +
                                " / " + (GC_script.AcuityState[9]).ToString("F3") +
                                " / " + (GC_script.AcuityState[12]).ToString("F0") +
                                " / " + (GC_script.AcuityState[13]).ToString("F0") +
                                " / " + (GC_script.AcuityState[14]).ToString("F0");
    }

    private void eye_test()
    {
        float random = Random.Range(-100.0f, 100.0f);
        CD_script.Left_eye_voltage =
                                new Vector2(-random, -(DebugTargetDegree.x - random));
        CD_script.Right_eye_voltage =
            (new Vector2(DebugTargetDegree.y, DebugTargetDegree.x * 2.0f));

        //CD_script.Left_eye_voltage =
        //    new Vector2(DebugTargetDegree.y, DebugTargetDegree.x);
        //CD_script.Left_eye_voltage =
        //    new Vector2(DebugTargetDegree.y, DebugTargetDegree.x);

        //HS_script.TrueHeadRR = new Vector3(3.0f, -10.0f, 0.0f);
        //Vector3 Debug_taget = GeneralMethods.RealToVirtual_curved(180.0f, 121.0f,
        //                        DebugTargetDegree.x, DebugTargetDegree.y);
        //Debug_taget_Text.text = Debug_taget.ToString("F2");
        //DebugTarget_TRANS.GetComponent<ChangePosition>().changePosition(
        //                            Debug_taget.y, Debug_taget.x);

        //RealToVirtualTest_Text.text = GeneralMethods.RealToVirtual_curved(180.0f, 121.0f,
        //                            RealToVirtualTest.x, RealToVirtualTest.y).ToString("F2");
    }

    public void add_text1(string str)
    {
        DebugText1.text += str;
    }

    public void update_text1(string str,string id)
    {
        text1Hash[id] = str;
    }

    public void clear_text1()
    {
        text1Hash.Clear();
    }
}
