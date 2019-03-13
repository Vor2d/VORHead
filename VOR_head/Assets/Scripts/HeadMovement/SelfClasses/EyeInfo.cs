using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeInfo
{
    public enum EyeFunction { linear }
    public enum EyeIndex { left, right }

    public EyeFunction Eye_function { get; set; }
    //[SerializeField] private EC_GameController ECGC_script;

    public float linear_left_hori_k { get; private set; }
    public float linear_left_hori_b { get; private set; }
    public float linear_left_vert_k { get; private set; }
    public float linear_left_vert_b { get; private set; }
    public float linear_right_hori_k { get; private set; }
    public float linear_right_hori_b { get; private set; }
    public float linear_right_vert_k { get; private set; }
    public float linear_right_vert_b { get; private set; }

    public EyeInfo()
    {
        this.Eye_function = EyeFunction.linear;
        this.linear_left_hori_k = 0;
        this.linear_left_hori_b = 0;
        this.linear_left_vert_k = 0;
        this.linear_left_vert_b = 0;
        this.linear_right_hori_k = 0;
        this.linear_right_hori_b = 0;
        this.linear_right_vert_k = 0;
        this.linear_right_vert_b = 0;
    }

    public EyeInfo(EyeInfo otherEI)
    {
        this.Eye_function = otherEI.Eye_function;
        this.linear_left_hori_k = otherEI.linear_left_hori_k;
        this.linear_left_hori_b = otherEI.linear_left_hori_b;
        this.linear_left_vert_k = otherEI.linear_left_hori_k;
        this.linear_left_vert_b = otherEI.linear_left_hori_b;
        this.linear_right_hori_k = otherEI.linear_right_hori_k;
        this.linear_right_hori_b = otherEI.linear_right_hori_b;
        this.linear_right_vert_k = otherEI.linear_right_hori_k;
        this.linear_right_vert_b = otherEI.linear_right_hori_b;
    }

    //left_eye_voltages: target position (degree), then eye voltages;
    //Horizontal then Vertical;
    public void calibration(List<KeyValuePair<Vector2,Vector2>> left_eye_voltages,
                            List<KeyValuePair<Vector2, Vector2>> right_eye_voltages,
                            EC_GameController.FitMode fit_mode)
    {
        switch(fit_mode)
        {
            case EC_GameController.FitMode.P2P:
                List<KeyValuePair<Vector2, Vector2>> left_target_Volmedian =
                                                get_median_list(left_eye_voltages);
                List<KeyValuePair<Vector2, Vector2>> right_target_Volmedian =
                                                get_median_list(right_eye_voltages);
                fit_function(EyeFunction.linear, EyeIndex.left, left_target_Volmedian);
                fit_function(EyeFunction.linear, EyeIndex.right, right_target_Volmedian);
                break;
            case EC_GameController.FitMode.continuously:
                fit_function(EyeFunction.linear, EyeIndex.left, left_eye_voltages);
                fit_function(EyeFunction.linear, EyeIndex.right, right_eye_voltages);
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
            {
                if (eye_data.Count < 2)
                {
                    Debug.Log("Fit function error!");
                    return;
                }
                fit_linear(target_EI, eye_data);
                break;
            }
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
        float LLHb = 0.0f;
        float LLHk = 0.0f;
        float LLVb = 0.0f;
        float LLVk = 0.0f;
        float LRHb = 0.0f;
        float LRHk = 0.0f;
        float LRVb = 0.0f;
        float LRVk = 0.0f;
        switch (target_EI)
        {
            case EyeIndex.left:
                GeneralMethods.linear_regression(horizontal_x_y_list,
                                                        out LLHb, out LLHk);
                GeneralMethods.linear_regression(horizontal_x_y_list,
                                                        out LLVb, out LLVk);
                linear_left_hori_b = LLHb;
                linear_left_hori_k = LLHk;
                linear_left_vert_b = LLVb;
                linear_left_vert_k = LLVk;
                break;
            case EyeIndex.right:
                GeneralMethods.linear_regression(horizontal_x_y_list,
                                                        out LRHb, out LRHk);
                GeneralMethods.linear_regression(horizontal_x_y_list,
                                                        out LRVb, out LRVk);
                linear_right_hori_b = LRHb;
                linear_right_hori_k = LRHk;
                linear_right_vert_b = LRVb;
                linear_right_vert_k = LRVk;
                break;
        }
    }

    public string var_to_string()
    {
        string result = "";

        result += " LLHk " + linear_left_hori_k.ToString("F2");
        result += " LLHb " + linear_left_hori_b.ToString("F2");
        result += " LLVk " + linear_left_vert_k.ToString("F2");
        result += " LLVb " + linear_left_vert_b.ToString("F2");
        result += " LRHk " + linear_right_hori_k.ToString("F2");
        result += " LRHb " + linear_right_hori_b.ToString("F2");
        result += " LRVk " + linear_right_vert_k.ToString("F2");
        result += " LRVb " + linear_right_vert_b.ToString("F2");

        return result;
    }
}
