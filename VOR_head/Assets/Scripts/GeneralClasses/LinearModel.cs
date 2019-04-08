using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//y = b1 * x + b0;
public class LinearModel : GeneralModel
{
    public float b0 { get; private set; }
    public float b1 { get; private set; }

    public LinearModel() : base()
    {
        this.b0 = 0.0f;
        this.b1 = 0.0f;
    }

    public LinearModel(LinearModel other_LM)
    {
        this.b0 = other_LM.b0;
        this.b1 = other_LM.b1;
    }

    public override void fit_model(List<Vector2> x_y_list)
    {
        float x_sum = 0.0f;
        float y_sum = 0.0f;
        foreach (Vector2 x_y in x_y_list)
        {
            x_sum += x_y.x;
            y_sum += x_y.y;
        }
        float x_mean = x_sum / x_y_list.Count;
        float y_mean = y_sum / x_y_list.Count;
        float x_variance = 0.0f;
        float x_Vsum = 0.0f;
        float x_standardD = 0.0f;
        float y_standardD = 0.0f;
        float xSD_TySD = 0.0f;  //x standard division times y standard division;
        float xSD_TySD_sum = 0.0f;
        foreach (Vector2 x_y in x_y_list)
        {
            x_standardD = x_y.x - x_mean;
            y_standardD = x_y.y - y_mean;
            xSD_TySD = x_standardD * y_standardD;
            xSD_TySD_sum += xSD_TySD;
            x_variance = Mathf.Pow(x_standardD, 2);
            x_Vsum += x_variance;
        }
        b1 = xSD_TySD_sum / x_Vsum;
        b0 = y_mean - b1 * x_mean;
    }

    public override float get_value(float x)
    {
        return b1 * x + b0;
    }

    public override string var_to_string()
    {
        return "LinearModel: b0 " + b0.ToString("F2") + " b1 " + b1.ToString("F2");
    }


}
