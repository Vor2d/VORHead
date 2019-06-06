using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HMTS_enum;
using UnityEngine.UI;

public class EC_DebugController : MonoBehaviour
{
    public HeadSimulator HS_script;
    public Vector3 ManuHeadRotate;
    public EC_GameController ECGC_script;
    public CoilData CD_script;
    public Text LearningRateText;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        LearningRateText.text = ECGC_script.get_NN1thread().get_learning_rate().ToString("F10");
        float random;
        switch (ECGC_script.DC_script.FitMode)
        {
            case EyeFitMode.P2P:
                CD_script.Left_eye_voltage = ECGC_script.Curr_target;
                CD_script.Right_eye_voltage = 
                                ECGC_script.Curr_target * 2 + new Vector2(1.0f, 1.0f);
                break;
            case EyeFitMode.continuously:
                HS_script.TrueHeadRR = ManuHeadRotate;
                random = Random.Range(-100.0f, 100.0f);
                CD_script.Left_eye_voltage = 
                    new Vector2(random, HS_script.TrueHeadRR.x - random);
                CD_script.Right_eye_voltage =
                    new Vector2(-HS_script.TrueHeadRR.y, -HS_script.TrueHeadRR.x * 2.0f);
                break;
            case EyeFitMode.self_detect:
                //HS_script.TrueHeadRR = ManuHeadRotate;
                //CD_script.Left_eye_voltage = new Vector2(-ManuHeadRotate.y, -ManuHeadRotate.x);
                //CD_script.Right_eye_voltage = 
                //    new Vector2(-ManuHeadRotate.y * 2.0f, -ManuHeadRotate.x * 2.0f);
                HS_script.TrueHeadRR = ManuHeadRotate;
                random = Random.Range(-90.0f, 90.0f);
                CD_script.Left_eye_voltage =
                    new Vector2(random, HS_script.TrueHeadRR.x - random);
                CD_script.Right_eye_voltage =
                    new Vector2(-HS_script.TrueHeadRR.y, -HS_script.TrueHeadRR.x * 2.0f);
                break;
        }
        
    }
}
