using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeInfo
{
    public enum EyeFunction { linear }
    public enum EyeIndex { left, right }

    public EyeFunction Eye_function { get; set; }

    private float linear_leftk;
    private float linear_leftb;
    private float linear_rightk;
    private float linear_rightb;

    public EyeInfo()
    {
        this.Eye_function = EyeFunction.linear;
        this.linear_leftk = 0;
        this.linear_leftb = 0;
        this.linear_rightk = 0;
        this.linear_rightb = 0;
    }

    public EyeInfo(EyeInfo otherEI)
    {
        this.Eye_function = otherEI.Eye_function;
        this.linear_leftk = otherEI.linear_leftk;
        this.linear_leftb = otherEI.linear_leftb;
        this.linear_rightk = otherEI.linear_rightk;
        this.linear_rightb = otherEI.linear_rightb;
    }

    public void fit_function(EyeFunction target_EF, EyeIndex target_EI,
                                List<KeyValuePair<float, List<float>>> eye_data)
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

    private void fit_linear(EyeIndex target_EI, List<KeyValuePair<float,List<float>>> eye_data)
    {
        //float x_median1 = GeneralMethods.get_median(x_list[0]);
        //float x_median2 = GeneralMethods.get_median(x_list[1]);
        //float y_median1 = GeneralMethods.get_median(y_list[0]);
        //float y_median2 = GeneralMethods.get_median(y_list[1]);
        List<Vector2> x_y_list = new List<Vector2>();
        foreach (KeyValuePair<float, List<float>> ED_each_trial in eye_data)
        {
            x_y_list.Add(new Vector2(GeneralMethods.get_median(ED_each_trial.Value),
                                      ED_each_trial.Key));
        }

        List<Vector2> k_b_list = new List<Vector2>();
        float k, b;
        switch (target_EI)
        {
            case EyeIndex.left:
                {
                    //GeneralMethods.fit_linear(x_median1, x_median2, y_median1, y_median2,
                    //                            out linear_leftk, out linear_leftb);
                    for(int i = 0;i< x_y_list.Count;i+=2)
                    {
                        if(i+1 < x_y_list.Count)
                        {
                            //GeneralMethods.fit_linear(x_y_list[i].x,x_y_list[i+1].x,
                            //                            x_y_list[i].y,x_y)
                        }
                    }
                    break;
                }
            case EyeIndex.right:
                {
                    //GeneralMethods.fit_linear(x_median1, x_median2, y_median1, y_median2,
                    //                            out linear_rightk, out linear_rightb);
                    break;
                }
        }
        
    }
}
