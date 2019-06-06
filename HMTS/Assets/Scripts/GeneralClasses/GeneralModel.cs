using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GeneralModel
{
    public GeneralModel()
    {

    }

    //x then y; voltage then target degree;
    public virtual void fit_model(List<Vector2> x_y_list) { }

    //x1, x2 then y; H voltage V voltage then target degree;
    public virtual void fit_model(List<Vector3> x1_x2_y_list) { }

    public virtual void fit_model(AForge.Neuro.ActivationNetwork _NNetwork) { }

    public virtual float get_value(float x)
    {
        return -1.0f;
    }

    //x1,x2; H voltage then V voltage;
    public virtual float get_value(Vector2 x1_x2)
    {
        return -1.0f;
    }

    public virtual string var_to_string()
    {
        return "";
    }


}
