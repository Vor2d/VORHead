using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EyeFunction { linear }
public enum EyeIndex { left,right}

public class EyeInfo
{
    public EyeFunction eye_function { get; set; }

    private float linear_leftk;
    private float linear_leftb;
    private float linear_rightk;
    private float linear_rightb;

    public EyeInfo()
    {
        this.eye_function = EyeFunction.linear;
        this.linear_leftk = 0;
        this.linear_leftb = 0;
        this.linear_rightk = 0;
        this.linear_rightb = 0;
    }

    public EyeInfo(EyeInfo otherEI)
    {
        this.eye_function = otherEI.eye_function;
        this.linear_leftk = otherEI.linear_leftk;
        this.linear_leftb = otherEI.linear_leftb;
        this.linear_rightk = otherEI.linear_rightk;
        this.linear_rightb = otherEI.linear_rightb;
    }

    public void fit_function(EyeFunction target_EF, EyeIndex target_EI,
                                List<List<float>> x_list, List<List<float>> y_list)
    {
        switch(target_EF)
        {
            case EyeFunction.linear:
                {
                    if (x_list.Count < 2 || y_list.Count < 2)
                    {
                        return;
                    }
                    fit_linear(target_EI,x_list,y_list);
                    break;
                }
        }
    }

    private void fit_linear(EyeIndex target_EI,
                                List<List<float>> x_list, List<List<float>> y_list)
    {
        float x_median1 = GeneralMethods.get_median(x_list[0]);
        float x_median2 = GeneralMethods.get_median(x_list[1]);
        float y_median1 = GeneralMethods.get_median(y_list[0]);
        float y_median2 = GeneralMethods.get_median(y_list[1]);

        switch(target_EI)
        {
            case EyeIndex.left:
                {
                    GeneralMethods.fit_linear(x_median1, x_median2, y_median1, y_median2,
                                                out linear_leftk, out linear_leftb);
                    break;
                }
            case EyeIndex.right:
                {
                    GeneralMethods.fit_linear(x_median1, x_median2, y_median1, y_median2,
                                                out linear_rightk, out linear_rightb);
                    break;
                }
        }
        
    }
}
