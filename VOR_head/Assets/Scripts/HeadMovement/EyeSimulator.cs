using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HMTS_enum;

public class EyeSimulator : MonoBehaviour
{
    [SerializeField] private CoilData CD_script;

    [SerializeField] private EyeIndex CurrEyeIndex;
    [SerializeField] private HeadSimulator HS_script;
    [SerializeField] private Transform EyeIndicator_TRANS;

    private Vector2 voltage;
    private float real_hori_degree;
    private float real_vert_degree;
    private Vector3 real_rotate;
    private Vector3 virtual_rotate;
    private float combined_RHDegr; //degree combined with head rotation.
    private float combined_RVDegr;

    private DataController DC_script;
    private ChangePosition CP_script;

    private void Start()
    {
        if(DC_script == null)
        {
            DC_script = GameObject.Find("DataController").GetComponent<DataController>();
        }

        this.voltage = new Vector2();
        this.real_rotate = new Vector3();
        this.real_hori_degree = 0.0f;
        this.real_vert_degree = 0.0f;
        this.combined_RHDegr = 0.0f;
        this.combined_RVDegr = 0.0f;
        this.CP_script = EyeIndicator_TRANS.GetComponent<ChangePosition>();
    }

    private void Update()
    {
        simulate_eye();

    }

    private void simulate_eye()
    {
        switch (CurrEyeIndex)
        {
            case EyeIndex.left:
                voltage = CD_script.Left_eye_voltage;
                switch(DC_script.FitFunction)
                {
                    case EyeFunction.linear:
                        real_hori_degree = DC_script.Eye_info.LeftHModel.get_value(voltage.x);
                        real_vert_degree = DC_script.Eye_info.LeftVModel.get_value(voltage.y);
                        break;
                    case EyeFunction.TDlinear:
                        real_hori_degree = DC_script.Eye_info.LeftHModel.
                                                get_value(new Vector2(voltage.x,voltage.y));
                        real_vert_degree = DC_script.Eye_info.LeftVModel.
                                                get_value(new Vector2(voltage.x, voltage.y));
                        break;
                }
                break;
            case EyeIndex.right:
                voltage = CD_script.Right_eye_voltage;
                switch (DC_script.FitFunction)
                {
                    case EyeFunction.linear:
                        real_hori_degree = DC_script.Eye_info.RightHModel.get_value(voltage.x);
                        real_vert_degree = DC_script.Eye_info.RightVModel.get_value(voltage.y);
                        break;
                    case EyeFunction.TDlinear:
                        real_hori_degree = DC_script.Eye_info.RightHModel.
                                                get_value(new Vector2(voltage.x,voltage.y));
                        real_vert_degree = DC_script.Eye_info.RightVModel.
                                                get_value(new Vector2(voltage.x, voltage.y));
                        break;
                }

                break;
        }
        combined_RHDegr = HS_script.TrueHeadRR.y + real_hori_degree;
        combined_RVDegr = HS_script.TrueHeadRR.x + real_vert_degree;
        if (DC_script.SystemSetting.Using_curved_screen)
        {
            virtual_rotate = GeneralMethods.RealToVirtual_curved(
                                DC_script.SystemSetting.Player_screen_cm,
                                DC_script.SystemSetting.Screen_width_cm,
                                combined_RVDegr, combined_RHDegr);
        }
        else
        {
            virtual_rotate = GeneralMethods.RealToVirtual(
                                DC_script.SystemSetting.Player_screen_cm,
                                DC_script.SystemSetting.Screen_width_cm,
                                combined_RVDegr, combined_RHDegr);
        }
        transform.localEulerAngles = virtual_rotate;
        CP_script.changePosition(virtual_rotate.y, virtual_rotate.x);
    }

}
