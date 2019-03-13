using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EC_DebugController : MonoBehaviour
{
    public HeadSimulator HS_script;
    public Vector3 ManuHeadRotate;
    public EC_GameController ECGC_script;
    public CoilData CD_script;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        switch(ECGC_script.DC_script.Fit_Mode)
        {
            case EC_GameController.FitMode.P2P:
                CD_script.Left_eye_voltage = ECGC_script.Curr_target;
                CD_script.Right_eye_voltage = 
                                ECGC_script.Curr_target * 2 + new Vector2(1.0f, 1.0f);
                break;
            case EC_GameController.FitMode.continuously:
                HS_script.TrueHeadRR = ManuHeadRotate;
                CD_script.Left_eye_voltage = 
                    new Vector2(-HS_script.TrueHeadRR.y, -HS_script.TrueHeadRR.x);
                CD_script.Right_eye_voltage = 
                    new Vector2(-HS_script.TrueHeadRR.y, -HS_script.TrueHeadRR.x) * 2.0f +
                    new Vector2(1.0f,1.0f);
                break;
        }
        
    }
}
