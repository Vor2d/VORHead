using UnityEngine;
using System.IO;
using System;
using System.Text;
using System.Threading;
using UnityEngine.XR;
using System.Collections;

public class VRController : MonoBehaviour
{
    private bool firstBuild = true;

    //stream
    private VRStream vrStream;
    OVRDisplay Display = new OVRDisplay();

    //individual components in degrees/s of the quyaternion speed vectors MFW - IT WOULD BE BETTER TO CALL THIS ANGULAR VELOCITY
    public Vector3 angularVelocityRead = new Vector3();
    public Vector3 angularVelocityLast = new Vector3(0,0,0);
    //public float angularAcc;  
    //public ArrayList countback = new ArrayList();

    //streaming time 
    public UInt32 streamSample;

    StreamWriter file;
    //quaternion representation of angular velocity in radians 
    public Quaternion currentRotation;

    // quaternion for zero position
    private Quaternion referenceOrientation;

    //the Vector3 degrees representation of the angular velocity taken from the differentiation of the quaternion
    //public Vector3 angularVelocity;


    public int startCounter = 0;
    private bool alreadyWritten;

    //stopwatch to keep track of time through system rather than unity
    private System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();

    public float time;

    //reference to record DynamicAcuityController decisions
    public DynamicAcuityController dynamicReference;
    public string speedEvaluationHash = null;
    StringBuilder logger = new StringBuilder();

    // Use this for initialization
    void Awake()
    {
        file = new StreamWriter("log.txt");
        // get the stream component
        vrStream = GetComponent<VRStream>();
        dynamicReference = GetComponent<DynamicAcuityController>();
    }

    void Start()
    {
        // initializes arrays, fixes positions
        referenceOrientation = new Quaternion();
        currentRotation = new Quaternion();

        // Initialize reference to zero position
        // This will be updated by FixedUpdate
        //intializing readable quaternions
        //angularVelocity = new Vector3();
        //threading clauses
        threadGo = true;
        stopwatch.Start();

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
       // file.WriteLine(logger.ToString());
        Debug.Log("aborting");
        threadGo = false;
        if (file != null)
        {
            file.Close();
            Debug.Log("File Closed");
        }
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
        currentRotation = InputTracking.GetLocalRotation(XRNode.Head);
        if (firstBuild)
        {
            //this.zero();
            //this.calibrate();
            firstBuild = false;
        }
    }


    //Main Thread
    //Called in separate process: Child of PlStream job
    public void VelocityThread2()
    {
        //internal variables

        //thread variables
        // int i = 0;
        if (threadGo)
        {
                //deltaTime += (float)(currentTimestep);
                // lastTime = (float)currentTimestep;
                //Stopwatch reset
            angularVelocityRead = OVRPlugin.GetNodeAngularVelocity(OVRPlugin.Node.Head, OVRPlugin.Step.Render).FromFlippedZVector3f() * Mathf.Rad2Deg;
            //countback.Enqueue(angularVelocityRead);
            //Debug.Log(countback.Count);

            //Debug.Log(String.Format("velocity: {0}", angularVelocityRead));
            //Debug.Log("Before Record");
            if (angularVelocityRead != angularVelocityLast)
            {
                stopwatch.Stop();
                logger.Append(dynamicReference.fixTime + ", ");
                logger.Append(angularVelocityRead.x + ", ");
                logger.Append(angularVelocityRead.y + ", ");
                logger.Append(angularVelocityRead.z + ", ");
                logger.Append(stopwatch.Elapsed.TotalSeconds.ToString("F6"));
                //logger.Append(Display.latency.render);
                logger.AppendLine();
                //Debug.Log("Record Success");
                //file.WriteLine(angularVelocityRead.magnitude + "," + angularVelocityRead.x + "," + angularVelocityRead.y + "," + angularVelocityRead.z + "," + stopwatch.Elapsed.TotalSeconds);

                //angularAcc = Display.angularAcceleration.magnitude;
                //countback.Add(angularVelocityRead.magnitude);
                /*if (countback.Count >= 21)
                {
                //countback.Dequeue();
                countback.RemoveAt(0);
                //Debug.Log(String.Format("count size{0}, count last element{1}",countback.Count,countback[9]));
                }*/
                //Debug.Log(String.Format("velocity: {0}, {1}", angularVelocityRead, stopwatch.Elapsed.TotalSeconds));
                angularVelocityLast = angularVelocityRead;
                stopwatch.Reset();
                stopwatch.Start();
            }
        }
    }

    //method for writing logs into a StringBuilder; Will log all desired data that has been read from the polhemus on a frame-by-frame basis
    //method should be called once per frame as it will only write single line



    // sets reference orientation to current value 
    /*public void calibrate()
    {
        Vector4 zeroOrientation = new Vector4();
        zeroOrientation = vrStream.orientation;

        // referenceOrientation is the quaternion of the reference (zero) position 
        referenceOrientation.w = zeroOrientation[0];
        referenceOrientation.x = -zeroOrientation[2]; // Unity Y = coil Z
        referenceOrientation.y = zeroOrientation[3];  // Unity Y = coil Z
        referenceOrientation.z = zeroOrientation[1];  // Unity Z = coil X

    }*/


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

    /*public float GetAcceleration()
    {
        if(countback.Count >= 20)
        {
            float First = (float)countback[0];
            float Last = (float)countback[19];
            //Debug.Log(String.Format("First{0}, Last{1}", First, Last));
            return (First - Last);
        }
        else
        {
            return 0;
        }
    }*/
    /*public void calibrate()
    {
        InputTracking.Recenter();
    }*/
        
    public IEnumerator logSpeed(bool judge)
    {
        file.WriteLine(logger.ToString());
        file.Close();
        logger = new StringBuilder();
        yield return new WaitForSeconds(.1f);
    }
    public void QuitStream()
    {
        Debug.Log("aborting");
        //file.WriteLine(logger.ToString());
        threadGo = false;
        if (file != null)
        {
            file.Close();
        }
    }

    /* public void QuitStream()
     {
         clstream.OnPlStreamQuit();
     }*/

    ////changes the velocityHistory size to desired length 
    //public void changeVelocityHistory(int size)
    //{
    //    this.velocityHistorySize = size;
    //    velocityReadHistory = new Queue<Vector3>();
    //}


}
