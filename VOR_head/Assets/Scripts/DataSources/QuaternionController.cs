using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

//Main Controller for grabbing Quaternion values from Polhemus or VR system
//***Must be combined with respective _____Stream and DataStream scripts in the same object to function***
public class QuaternionController : MonoBehaviour
{
    //optional sliders
    public Slider divisor_slider;
    public Text divisor_value;
    public Slider sensors_slider;
    public Text sensors_value;

    //optional displays
    public Text displayValue;
    public Text fpsDisplay;


    //Test locationa and rotation
    public Vector3 location;
    public Quaternion rotation;
    private bool firstBuild = true;

    //stream
    private DataStream dataStream;

    //calibration values
    private Vector3 prime_position;
    private Vector4 calibrate_rotation;

    //public field that can be accessed by other classes
    public Vector3 angularSpeed; //regular angular speed calculated from direct Euler function
    public Quaternion quaternionSpeed; //radians Quaternion based  rotational speed average at any given time
    public Vector3 eulerQuaternionSpeed; //the Quaternion based rotational speed average at any given frame

    //individual components in degrees of the quaternion speed vectors
    public Vector3 angularVelocityRead = new Vector3();

    //averages of motions defined by the queue
    public int avgNumber = 3  ;

    public float avgMotionX;
    public float avgMotionY;
    public float avgMotionZ;

    private Queue<float> avgMotionXQueue;
    private Queue<float> avgMotionYQueue;
    private Queue<float> avgMotionZQueue;

    public Vector3 accelVector;

    //threshold of past data set
    public float thresholdLimit = 150.0f;
    public float accelLimit = 50f;

    public float thresholdX;
    public float thresholdY;
    public float thresholdZ;

    private float lastMotionX;
    private float lastMotionY;
    private float lastMotionZ;


    //internal storage of past iteration's calculated values
    public Vector3 oldPosition;
    public Quaternion oldRotation;
    private StreamWriter file;

    //Quaternion angular change; Read quaternion Read with mouse movement changes
    public Quaternion quaternionRead;

    //quaternion representation of angular velocity in radians 
    public Quaternion diffQuaternion;

    //the Vector3 degrees representation of the angular velocity taken from the differentiation of the quaternion
    public Vector3 angularVelocityQuaternion;

    private float previousTime;
    private float[] cumulTime;
    private int counter;
    public int counterLimit = 10;


    public int logCounter;
    public int logCounterLimit = 0;
    private bool logging;
    private StringBuilder logdata;

    public int startCounter = 0;
    private int logStart;
    private bool alreadyWritten;
    private int firstThreshold = 0;

    //stopwatch to keep track of time through system rather than unity
    private System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
    private System.Diagnostics.Stopwatch stopwatch2 = new System.Diagnostics.Stopwatch();


    public float time;
    private float lastTime;

    //deltaTime history parameters
    private Queue<float> deltaTimeHistory = new Queue<float>();
    private int deltaTimeHistorySize = 5;
    private Queue<Vector3> velocityReadHistory = new Queue<Vector3>();
    public int velocityHistorySize = 10;
    //delays first x amount of readings from polhemus to allow synchronization
    private int filterTimeDelayCounter = 0;
    private int filterTimeDelayAmount = 100;
    private int filterTimeCounter = 0;
    private int filterTimeAmount = 2;
    [SerializeField]
    private float filterTimeErrorAllowance = .0005f;

    //records past velocity for logging purposes
    private Vector3 logVelocity = new Vector3();

    //reference to record DynamicAcuityController decisions
    public DynamicAcuityController dynamicReference;
    public string speedEvaluationHash = null;



    // Use this for initialization
    void Awake()
    {
        // set divisor defaults
        //divisor_slider.value = 1;

        // set sensors defaults
        //sensors_slider.value = 1;

        // get the stream component
        dataStream = GetComponent<DataStream>();
        Application.targetFrameRate = 240;

        // set sensors_slider max value
        //sensors_slider.maxValue = dataStream.active.Length;
    }

    void Start()
    {
        // initializes arrays, fixes positions
        calibrate_rotation = new Vector4();
        oldPosition = new Vector3();
        oldRotation = new Quaternion();


        //counter initializations
        counter = 0;
        logCounter = 0;
        filterTimeDelayCounter = 0;

        previousTime = 0;
        cumulTime = new float[counterLimit];
        deltaTimeHistory = new Queue<float>();
        velocityReadHistory = new Queue<Vector3>();

        //logging
        startCounter = 0;
        logStart = 0;

        logdata = new StringBuilder();
        if (logCounterLimit <= 0)
        {
            alreadyWritten = true;
            file = null;
        }
        else
        {
            file = new System.IO.StreamWriter("log.txt");
            alreadyWritten = false;
        }


        //intializing readable quaternions
        quaternionRead = new Quaternion();
        quaternionSpeed = new Quaternion();
        angularVelocityQuaternion = new Vector3();
        diffQuaternion = new Quaternion();

        //averaging speeds
        avgMotionXQueue = new Queue<float>();
        avgMotionYQueue = new Queue<float>();
        avgMotionZQueue = new Queue<float>();

        accelVector = new Vector3();

        lastMotionX = 0;
        lastMotionY = 0;
        lastMotionZ = 0;

        //initialize stopwatch
        stopwatch.Start();
        time = 0f;
        lastTime = 0f;



        //threading clauses
        threadGo = true;
        firstThreshold = 0;


        /*
        _thread1 = new Thread(() => VelocityThread2(dataStream));
        _thread1.Start();

        //_thread2 = new Thread(() => LoggerThread());
        //_thread2.Start();
        */

    }

    //Threading utilites
    private Thread _thread1;
    private Thread _thread2;
    private Mutex _mutex1 = new Mutex();
    private Mutex _mutex2 = new Mutex();

    private bool threadGo = false;

    //Stacks for Threads to execute on

    //for Quaternion represented float Queues, the queue is treated as a 4 byte struct for data where the 4 elements are w,x,y,z in order.
    //NOTE: current representation in object Quaternions
    /*
    private Queue<Quaternion> currentRotationQueue = new Queue<Quaternion>();
    private Queue<Quaternion> oldRotationQueue = new Queue<Quaternion>();
    */
    //stores indices of polhemus device to read
    private Queue<int> readPlStreamQueue = new Queue<int>();

    private Queue<int> indexQueue = new Queue<int>();
    private Queue<float> thread2DeltaTimeQueue = new Queue<float>();
    private Queue<Vector4> polRotationQueue = new Queue<Vector4>();



    //does cleanup whenever application is closed
    void OnApplicationQuit()
    {
        Debug.Log("aborting");
        threadGo = false;
        if (file != null)
            file.Close();
        /*
        if(_thread1 != null)
             _thread1.Abort();
        if(_thread2 != null)
            _thread2.Abort();
        if (file != null)
            file.Close();
        */

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


        // update divisor text
        //divisor_value.text = divisor_slider.value.ToString("F1");
        if (firstBuild && dataStream != null)
        {
            prime_position = dataStream.zero();
            calibrate_rotation = dataStream.calibrate();
            firstBuild = false;
         }
    }



    //gets the change of the quaternion over time 
    private Quaternion quaternionDerivative(Quaternion newQ, Quaternion oldQ, float deltaTime)
    {
        float oldw = oldQ.w;
        float oldx = oldQ.x;
        float oldy = oldQ.y;
        float oldz = oldQ.z;

        float newW = newQ.w;
        float newx = newQ.x;
        float newy = newQ.y;
        float newz = newQ.z;

        float diffw = 2 * (newW - oldw) / deltaTime;
        float diffx = 2 * (newx - oldx) / deltaTime;
        float diffy = 2 * (newy - oldy) / deltaTime;
        float diffz = 2 * (newz - oldz) / deltaTime;


        Quaternion dQ = new Quaternion(diffw, diffx, diffy, diffz);
        quaternionRead = dQ;

        return dQ;
    }

    //gets the timed average of the quaternion set it is given; 
    //calculates the average based on the new quaternion speed it is given at the current instance
    //form of euler angle returned is the angular velocity in degrees
    private Vector3 averageQuaternionSpeed(Vector3 newQ, Stack<Vector3> quaternionHistory)
    {
        quaternionHistory.Pop();
        quaternionHistory.Push(newQ);


        float totalSpeedsX = 0f;
        float totalSpeedsY = 0f;
        float totalSpeedsZ = 0f;
        Stack<Vector3> temp = new Stack<Vector3>(quaternionHistory);
        foreach (Vector3 unit in temp)
        {
            totalSpeedsX += unit.x * Mathf.Rad2Deg;
            totalSpeedsY += unit.y * Mathf.Rad2Deg;
            totalSpeedsZ += unit.z * Mathf.Rad2Deg;
        }
        float avgX = totalSpeedsX / temp.Count;
        float avgY = totalSpeedsY / temp.Count;
        float avgZ = totalSpeedsZ / temp.Count;

        return new Vector3(avgX, avgY, avgZ);
    }


    //dataPackage that contains position and rotation vectors converted from DataSream source
    private DataPackage dataPackage = new DataPackage(new Vector3(), new Quaternion());

    //Main Thread
    //Called in seperate process: Child of dataStream job
    public void VelocityThread2(DataStream dataStream)
    {
        //internal variables
        float deltaTime = 0f;
        double currentTimestep = 0;

        //thread variables
        int i = 0;
        Vector3 pol_position;
        Vector4 pol_rotation;
        Vector3 currentPosition;

        Quaternion currentRotation = new Quaternion();
        Quaternion rcalibration = new Quaternion();

        Quaternion diffQuaternion;

        if (threadGo && dataStream != null)
        {
            stopwatch.Stop();
            currentTimestep = stopwatch.Elapsed.TotalSeconds;
            stopwatch.Reset();
            stopwatch.Start();
            //deltaTime += (float)(currentTimestep);
            deltaTime = (float)currentTimestep;
            lastTime = (float)currentTimestep;
            //Stopwatch reset


            if (deltaTime >= .001f)
            {
                if (firstThreshold > 10)
                {
                    //filters extraneous deltaTimes and normalizes the distribution based on the deltaTime history
                    deltaTime = filterDeltaTime(deltaTime);

                    //pulls data of rotation and position from DataPackage
                    dataPackage = dataStream.read();
                    currentPosition = dataPackage.position;
                    currentRotation = dataPackage.rotation;

                    //Quaternion differentiation
                    diffQuaternion = quaternionDerivative(currentRotation, oldRotation, deltaTime);
                    diffQuaternion = diffQuaternion * Quaternion.Inverse(currentRotation);

                    //d q(t) /dt = ½ * W(t) q(t)
                    //The equation for angular velocity from quaternions is 
                    // W(t)  = 2 *  d  q(t)/dt * q^-1(t)
                    //where W(t) is teh angular veloctiy
                    angularVelocityRead.x = (diffQuaternion.x * Mathf.Rad2Deg);
                    angularVelocityRead.y = (diffQuaternion.y * Mathf.Rad2Deg);
                    angularVelocityRead.z = (diffQuaternion.z * Mathf.Rad2Deg);

                    //writes acceleration components into accelVector
                    recordAccelerationVector(deltaTime);


                    //getThresholdMotionValues();
                    //checkAccelerationLimit();


                    //initializes avgMotionx on first run

                    if (avgMotionX == 0)
                    {
                        avgMotionX = angularVelocityRead.x;
                        avgMotionY = angularVelocityRead.y;
                        avgMotionZ = angularVelocityRead.z;
                    }

                    //adds the threshold values to be averaged

                    if (avgMotionXQueue.Count >= avgNumber)
                    {
                        avgMotionXQueue.Dequeue();
                        avgMotionYQueue.Dequeue();
                        avgMotionZQueue.Dequeue();
                    }
                    avgMotionXQueue.Enqueue(angularVelocityRead.x);
                    avgMotionYQueue.Enqueue(angularVelocityRead.y);
                    avgMotionZQueue.Enqueue(angularVelocityRead.z);

                    //collects history for 
                    if(velocityReadHistory.Count >= velocityHistorySize)
                         velocityReadHistory.Dequeue();
                    velocityReadHistory.Enqueue(new Vector3(angularVelocityRead.x, angularVelocityRead.y, angularVelocityRead.z));

                    //records past velocity for 
                    if (velocityReadHistory.Count >= 3)
                        logVelocity = getPastVelocity();

                    //final call to update avgMotion X, Y,Z
                    if (avgMotionXQueue.Count >= 3)
                        getMedianMotionValuesFromThreshold();


                    //update old values
                    oldRotation = currentRotation;


                    Vector3 pastVelocity = getPastVelocity();
                    //Debug
                    if ((logCounterLimit > 0) && (logCounter <= logCounterLimit))
                    {
                        //logdata.AppendLine(deltaTime.ToString() + " " + thresholdX + " " + thresholdY + " " + thresholdZ);
                        //logdata.AppendLine(deltaTime.ToString() + " " + accelVector.x + " " + accelVector.y + " " + accelVector.z);    
                        logPolhemusData(logdata, deltaTime);
                        logCounter++;
                    }

                    time = deltaTime;

                    deltaTime = 0;
                }
                else
                {
                    lastMotionX = 0;
                    lastMotionY = 0;
                    lastMotionZ = 0;

                    thresholdX = 0;
                    thresholdY = 0;
                    thresholdZ = 0;

                    firstThreshold++;
                }
            }


            if ((logCounterLimit > 0) && (logCounter > logCounterLimit) && (!alreadyWritten))
            {
                file.WriteLine(logdata.ToString());
                alreadyWritten = true;
            }
        }
        return;
    }

    //method for writing logs into a StringBuilder; Will log all desired data that has been read from the polhemus on a frame-by-frame basis
    //method should be called once per frame as it will only write single line
    private void logPolhemusData(StringBuilder logger, float deltaTime)
    {
        float deltaVelocity = avgMotionZ - logVelocity.z;
        logger.Append(deltaTime.ToString() + " ");
        logger.Append(avgMotionX + " ");
        logger.Append(avgMotionY + " ");
        logger.Append(avgMotionZ + " ");
        /*
        logger.Append(logVelocity.x + " ");
        logger.Append(logVelocity.y + " ");
        logger.Append(logVelocity.z + " ");
        */
        logger.Append(deltaVelocity + " ");
        
        if(dynamicReference != null)
        {
            if(speedEvaluationHash != null)
            {
                logger.Append(speedEvaluationHash);
                speedEvaluationHash = null;
            }
        }
        logger.AppendLine();
    }

    //averages the velocities thorugh a extrapolated difference through the mean of their accelerations
    private void getAverageMotionValues()
    {
        Queue<float>[] queues = { avgMotionXQueue, avgMotionYQueue, avgMotionZQueue };
        float cumulative = 0f;
        float last = 0f;
        int j = 0;

        //calculates cumulative accelerations for each component
        for (int i = 0; i < queues.Length; i++)
        {
            j = 0;
            foreach (float motion in queues[i])
            {
                if (j == 0)
                {
                    last = motion;
                }
                else
                {
                    cumulative += (last - motion);
                    last = motion;

                }
                j++;
            }
            //uses average acceleration to determine next avgMotion
            cumulative = (cumulative) / (avgNumber - 1);
            switch (i)
            {
                case 0:
                    avgMotionX = avgMotionX + cumulative;
                    break;
                case 1:
                    avgMotionY = avgMotionY + cumulative;
                    break;
                case 2:
                    avgMotionZ = avgMotionZ + cumulative;
                    break;
                case 3:
                    break;


            }
            cumulative = 0f;
        }
    }


    float median;
    List<float> medianList;
    float[] medianElements;

    //gets velocity averages as median
    private void getMedianMotionValuesFromThreshold()
    {
        Queue<float>[] queues = { avgMotionXQueue, avgMotionYQueue, avgMotionZQueue };
        median = 0f;
        int index;

        for (int i = 0; i < queues.Length; i++)
        {
            medianList = new List<float>();
            medianElements = queues[i].ToArray();

            //Pre-sort array
            medianList.AddRange(medianElements);
            medianList.Sort();
            index = medianList.Count / 2;
            medianElements = medianList.ToArray();
            median = medianElements[index];

            //calculate the median from sorted array
            if (medianList.Count % 2 == 0)
            {
                median += medianList.ToArray()[index - 1];
                median = median / 2;
            }

            //create velocity component for each vector
            switch (i)
            {
                case 0:
                    avgMotionX = median;
                    break;
                case 1:
                    avgMotionY = median;
                    break;
                case 2:
                    avgMotionZ = median;
                    break;
                case 3:
                    break;


            }
        }
    }

    //uses the Queue of average Motions to find the average Acceleration component
    //uses the most recent values in the Queue
    private void recordAccelerationVector(float deltaTime)
    {
        float accelx = (angularVelocityRead.x - lastMotionX)/deltaTime;
        float accely = (angularVelocityRead.y - lastMotionY)/deltaTime;
        float accelz = (angularVelocityRead.z - lastMotionZ)/deltaTime;
        accelVector = new Vector3(accelx, accely, accelz);
  
    }

    //filters extraneous deltaTimes and normalizes the distribution based on the deltaTime history
    private float filterDeltaTime(float currentDeltaTime)
    {
        
        //breaks if during first x amount of delayed frames for synchronization/stablization of readings
        if(filterTimeDelayCounter < filterTimeDelayAmount)
        {
            recordDeltaTime(currentDeltaTime);
            filterTimeDelayCounter++;
            return currentDeltaTime;
        }

        //makes sure deltaTima is within expected bounds
        float deltaTime = filterDeltaTimeValue(currentDeltaTime);


        return deltaTime;
    }


    //add currentDeltaTime into the deltaTimeHistory Queue 
    private void recordDeltaTime(float currentDeltaTime)
    {
        deltaTimeHistory.Enqueue(currentDeltaTime);
        if (deltaTimeHistory.Count > deltaTimeHistorySize)
            deltaTimeHistory.Dequeue();
    }

    //filters currentDeltaTime if not within the range of the average of the deltaTimeHistory
    private float filterDeltaTimeValue(float currentDeltaTime)
    {
        float deltaTime = getAverageDeltaTime();
        if (filterTimeCounter <= 0 && (Mathf.Abs(currentDeltaTime - deltaTime) > filterTimeErrorAllowance))
        {
            filterTimeCounter = filterTimeAmount;
        }


        if (filterTimeCounter > 0)
        {
            recordDeltaTime(deltaTime);
            filterTimeCounter--;
            return deltaTime;
        }
        else {
            recordDeltaTime(currentDeltaTime);
            return currentDeltaTime;
        }

       
    }

    //returns the mean value from the values in deltaTimeHistory
    private float getAverageDeltaTime()
    {
        if (deltaTimeHistory.Count == 0)
            return 0f;
        float averageTime = 0f;
        foreach(float time in deltaTimeHistory)
        {
            averageTime+= time;
        }
        return averageTime/deltaTimeHistory.Count;
    }

    //caps velocity recordings to specified limit of changes (limit by accelerations)
    private void getThresholdMotionValues()
    {

        Queue<float>[] queues = { avgMotionXQueue, avgMotionYQueue, avgMotionZQueue };
        float threshold = 0f;
        int j = 0;
        for (int i = 0; i < queues.Length; i++)
        {

            foreach (float motion in queues[i])
            {
                if (j >= queues[i].Count / 2)
                {
                    if ((motion > 0) && (motion > threshold))
                        threshold = motion;
                    else if (motion < threshold)
                        threshold = motion;
                }
                j++;
            }

            //gets limit of threshold
            if (threshold > 0)
            {
                threshold = Math.Min(threshold, thresholdLimit);
            }
            else
            {
                threshold = Math.Max(threshold, -thresholdLimit);
            }

            switch (i)
            {
                case 0:
                    thresholdX = threshold;
                    break;
                case 1:
                    thresholdY = threshold;
                    break;
                case 2:
                    thresholdZ = threshold;
                    break;
                case 3:
                    break;
            }
            threshold = 0f;
            j = 0;
        }
    }

    //converts motion values back to old storages
    private void checkAccelerationLimit()
    {
        assignMotion(ref thresholdX, ref lastMotionX, angularVelocityRead.x);
        assignMotion(ref thresholdY, ref lastMotionY, angularVelocityRead.y);
        assignMotion(ref thresholdZ, ref lastMotionZ, angularVelocityRead.z);
    }

    //puts the threshold values from limiting the accelerations
    private void assignMotion(ref float thresholdVal, ref float lastMotion, float motion)
    {
        float difference = motion - lastMotion;
        if (Math.Abs(difference) > accelLimit)
        {
            if (difference > 0)
                lastMotion += accelLimit;
            else
                lastMotion -= accelLimit;
            thresholdVal = lastMotion;
        }
        else {
            thresholdVal = lastMotion = motion;
        }
    }

    //fixes position to all 0's
    public void zero()
    {

        prime_position = dataStream.zero();
    }


    //fixes rotation to zero in difference
    public void calibrate()
    {
        calibrate_rotation = dataStream.calibrate();
    }

    
    //changes the velocityHistory size to desired length 
    public void changeVelocityHistory(int size)
    {
        this.velocityHistorySize = size;
        velocityReadHistory = new Queue<Vector3>();
    }


    /*
     * Getter Methods 
     * */


    //gets angular speed using the quaternion differentiation
    public Vector3 getEulerRotationalSpeed()
    {
        return this.eulerQuaternionSpeed;
    }

    //gets the fixed position of the polhemus tracker
    public Vector3 getPosition()
    {
        return this.oldPosition;
    }

    //gets the Quaternion Rotation of the polhemus tracker
    public Quaternion getRotation()
    {
        return oldRotation;
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

    //returns the median velocity average for each of the three vector components
    public Vector3 getAverageVelocity()
    {
        return new Vector3(avgMotionX, avgMotionY, avgMotionZ);
    }

    //returns the most recent acceleration calculated for teh three vector components
    public Vector3 getAccelerationVector()
    {
        return accelVector;
    }

    //returns set of velocity history Queue from velocityHistorySize frames back
    public Queue<Vector3> getVelocityHistory()
    {
        return velocityReadHistory;
    }

    //returns single velocity from velocityReadHistory from velocityHistorySize frames back
    public Vector3 getPastVelocity()
    {
        if (velocityReadHistory != null && velocityReadHistory.Count > 0)
            return velocityReadHistory.Peek();
        else
            return new Vector3();
    }


    public void QuitStream()
    {
        dataStream.OnQuit();
    }

    
}