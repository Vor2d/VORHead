using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeSimulator : MonoBehaviour
{
    [SerializeField] private CoilData CD_script;

    [SerializeField] private EyeInfo.EyeIndex CurrEyeIndex;

    private Vector2 voltage;
    private float real_hori_degree;
    private float real_vert_degree;
    private Vector3 real_rotate;
    private Vector3 virtual_rotate;

    private DataController DC_script;

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
    }

    private void Update()
    {

        switch(CurrEyeIndex)
        {
            case EyeInfo.EyeIndex.left:
                voltage = CD_script.Left_eye_voltage;
                real_hori_degree = GeneralMethods.linear_cal_back(
                                        DC_script.Eye_info.linear_left_hori_k, 
                                        DC_script.Eye_info.linear_left_hori_b, 
                                        voltage.x);
                real_vert_degree = GeneralMethods.linear_cal_back(
                                        DC_script.Eye_info.linear_left_vert_k,
                                        DC_script.Eye_info.linear_left_vert_b,
                                        voltage.y);
                //real_rotate = new Vector3(real_vert_degree, real_hori_degree, 0.0f);

                break;
            case EyeInfo.EyeIndex.right:
                voltage = CD_script.Right_eye_voltage;
                real_hori_degree = GeneralMethods.linear_cal_back(
                                        DC_script.Eye_info.linear_right_hori_k,
                                        DC_script.Eye_info.linear_right_hori_b,
                                        voltage.x);
                real_vert_degree = GeneralMethods.linear_cal_back(
                                        DC_script.Eye_info.linear_right_vert_k,
                                        DC_script.Eye_info.linear_right_vert_b,
                                        voltage.y);
                break;
        }
        if (DC_script.SystemSetting.Using_curved_screen)
        {
            virtual_rotate = GeneralMethods.RealToVirtual_curved(
                                DC_script.SystemSetting.Player_screen_cm,
                                DC_script.SystemSetting.Screen_width_cm,
                                real_vert_degree, real_hori_degree);
        }
        else
        {
            virtual_rotate = GeneralMethods.RealToVirtual(
                                DC_script.SystemSetting.Player_screen_cm,
                                DC_script.SystemSetting.Screen_width_cm,
                                real_vert_degree, real_hori_degree);
        }
        transform.localEulerAngles = virtual_rotate;
    }


}
