using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.XR;
//top level wrapper class to retrieve all data information from any data stream typing
//*** To Use Instructions:
//Put in same object as the VRController, CoilController, or Quaternion Controller Script 
//in the scene, then set the source field in the script to correct type: VR, Coil, or Quaternion respectively
public class DataSource : MonoBehaviour {
    QuaternionController qController;
    CoilController coilController;
    VRController vrController;

    //TODO insert callers to retrieve values from assigned controller
    public Vector3 angularVelocityRead = new Vector3();
    public Quaternion currentRotation = new Quaternion();
    public DynamicAcuityController dynamicReference;
    public UInt32 streamSample;
    [SerializeField]
    private Source source = Source.VR;
    private int resource = 0;

    //variables used for loggin
    public string speedEvaluationHash = null;




    // Use this for initialization
    void Start () {
        initializeSource(source);
    }

    public void initializeSource(Source sourceType)
    {
        if (source == Source.VR)
            vrController = GetComponent<VRController>();
        else if (source == Source.Coil)
            coilController = GetComponent<CoilController>();
        else if (source == Source.Polhemus)
            qController = GetComponent<QuaternionController>();
    }
	
	// Update is called once per frame
	void Update () {
        pullVRControllerData();
	}

    void pullQuaternionControllerData()
    {
        currentRotation = qController.getRotation();
        angularVelocityRead = qController.angularVelocityRead;
        qController.speedEvaluationHash = speedEvaluationHash; 
    }

    void pullCoilControllerData()
    {
        currentRotation = coilController.currentRotation;
        angularVelocityRead = coilController.angularVelocityRead;
        coilController.speedEvaluationHash = speedEvaluationHash;
    }

    void pullVRControllerData()
    {
        currentRotation = vrController.currentRotation;
        angularVelocityRead = vrController.angularVelocityRead;
        vrController.speedEvaluationHash = speedEvaluationHash;
    } 
    public void QuitStream()
    {
        if (source == Source.Polhemus)
        {
            qController.QuitStream();
        }
        else if (source == Source.Coil)
        {
            coilController.QuitStream();
        }
        else if (source == Source.VR)
        {
            vrController.QuitStream();
        }
    }


    public void calibrate()
    {
        if (source == Source.Polhemus)
        {
            qController.calibrate();
        }
        else if (source == Source.Coil)
        {
            coilController.calibrate();
        }
        else if (source == Source.VR)
        {
            InputTracking.Recenter();
        }
    }


    public enum Source
    {
        None = 0,  Polhemus = 1, Coil = 2, VR = 3
    }
}
