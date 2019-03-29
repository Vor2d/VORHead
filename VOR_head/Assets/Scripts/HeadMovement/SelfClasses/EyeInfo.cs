using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using HMTS_enum;

namespace HMTS_enum
{
    public enum EyeFunction { linear, TDlinear,NN1 }
    public enum EyeIndex { left, right }
}

public class EyeInfo
{
    public EyeFunction Eye_function { get; set; }
    public GeneralModel LeftHModel { get; private set; }
    public GeneralModel LeftVModel { get; private set; }
    public GeneralModel RightHModel { get; private set; }
    public GeneralModel RightVModel { get; private set; }

    public EyeInfo()
    {
        this.Eye_function = EyeFunction.linear;
        this.LeftHModel = new LinearModel();
        this.LeftVModel = new LinearModel();
        this.RightHModel = new LinearModel();
        this.RightVModel = new LinearModel();
    }

    public EyeInfo(EyeInfo otherEI)
    {
        this.Eye_function = otherEI.Eye_function;
        assign_model(otherEI);
    }

    private void assign_model(EyeInfo otherEI)
    {
        switch (otherEI.Eye_function)
        {
            case EyeFunction.linear:
                LeftHModel = new LinearModel(otherEI.LeftHModel as LinearModel);
                LeftVModel = new LinearModel(otherEI.LeftVModel as LinearModel);
                RightHModel = new LinearModel(otherEI.RightHModel as LinearModel);
                RightVModel = new LinearModel(otherEI.RightVModel as LinearModel);
                break;
            case EyeFunction.TDlinear:
                LeftHModel = new TDLinearModel(otherEI.LeftHModel as TDLinearModel);
                LeftVModel = new TDLinearModel(otherEI.LeftVModel as TDLinearModel);
                RightHModel = new TDLinearModel(otherEI.RightHModel as TDLinearModel);
                RightVModel = new TDLinearModel(otherEI.RightVModel as TDLinearModel);
                break;
            case EyeFunction.NN1:
                LeftHModel = new NN1Model(otherEI.LeftHModel as NN1Model);
                LeftVModel = new NN1Model(otherEI.LeftVModel as NN1Model);
                RightHModel = new NN1Model(otherEI.RightHModel as NN1Model);
                RightVModel = new NN1Model(otherEI.RightVModel as NN1Model);
                break;
        }
    }

    public void set_model(EyeFunction eye_function)
    {
        switch(eye_function)
        {
            case EyeFunction.linear:
                LeftHModel = new LinearModel();
                LeftVModel = new LinearModel();
                RightHModel = new LinearModel();
                RightVModel = new LinearModel();
                break;
            case EyeFunction.TDlinear:
                LeftHModel = new TDLinearModel();
                LeftVModel = new TDLinearModel();
                RightHModel = new TDLinearModel();
                RightVModel = new TDLinearModel();
                break;
            case EyeFunction.NN1:
                LeftHModel = new NN1Model(0);
                LeftVModel = new NN1Model(1);
                RightHModel = new NN1Model(0);
                RightVModel = new NN1Model(1);
                break;
        }
    }

    //left_eye_voltages: target position (degree), then eye voltages;
    //Horizontal then Vertical;
    public void calibration(List<KeyValuePair<Vector2,Vector2>> left_eye_voltages,
                            List<KeyValuePair<Vector2, Vector2>> right_eye_voltages,
                            EyeFitMode fit_mode, EyeFunction eye_function,
                            AForge.Neuro.ActivationNetwork left_NN,
                            AForge.Neuro.ActivationNetwork right_NN)
    {
        switch(fit_mode)
        {
            case EyeFitMode.P2P:
                List<KeyValuePair<Vector2, Vector2>> left_target_Volmedian =
                                                get_median_list(left_eye_voltages);
                List<KeyValuePair<Vector2, Vector2>> right_target_Volmedian =
                                                get_median_list(right_eye_voltages);
                fit_function(eye_function, EyeIndex.left, left_target_Volmedian);
                fit_function(eye_function, EyeIndex.right, right_target_Volmedian);
                break;
            case EyeFitMode.continuously:
                fit_function(eye_function, EyeIndex.left, left_eye_voltages);
                fit_function(eye_function, EyeIndex.right, right_eye_voltages);
                break;
            case EyeFitMode.self_detect:
                fit_NN(eye_function, EyeIndex.left, left_NN);
                fit_NN(eye_function, EyeIndex.right, right_NN);
                break;
        }
    }

    private void fit_NN(EyeFunction eye_function, EyeIndex eye_index, 
                        AForge.Neuro.ActivationNetwork NNetwork)
    {
        switch(eye_function)
        {
            case EyeFunction.NN1:
                switch(eye_index)
                {
                    case EyeIndex.left:
                        LeftHModel.fit_model(NNetwork);
                        LeftVModel.fit_model(NNetwork);
                        break;
                    case EyeIndex.right:
                        RightHModel.fit_model(NNetwork);
                        RightVModel.fit_model(NNetwork);
                        break;
                }
                break;
        }
    }

    private List<KeyValuePair<Vector2,Vector2>> get_median_list(
                                        List<KeyValuePair<Vector2, Vector2>> eye_voltages)
    {
        List<KeyValuePair<Vector2, Vector2>> target_Volmedian =
                                    new List<KeyValuePair<Vector2, Vector2>>();
        Vector2 last_target = new Vector2();
        List<float> EV_horizontal = new List<float>();
        List<float> EV_vertical = new List<float>();
        float EV_Hmedian = 0.0f;
        float EV_Vmedian = 0.0f;
        last_target = eye_voltages[0].Key;
        foreach (KeyValuePair<Vector2, Vector2> eye_voltage in eye_voltages)
        {
            if (last_target != eye_voltage.Key)
            {
                EV_Hmedian = GeneralMethods.get_median(EV_horizontal);
                EV_Vmedian = GeneralMethods.get_median(EV_vertical);
                target_Volmedian.Add(new KeyValuePair<Vector2, Vector2>(
                            last_target, new Vector2(EV_Hmedian, EV_Vmedian)));
                last_target = eye_voltage.Key;
                EV_horizontal = new List<float>();
                EV_vertical = new List<float>();
            }
            else
            {
                EV_horizontal.Add(eye_voltage.Value.x);
                EV_vertical.Add(eye_voltage.Value.y);
            }
        }
        EV_Hmedian = GeneralMethods.get_median(EV_horizontal);
        EV_Vmedian = GeneralMethods.get_median(EV_vertical);
        target_Volmedian.Add(new KeyValuePair<Vector2, Vector2>(
                    last_target, new Vector2(EV_Hmedian, EV_Vmedian)));
        return target_Volmedian;
    }

    //Eye_data: target position (degrees), then eye voltages;
    //Horizontal first;
    public void fit_function(EyeFunction target_EF, EyeIndex target_EI,
                                    List<KeyValuePair<Vector2, Vector2>> eye_data)
    {
        switch(target_EF)
        {
            case EyeFunction.linear:
                if (eye_data.Count < 2)
                {
                    throw new Exception("Fit function error!");
                }
                fit_linear(target_EI, eye_data);
                break;
            case EyeFunction.TDlinear:
                if (eye_data.Count < 2)
                {
                    throw new Exception("Fit function error!");
                }
                fit_TDlinear(target_EI, eye_data);
                break;
        }
    }

    private void fit_TDlinear(EyeIndex target_EI, List<KeyValuePair<Vector2, Vector2>> eye_data)
    {
        //Horizotal horizontal_x vertical_x target_degree list;
        List<Vector3> H_Hx_Vx_HT_list = new List<Vector3>();
        List<Vector3> V_Hx_Vx_VT_list = new List<Vector3>();
        foreach (KeyValuePair<Vector2, Vector2> target_EVol in eye_data)
        {
            H_Hx_Vx_HT_list.Add(new Vector3(target_EVol.Value.x,
                                            target_EVol.Value.y,
                                            target_EVol.Key.x));
            V_Hx_Vx_VT_list.Add(new Vector3(target_EVol.Value.x,
                                            target_EVol.Value.y,
                                            target_EVol.Key.y));
        }
        switch (target_EI)
        {
            case EyeIndex.left:
                LeftHModel.fit_model(H_Hx_Vx_HT_list);
                Debug.Log("Start!!!!!");
                LeftVModel.fit_model(V_Hx_Vx_VT_list);
                Debug.Log("End!!!!!");
                break;
            case EyeIndex.right:
                RightHModel.fit_model(H_Hx_Vx_HT_list);
                RightVModel.fit_model(V_Hx_Vx_VT_list);
                break;
        }
    }

    private void fit_linear(EyeIndex target_EI, List<KeyValuePair<Vector2, Vector2>> eye_data)
    {
        List<Vector2> horizontal_x_y_list = new List<Vector2>();
        List<Vector2> vertical_x_y_list = new List<Vector2>();
        foreach (KeyValuePair<Vector2, Vector2> target_EVol in eye_data)
        {
            horizontal_x_y_list.
                    Add(new Vector2(target_EVol.Value.x, target_EVol.Key.x));
            vertical_x_y_list.
                    Add(new Vector2(target_EVol.Value.y, target_EVol.Key.y));
        }
        switch (target_EI)
        {
            case EyeIndex.left:
                LeftHModel.fit_model(horizontal_x_y_list);
                LeftVModel.fit_model(vertical_x_y_list);
                break;
            case EyeIndex.right:
                RightHModel.fit_model(horizontal_x_y_list);
                RightVModel.fit_model(vertical_x_y_list);
                break;
        }
    }

    public string var_to_string()
    {
        string result = "";

        result += "LeftHModel " + LeftHModel.var_to_string() + "\n";
        result += "LeftVModel " + LeftVModel.var_to_string() + "\n";
        result += "RightHModel " + RightHModel.var_to_string() + "\n";
        result += "RightVModel " + RightVModel.var_to_string() + "\n";

        return result;
    }
}
