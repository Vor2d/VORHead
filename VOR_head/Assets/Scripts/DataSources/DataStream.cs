using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/**
 * Data Stream represents different parsers for respective hardware 
 **/
public class DataStream : MonoBehaviour {

    //stream reference for plStream
    PlStream plStream;

    //calibration values
    private Vector3 prime_position = new Vector3();
    private Vector4 calibrate_rotation = new Vector4();

    //return values
    public Vector3 position = new Vector3();
    public Vector4 rotation = new Vector4();
    public Vector3 currentPosition = new Vector3();
    public Quaternion currentRotation = new Quaternion();

    [SerializeField]
    private DataSource.Source source = DataSource.Source.Polhemus;

    

    // Use this for initialization
    void Start() {
        if ((int)source == 1)
        {
            plStream = GetComponent<PlStream>();
        }

    }

    // Update is called once per frame
    void Update() {

    }

    //used when other classes call Update or FixedUpdate method on data stream
    public void calledUpdate()
    {
        for (int i = 0; plStream != null && i < plStream.active.Length; ++i)
        {
            if (plStream.active[i])
            {

            }
        }
    }

    /**
     * TODO
     * */
    public Vector3 zero()
    {
        return zeroPolhemus();
    }

    private Vector3 zeroPolhemus()
    {
        for (var i = 0; i < plStream.active.Length; ++i)
        {
            if (plStream.active[i])
            {
                prime_position = plStream.positions[i];
                break;
            }
        }
        return prime_position;
    }


    //branches for calibrating rotation vectors
    public Vector4 calibrate()
    {
        return calibratePolhemus();
    }

    private Vector4 calibratePolhemus()
    {
        for (var i = 0; i < plStream.active.Length; ++i)
        {
            if (plStream.active[i])
            {
                calibrate_rotation = plStream.orientations[i];
                break;
            }
        }
        return calibrate_rotation;
    }

    //precursor method to set position and rotation values before reading them directly from StreamParser
    public DataPackage read()
    {
        return readPolhemus();
    }

    //reading protocol for Polhemus Device; sets currentPosition and currentRotatin values
    private DataPackage readPolhemus()
    {
        currentRotation = new Quaternion();
        Quaternion rcalibration = new Quaternion();

        int i = 0;
        /*
         transfers plstream translated bit information to unity engine parameters
         */



        position = plStream.positions[i] - prime_position;
        rotation = plStream.orientations[i] //-calibrate_rotation;
            ;

        // doing crude (90 degree) rotations into frame

        currentPosition.x = position.y;
        currentPosition.y = -position.z;
        currentPosition.z = position.x;



        currentRotation.w = rotation[0];
        currentRotation.x = -rotation[2];
        currentRotation.y = rotation[3];
        currentRotation.z = -rotation[1];
        //unity_rotation = Quaternion.Inverse(unity_rotation);

        rcalibration.w = calibrate_rotation[0];
        rcalibration.x = -calibrate_rotation[2];
        rcalibration.y = calibrate_rotation[3];
        rcalibration.z = -calibrate_rotation[1];


        //recalibrated rotation
        currentRotation = currentRotation * Quaternion.Inverse(rcalibration);

        //on return, currentPosition and currentRotation have been set as read-able values from Polhemus
        return new DataPackage(currentPosition, currentRotation);
    }

    public void OnQuit()
    {
        if ((int)source == 1)
        {
            OnPlStreamQuit();
        }
    }

    public void OnPlStreamQuit()
    {
        plStream.OnPlStreamQuit();
    }




}
