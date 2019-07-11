using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Accord.Math;
using Accord.Math.Optimization;
using Accord.Math.Optimization.Losses;

public class CurveFit
{
    /// <summary>
    /// w[]{L,k,x0,B};
    /// </summary>
    private static Func<double[], double[], double> logistic =
        (double[] x, double[] w) => (w[0] / (1.0 + Math.Pow(Math.E, (-w[1] * (x[0] - w[2])))) + w[3]);

    private static Func<double, double[], double[]> logistic_BC =
        (double y, double[] w) => new double[] { (Math.Log((w[0] / (y - w[3])) - 1)) / (-w[1]) + w[2]};

    public enum FitModes { Logistic,Default};

    public bool Success { get; private set; }
    public double[] Prediction { get; private set; }
    public double[] Solution { get; private set; }
    public double Error { get; private set; }

    private FitModes fit_mode;
    private double[][] inputs;
    private double[] outputs;
    private Func<double[], double[], double> model;
    private Func<double[], double> objective;
    private Cobyla cobyla;
    private int para_num;
    private int iter_num;
    private Func<double, double[], double[]> model_BC;

    public CurveFit()
    {
        this.fit_mode = default;
        this.model = null;
        this.objective = null;
        this.cobyla = null;
        this.Success = false;
        this.para_num = 0;
        this.iter_num = 0;
        this.model_BC = null;
    }

    public void init_curve_fit(double[][] ip,double[] op, FitModes _fit_mode = FitModes.Default, 
                                Func<double[], double[], double> _model = null,int _para_num = 0,
                                Func<double,double[],double[]> _model_BC = null)
    {
        inputs = ip;
        outputs = op;
        switch(_fit_mode)
        {
            case FitModes.Default:
                if(_model == null)
                {
                    throw new Exception("No mode specified!!!");
                }
                else
                {
                    model = _model;
                    para_num = _para_num;
                    model_BC = _model_BC;
                }
                break;
            case FitModes.Logistic:
                model = logistic;
                para_num = 4;
                iter_num = 2000;
                model_BC = logistic_BC;
                break;
        }
        init_func();
    }

    public double[] back_cal(double y)
    {
        if(model_BC == null)
        {
            throw new Exception("No back calculation model!!");
        }
        else
        {
            return model_BC(y, Solution);
        }
    }

    public void set_bc_model(Func<double,double[],double[]> _model_BC)
    {
        model_BC = _model_BC;
    }

    public void set_iter_num(int _iter_num)
    {
        iter_num = _iter_num;
    }

    //private double[] scale_array(double[] target_arr)
    //{
    //    double[] new_arr = new double[target_arr.Length];
    //    int iter = 0;
    //    foreach (double val in target_arr)
    //    {
    //        new_arr[iter] = val / MaxNum;
    //        iter++;
    //    }
    //    return new_arr;
    //}

    private void init_func()
    {
        objective = (double[] w) =>
        {
            double sumOfSquares = 0.0;
            for (int i = 0; i < inputs.Length; i++)
            {
                double expected = outputs[i];
                double actual = model(inputs[i], w);
                sumOfSquares += Math.Pow(expected - actual, 2);
            }
            return sumOfSquares;
        };

        cobyla = new Cobyla(numberOfVariables: para_num)
        {
            Function = objective,
            MaxIterations = iter_num,
            Solution = new double[para_num] // start with some random values
        };
    }

    public bool learning()
    {
        Success = cobyla.Minimize(); // should be true

        Debug.Log("learning success " + Success);
        Solution = cobyla.Solution;

        // Get machine's predictions for inputs
        Prediction = inputs.Apply(x => model(x, Solution));

        // Compute the error in the prediction (should be 0.0)
        Error = new SquareLoss(outputs).Loss(Prediction);
        display_array("error", new double[] { Error });

        return Success;
    }

    private void display_array(string str, double[] values)
    {
        Debug.Log(str);
        int counter = 0;
        foreach (double val in values)
        {
            Debug.Log(counter.ToString() + " " + val);
            counter++;
        }

    }
}
