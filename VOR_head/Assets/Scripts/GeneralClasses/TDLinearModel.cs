using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//y = a + b1x + b2x;
public class TDLinearModel : GeneralModel
{
    private float a;
    private float b1;
    private float b2;

    public TDLinearModel() : base()
    {
        this.a = 0.0f;
        this.b1 = 0.0f;
        this.b2 = 0.0f;
    }

    public TDLinearModel(TDLinearModel other_LM)
    {
        this.a = other_LM.a;
        this.b1 = other_LM.b1;
        this.b2 = other_LM.b2;
    }

    //a = avy - b1avx1 - b2avx2;
    //b1 = (Sx2sq*Sx1y - Sx1x2*Sx2y)/(Sx1sq*Sx2sq - (Sx1x2)sq);
    //b2 = (Sx1sq*Sx2y - Sx1x2*Sx1y)/(Sx1sq*Sx2sq - (Sx1x2)sq);
    public override void fit_model(List<Vector3> x1_x2_y_list)
    {
        float x1 = 0.0f;
        float x2 = 0.0f;
        float y = 0.0f;
        float x1_sum = 0.0f;
        float x2_sum = 0.0f;
        float y_sum = 0.0f;
        float x1_sq_sum = 0.0f;
        float x2_sq_sum = 0.0f;
        float x1_y_sum = 0.0f;
        float x2_y_sum = 0.0f;
        float x1_x2_sum = 0.0f;
        foreach(Vector3 x1_x2_y in x1_x2_y_list)
        {
            x1 = x1_x2_y.x;
            x2 = x1_x2_y.y;
            y = x1_x2_y.z;

            x1_sum += x1;
            x2_sum += x2;
            y_sum += y;
            x2_sq_sum += Mathf.Pow(x2, 2.0f);
            x1_y_sum += x1 * y;
            x1_x2_sum += x1 * x2;
            x2_y_sum += x2 * y;
            x1_sq_sum += Mathf.Pow(x1, 2.0f);
        }

        float ave_x1 = x1_sum / x1_x2_y_list.Count;
        float ave_x2 = x2_sum / x1_x2_y_list.Count;
        float ave_y = y_sum / x1_x2_y_list.Count;
        float Sx2sqSx1y = x2_sq_sum * x1_y_sum;
        float Sx1x2Sx2y = x1_x2_sum * x2_y_sum;
        float Sx1sqSx2sq = x1_sq_sum * x2_sq_sum;
        float Sx1x2_sq = Mathf.Pow(x1_x2_sum, 2.0f);
        float Sx1sqSx2y = x1_sq_sum * x2_y_sum;
        float Sx1x2Sx1y = x1_x2_sum * x1_y_sum;

        Debug.Log("ave_x1 " + ave_x1);
        Debug.Log("ave_x2 " + ave_x2);
        Debug.Log("ave_y " + ave_y);
        Debug.Log("x2_sq_sum " + x2_sq_sum);
        Debug.Log("x1_y_sum " + x1_y_sum);
        Debug.Log("x1_x2_sum " + x1_x2_sum);
        Debug.Log("x2_y_sum " + x2_y_sum);
        Debug.Log("x1_sq_sum " + x1_sq_sum);
        Debug.Log("Sx2sqSx1y " + Sx2sqSx1y);
        Debug.Log("Sx1x2Sx2y " + Sx1x2Sx2y);
        Debug.Log("Sx1sqSx2sq " + Sx1sqSx2sq);
        Debug.Log("Sx1x2_sq " + Sx1x2_sq);
        Debug.Log("Sx1sqSx2y " + Sx1sqSx2y);
        Debug.Log("Sx1x2Sx1y " + Sx1x2Sx1y);

        b1 = (Sx2sqSx1y - Sx1x2Sx2y) / (Sx1sqSx2sq - Sx1x2_sq);
        b2 = (Sx1sqSx2y - Sx1x2Sx1y) / (Sx1sqSx2sq - Sx1x2_sq);
        a = ave_y - b1 * ave_x1 - b2 * ave_x2;
    }

    public override float get_value(Vector2 x1_x2)
    {
        return a + b1 * x1_x2.x + b2 * x1_x2.y;
    }

    public override string var_to_string()
    {
        return "TDLinear: a " + a.ToString("F2") + 
                        " b1 " + b1.ToString("F2") + 
                        " b2 " + b2.ToString("F2");
    }
}
