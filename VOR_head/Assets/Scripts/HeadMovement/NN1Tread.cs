using System.Threading;
using UnityEngine;
using HMTS_enum;
using System;

public class NN1Tread : MonoBehaviour
{
    [SerializeField] private CoilData CD_script;
    [SerializeField] private HeadSimulator HS_script;
    [SerializeField] private EyeIndex EyeTrainIndex;
    
    private Thread NNThread;

    public double Error_rate { get; private set; }
    private AForge.Neuro.ActivationNetwork NNetwork;
    private AForge.Neuro.Learning.BackPropagationLearning BPLearner;
    private double[] HV_E_voltage;
    private double[] HV_H_degree;
    private bool running_flag;


    NN1Tread()
    {
        this.NNThread = new Thread(trainning);

        this.NNetwork = 
            new AForge.Neuro.ActivationNetwork(new AForge.Neuro.SigmoidFunction(), 2, 2);
        this.BPLearner = new AForge.Neuro.Learning.BackPropagationLearning(NNetwork);
        this.running_flag = false;
        this.HV_E_voltage = new double[2] { 0.0d, 0.0d };
        this.HV_H_degree = new double[2] { 0.0d, 0.0d };
        this.Error_rate = 0.0d;
    }

    public void start_training()
    {
        running_flag = true;
        NNThread.Start();
    }

    public void stop_trainning()
    {
        running_flag = false;
    }

    public AForge.Neuro.ActivationNetwork get_network()
    {
        return NNetwork;
    }

    private void trainning()
    {
        try
        {
            while (running_flag)
            {
                switch (EyeTrainIndex)
                {
                    case EyeIndex.left:
                        HV_E_voltage[0] = (double)CD_script.Left_eye_voltage.x;
                        HV_E_voltage[1] = (double)CD_script.Left_eye_voltage.y;
                        break;
                    case EyeIndex.right:
                        HV_E_voltage[0] = (double)CD_script.Right_eye_voltage.x;
                        HV_E_voltage[1] = (double)CD_script.Right_eye_voltage.y;
                        break;
                }
                HV_H_degree[0] = -HS_script.TrueHeadRR.y;
                HV_H_degree[1] = -HS_script.TrueHeadRR.x;
                Error_rate = BPLearner.Run(HV_E_voltage, HV_H_degree);
            }
        }
        catch(Exception e)
        {
            Debug.Log(e);
            Debug.Log("NN1Tread terminated!");
        }
        
    }

    private void OnApplicationQuit()
    {
        try
        {
            // signal shutdown
            running_flag = false;

            // attempt to join for 500ms
            if (!NNThread.Join(500))
            {
                // force shutdown
                NNThread.Abort();
            }
        }
        catch (Exception e)
        {
            Debug.Log(e);
            Debug.Log("unable to close the connection thread upon application exit. This is not a critical exception.");
        }
    }

    private void OnDestroy()
    {
        try
        {
            // signal shutdown
            running_flag = false;

            // attempt to join for 500ms
            if (!NNThread.Join(500))
            {
                // force shutdown
                NNThread.Abort();
            }
        }
        catch (Exception e)
        {
            Debug.Log(e);
            Debug.Log("unable to close the connection thread upon application exit. This is not a critical exception.");
        }
    }
}
