using UnityEngine;
using System.IO;
using System;
using System.Text;
using System.Threading;

public class CoilController : MonoBehaviour
{
    private bool firstBuild = true;

    //stream
    private CoilStream clstream;

    //individual components in degrees/s of the quyaternion speed vectors MFW - IT WOULD BE BETTER TO CALL THIS ANGULAR VELOCITY
    public Vector3 angularVelocityRead = new Vector3();

    //streaming time 
    public UInt32 streamSample;

    // public Quaternion oldRotation;
    private StreamWriter logfile;

    //quaternion representation of angular velocity in radians 
    public Quaternion currentRotation;

    // quaternion for zero position
    private Quaternion referenceOrientation;

    //the Vector3 degrees representation of the angular velocity taken from the differentiation of the quaternion
    //public Vector3 angularVelocity;

    public int logCounter;
    public int logCounterLimit = 14400; // 60 sec * 240 Hz
    private bool logging;
    private StringBuilder logdata;

    public int startCounter = 0;
    private bool alreadyWritten;

    //stopwatch to keep track of time through system rather than unity
    private System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
    
    public float time;

    //reference to record DynamicAcuityController decisions
    public DynamicAcuityController dynamicReference;
    public string speedEvaluationHash = null;

    // Use this for initialization
    void Awake()
    {
        // get the stream component
        clstream = GetComponent<CoilStream>();
        Application.targetFrameRate = 240;

    }

    void Start()
    {
        // initializes arrays, fixes positions
        referenceOrientation = new Quaternion();
        currentRotation = new Quaternion();

        // Initialize reference to zero position
        // This will be updated by FixedUpdate
        // not sure if this is necessary
        referenceOrientation.w = 1f;
        referenceOrientation.x = 0f;
        referenceOrientation.y = 0f;
        referenceOrientation.z = 0f;

        //counter initialization
        logCounter = 0;

        //logging  // ??
        startCounter = 0;
        // logStart = 0;
        logCounterLimit = -1; // turn off raw data logging

        logdata = new StringBuilder();
        if (logCounterLimit <= 0)
        {
            alreadyWritten = true;
            logfile = null;
        }
        else
        {
            logfile = new System.IO.StreamWriter("log.txt");  // ADD TIMESTAMP FOR UNIQUE NAME
            alreadyWritten = false;
        }

        //intializing readable quaternions
        //angularVelocity = new Vector3();

        //initialize stopwatch
        stopwatch.Start();

        //threading clauses
        threadGo = true;

    }

    //Threading utilites
    private Thread _thread1;
    private Thread _thread2;
    // private Mutex _mutex1 = new Mutex();
    // private Mutex _mutex2 = new Mutex();

    private bool threadGo = false;

    //does cleanup whenever application is closed
    void OnApplicationQuit()
    {
        Debug.Log("aborting");
        threadGo = false;
        if (logfile != null)
            logfile.Close();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("escape"))
        {
            //Application.Quit();
        }
    }



    // called before performing any physics calculations
    void FixedUpdate()
    {
        if (firstBuild)
        {
            //this.zero();
            this.calibrate();
            firstBuild = false;
            //Debug.Log("");
        }
    }


    //Main Thread
    //Called in separate process: Child of PlStream job
    public void VelocityThread2(CoilStream clstream)
    {
        //internal variables
        float deltaTime = 0f;
        double currentTimestep = 0;

        //thread variables
        // int i = 0;
        Vector4 coil_orientation;

        if (threadGo)
        {
            stopwatch.Stop();
            currentTimestep = stopwatch.Elapsed.TotalSeconds;
            stopwatch.Reset();
            stopwatch.Start();
            //deltaTime += (float)(currentTimestep);
            deltaTime = (float)currentTimestep;
            // lastTime = (float)currentTimestep;
            //Stopwatch reset


            if (deltaTime >= .001f)
            {
                
                   /*
                     transfers plstream translated bit information to unity engine parameters
                    */
                    //pol_position = plstream.positions[i] - prime_position;
                    coil_orientation = clstream.orientation; //-calibrate_rotation;

                    // Convert quaternion in coil coordinates to Unity coordinates
                    // Coil is RHR: X forward, Y left, Z up
                    // Unity is LHR: X right, Y up, Z forward
                    currentRotation.w = coil_orientation[0];
                    currentRotation.x = -coil_orientation[2]; // Unity X = -coil Y
                    currentRotation.y = coil_orientation[3];  // Unity Y = coil Z
                    currentRotation.z = coil_orientation[1];  // Unity Z = coil X
                    
                    //recalibrated rotation
                    currentRotation = currentRotation * Quaternion.Inverse(referenceOrientation);

                angularVelocityRead.x = -clstream.angularVelocity[1]; // Unity X = -coil Y
                angularVelocityRead.y = clstream.angularVelocity[2]; // Unity Y = coil Z
                angularVelocityRead.z = clstream.angularVelocity[0]; // Unity Z = coil X
                streamSample = clstream.simulinkSample;

                if ((logCounterLimit > 0) && (logCounter <= logCounterLimit))
                {
                    logPolhemusData(logdata, deltaTime);
                    logCounter++;
                }

                time = deltaTime;

                deltaTime = 0;

            }

            if ((logCounterLimit > 0) && (logCounter > logCounterLimit) && (!alreadyWritten))
            {
                logfile.WriteLine(logdata.ToString());
                alreadyWritten = true;
            }
        }
        return;
    }

    //method for writing logs into a StringBuilder; Will log all desired data that has been read from the polhemus on a frame-by-frame basis
    //method should be called once per frame as it will only write single line
    private void logPolhemusData(StringBuilder logger, float deltaTime)
    {
        // float deltaVelocity = avgMotionZ - logVelocity.z;
        logger.Append(deltaTime.ToString() + "\t");
        logger.Append(currentRotation.w + "\t");
        logger.Append(currentRotation.x + "\t");
        logger.Append(currentRotation.y + "\t");
        logger.Append(currentRotation.z + "\t");
        logger.Append(angularVelocityRead.x + "\t");
        logger.Append(angularVelocityRead.y + "\t");
        logger.Append(angularVelocityRead.z + "\t ");
        logger.AppendLine();
    }

  
  

    // sets reference orientation to current value 
    public void calibrate()
    {
        Vector4 zeroOrientation = new Vector4();
        zeroOrientation = clstream.orientation;
         
        // referenceOrientation is the quaternion of the reference (zero) position 
        referenceOrientation.w = zeroOrientation[0];
        referenceOrientation.x = -zeroOrientation[2]; // Unity Y = coil Z
        referenceOrientation.y = zeroOrientation[3];  // Unity Y = coil Z
        referenceOrientation.z = zeroOrientation[1];  // Unity Z = coil X

    }

    
    //gets the 3 component angular rotation from the Quaternion differentiation
    public Vector3 getQuaternionRotationalSpeed()
    {

        return new Vector3();
    }

    //returns the most recent velocity calculated for the three vector components
    public Vector3 getVelocity()
    {
        return angularVelocityRead;
    }

    public void QuitStream()
    {
        clstream.OnPlStreamQuit();
    }

    ////changes the velocityHistory size to desired length 
    //public void changeVelocityHistory(int size)
    //{
    //    this.velocityHistorySize = size;
    //    velocityReadHistory = new Queue<Vector3>();
    //}


}