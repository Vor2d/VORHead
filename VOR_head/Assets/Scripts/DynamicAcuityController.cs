using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Text;
using System.IO;


public class DynamicAcuityController : MonoBehaviour
{

    //references to Unity Values
    private static readonly float rotationUpperBound = 360f;

    //mode to use the script in
    [SerializeField]
    private EnumTypes.GameMode gameMode;

    //Most important: Connection to the data stream
    static public DataSource dataSource;  // get Controller script for streamed coil data
    public VRController vrController;



    // Public Variables
    public float speedThreshold;
    public float speedDifferenceMax = 40;
    public float speedDifferenceMin = 25;
    public GameObject background;
    public GameObject gloves;
    public GameObject soccer;
    public GameObject dot;
    public GameObject canvas;
    public Text savesText;
    public Text goalsText;
    public Text response;
    public Text warning;
    public int init;
    public string fixTime = "0.00";
    

    //public setttings for new trial head recentering windows
    public float headRecenteringThreshold = 5f;
    public float evaluationRotation = 0f;


    // Private Variables
    private float headSpeed; // Rotational head speed in desired head rotation direction
    private float lookbackSpeed; // lookback speed in desired head rotation direction

    // private float initSpeedThresMin;
    // private float initSpeedThresMax;
    // private float speedThresStepMin;
    // private float speedThresStepMax;
    // private float lastSpeedThresMin;
    // private float lastSpeedThresMax;
    private float lastOptotypeSize;
    private float decimalScale;    
    private float optotypeShowDelayTime;
    private Vector3 angularVelocity;

    // translationOnScreen - Linear distance on screen that corresponds to current head orientation
    private Vector3 translationOnScreen = Vector3.zero; 
    private float pixels2World; // used for calculating translationOnScreen
    private const int optotypeNumber = 4;
    private MeshRenderer[] meshRenderer; // array of MeshRenderers for all elements in Optotype prefab
    private bool showTestInfo = false;
    private bool evaluationStart = false; // For enable/disable test mode
    private bool paused = false; // For enable/disable input
    private bool DVATestStart = false;
    private string testInfo = "";
    
    //Persistent Variables
    public static float optotypeSize = 0;
    public static float logmarScale = 1.0f;
    //public PreferenceLoader pl;

    //TODO 
    //opptotype[randomkey] represents the direction/orientation of the correct choice for the current trial
    private KeyCode[] optotypeDirection = new KeyCode[optotypeNumber];
    private int randomKey = 0;
    private bool showOptionButtons = false;
    private bool animationIsOn = false;
    private bool showHUD = true;
    private float ballAnimationTime = 2.5f;
    private float moveBackTime = .5f;
    private int saves;
    private int goals;
    private PreferenceLoader pl;
    private KeyCode keyClicked;
    private Vector3 lastMousePosition = Vector3.zero;
    private ArrayList directionIndicator;
    private string directionIndicatorLabel;
    private int optotypeShowTimesRemaining;
    private Vector3 glovesInitPosition;
    private float ballSpeed = 5f;
    private bool movebackFinish = false; // indicated whether a moveBack() coroutine is finished
    private bool recalibrateTimeUp = false; // check if time is up in recalibrate() function
    private float bestDVA;
    private int bestDVACounter;
    private int bestDVAReset;
    private float backgroundScale;
    private string info;
    private bool arrowDisappear;
    private float accelerationComponent;
    // private float maxHeadSpeed;
    // private float maxHeadAcc;

    // Buttons
    // for single screen play
    /*
    private Rect RectMainMenu = new Rect(Screen.width / 30, Screen.height / 25, Screen.width / 15, Screen.height / 20); 
    private Rect RectRecalibrate = new Rect(Screen.width / 30 + Screen.width / 15 + 10, Screen.height / 25, Screen.width / 15, Screen.height / 20);
    private Rect RectLogResult= new Rect(Screen.width / 30 + Screen.width * 2 / 15 + 20, Screen.height / 25, Screen.width / 15, Screen.height / 20);
    */
    private Rect RectMainMenu = new Rect(Screen.width / 2 - Screen.width / 8, Screen.height * 2 / 6, Screen.width / 8, Screen.height / 10);
    private Rect RectRecalibrate = new Rect(Screen.width / 2 - Screen.width / 8, Screen.height * 3 / 6, Screen.width / 8, Screen.height / 10);
    private Rect RectLogResult = new Rect(Screen.width / 2 - Screen.width / 8, Screen.height * 4 / 6, Screen.width / 8, Screen.height / 10);
    // for dual screen
    GameObject Cube1Hid;
    GameObject Cube2Hid;

    //conditional String to show if movement too fast or slow
    private string speedEvaluationMessage = "";

    //switch to say if a new trial needs to be restared
    private bool needNewTrial = false;

    //Logging variables
    private StringBuilder trialLogger;
    private StringBuilder speedLogger;
    private bool alreadyLogged;
    private int logFileCounter = 0;
    private int rawDataLogCounter = 0;

    //holding values for trial logging
    private int speedEvaluationRecord = 0;

    // local class variables for current and past head velocity for readFromCoil()
    public Vector3 currentHeadVelocity; // this is the current head velocity that is pulled from the stream
    private Vector3 pastHeadVelocity; // this is the lookback velocity that is pulled from the velocity queue
    private Queue<Vector3> headVelocityHistory;  // this stores recent head velocity for lookback
    // lookbackWindow is the number of frames to look back when calculating the acceleration window checks
    // To do this, it is the capacity of the headVelocityHistory queue.
    private int lookbackWindow = 10;

    private uint streamSampleAtOptotypeAppearance;

    public string trialLogFile;

    // Use this for initialization
    void Start()
    {
        warning = GameObject.Find("warning").GetComponent<Text>();
        //Cube1Hid = GameObject.Find("Cube1Hid");
        //Cube2Hid = GameObject.Find("Cube2Hid");
        //vrController = GetComponent<VRController>();
        pl = StartingSceneCanvas.pl;
        //To Do
        optotypeShowDelayTime = (float)pl.optotypeShowDelayTime / 1000;
        
        background.transform.position = Vector3.zero;
        canvas.transform.localPosition = Vector3.zero;
        meshRenderer = this.GetComponentsInChildren<MeshRenderer>();
        showOptotype(true);
        Rect r = pl.GUIRectWithObject(meshRenderer[0]);
        pl.objectSizeScalerForStandardVision = PreferenceLoader.tanFiveOverSixty * (pl.patientToScreenDistance);
        //Debug.Log(String.Format("Screen DPI:{0}, StandardVisionSize{1}: ", Screen.dpi, pl.objectSizeScalerForStandardVision));

        saves = 0;
        goals = 0;
        optotypeDirection[0] = KeyCode.RightArrow;
        optotypeDirection[1] = KeyCode.UpArrow;
        optotypeDirection[2] = KeyCode.LeftArrow;
        optotypeDirection[3] = KeyCode.DownArrow;
        randomKey = UnityEngine.Random.Range(0, optotypeNumber);
        keyClicked = KeyCode.Return;
        this.transform.Rotate(new Vector3(0, 0, 90) * randomKey); // randomly chooses the optotype orientation
        directionIndicator = new ArrayList(); // stores direction sign objects which are created and destroyed at runtime
        dot.SetActive(false);
        dot.transform.position = Vector3.zero;
        glovesInitPosition = gloves.transform.localPosition;
        gloves.GetComponent<SpriteRenderer>().enabled = false;
        pixels2World = (Camera.main.orthographicSize * 2.0f) / Camera.main.pixelHeight; // screen space to world space ratio

        headVelocityHistory = new Queue<Vector3>();
        
        //switch for how to set logmar initially depending on the mode
        if (gameMode != EnumTypes.GameMode.Game)
        {
            logmarScale = pl.svaTestResult;  // NEED TO FIX THIS NAME
            decimalScale = Mathf.Pow(10, logmarScale);
            optotypeSize = decimalScale * pl.objectSizeScalerForStandardVision;
        }
        else // MFW - even for game mode, initial acuity should be what is set in the preferences (but it should not go down to SVA)
        {
            decimalScale = Mathf.Pow(10, logmarScale);
            optotypeSize = decimalScale * pl.objectSizeScalerForStandardVision;
        }
        //Debug.Log(decimalScale);
        //Debug.Log(pl.objectSizeScalerForStandardVision);
        //Debug.Log(optotypeSize);

        // WHAT DOES THIS DO?
        //background scale?
        backgroundScale = background.transform.localScale.x;
        this.transform.localScale = new Vector3(optotypeSize, optotypeSize, optotypeSize) / backgroundScale;
                
        bestDVA = logmarScale;
        bestDVACounter = 0;
        bestDVAReset = 0;
        arrowDisappear = false;

        //headSpeedWindow Threshold and settings for lookback
        speedThreshold = pl.dvaHeadSpeedTriggerThreshold;
        speedDifferenceMin = pl.dvaLowerHeadSpeedWindow;
        speedDifferenceMax = pl.dvaUpperHeadSpeedWindow;

        //history settings
        lookbackWindow = pl.dvaLookBackAmount;


        vrController = GetComponent<VRController>();
        //Polhemus Controller Link
        dataSource = GetComponent<DataSource>();
        dataSource.dynamicReference = this;
        // coilController.changeVelocityHistory(lookbackWindow);

        //Logging setup
        trialLogger = new StringBuilder();
        speedLogger = new StringBuilder();
        // rawDataLogger = new StringBuilder();

        recalibrate();
    }

    // FixedUpdate is called once per frame
    void FixedUpdate()
    {
        /*Vector3 point1 = Cube1Hid.transform.position;
        Vector3 point2 = Cube2Hid.transform.position;
        Vector2 ScreenPoint1 = Camera.main.WorldToScreenPoint(point1);
        Vector2 ScreenPoint2 = Camera.main.WorldToScreenPoint(point2);
        Debug.Log((ScreenPoint1 - ScreenPoint2).magnitude);*/
        // Wait for countdown to finish, is called every frame
        readTracker();

        // Start countdown, exit after one execution
        if (!DVATestStart)
        {
            StartCoroutine(startDVA());
        }

        // Wait for patient's choice the give response, is called every frame
        if (evaluationStart)
        {
            giveFeedback();
        }

        // Update Score
        savesText.text = saves.ToString();
        goalsText.text = goals.ToString();
        //recordMaxSpeedAndAcc();
    }


    // reads in motion information from tracker
    void readTracker()
    {
        //Vector3 headAngle;
        Vector3 translationOnScreen = Vector3.zero;

        readFromDataSource();

        // translation on screen stores how many pixels the objects will move per head's angular position
        // translation along the x-axis depends on rotation about the y-axis
        // translation along the y-axis depends on rotation about the x-axis

        //headAngle = Quaternion2GazeAngleRad(dataSource.currentRotation);
        //to do (1)
        //translationOnScreen.x = -Mathf.Tan(headAngle.y * pl.sceneGain) * pl.patientToScreenDistance * 12 * Screen.dpi * pixels2World;
        // translationOnScreen.y = - Mathf.Tan(headAngle.x * pl.sceneGain) * pl.patientToScreenDistance * 12 * Screen.dpi * pixels2World;

        //background.transform.position = translationOnScreen;
    }

    /* Deprecated:
	 *
	 * Name: recordMaxSpeedAndAcc
	 * Functionality: record the maximum speed and acceleration during one session
	 *
	 */
    /*
   void recordMaxSpeedAndAcc()
   {
       if (headSpeed.magnitude > maxHeadSpeed)
           maxHeadSpeed = headSpeed.magnitude;
       if (accelerationComponent > maxHeadAcc)
           maxHeadAcc = accelerationComponent;
   }
   */
    /*
	 * 
	 * Name: startDVA
	 * Functionality: start DVA test
	 * 
	 */

    private bool restartDVAFlag = false;
    private bool dvaContinue = true;
    private bool waitForEvaluationFlag = false;
    private bool evaluatePatientResponseFlag = false;
    private bool waitBeforeResetFlag = false;

    //TODO implement needNewTrial
    IEnumerator startDVA()
    {
        dvaContinue = true;
        restartDVAFlag = false;

        while (dvaContinue)
        {
            dvaContinue = true;
            if (restartDVAFlag)
            {
                if (waitForEvaluationFlag)
                    StopCoroutine(waitForEvaluation());
                if (evaluatePatientResponseFlag)
                    StopCoroutine(evaluatePatientResponse());
                if(waitBeforeResetFlag)
                    StopCoroutine(waitBeforeReset(5f));
                restartDVAFlag = false;
            }

            DVATestStart = true;

            // Have the subject recenter head to start the next trial
            StartCoroutine(recenterHeadForNewTrial(0f, Vector3.zero, moveBackTime));
            yield return new WaitUntil(() => (movebackFinish == true));

            // Wait at center position before starting new trial
            StartCoroutine(recenterHeadForNewTrial(headRecenteringThreshold, Vector3.zero, pl.keepHeadSteadyTime));
            yield return new WaitUntil(() => movebackFinish == true);
            StartCoroutine(waitForEvaluation());
            StartCoroutine(evaluatePatientResponse());

            //StartCoroutine(waitBeforeReset(5f));

            yield return new WaitUntil(() => (!dvaContinue) || (restartDVAFlag));
            //optotype check moved to evaluatePatientResponse
        }
    }

    /*
	 * 
	 * Name: showOptotype
	 * Functionality: helper function to show or hide optotype
	 * 
	 */
    void showOptotype(bool status)
    {
        if (status)
        {
            foreach (MeshRenderer m in meshRenderer)
            {
                m.enabled = true;
                //Debug.Log(GetComponent<Renderer>().bounds.size);
            }
        }
        else if (!status)
        {
            foreach (MeshRenderer m in meshRenderer)
            {
                m.enabled = false;
            }
        }

    }

    /* 
     * method to show message of the speedEvaluationResponse if head movement was too fast or too slow
     * */
    private void showSpeedEvaluationMessage()
    {
        response.text = speedEvaluationMessage;
    }


    /*
	 *
	 * Name: recalibrate
	 * Functionality: 1: automatically moves object to destination. 
	 * 2: detects if objects are within a certain range of destination
	 * for a certain amount of time.
	 * 
	 */
    IEnumerator recenterHeadForNewTrial(float headRecenteringThreshold, Vector3 destination, float time)
    {
        movebackFinish = false;
        if (headRecenteringThreshold != 0f)
        { // thres = 0f, defaut setting always autorecalibrate
            while (!recalibrateTimeUp)
            {
                showHUD = true;
                StopCoroutine("recalibrateTimer"); // Only use StartCoroutine(string name, object value) and StopCoroutine(string name) would work here.
                dot.SetActive(true);
                response.text = "Center Head";
                yield return new WaitUntil(() => headRotationCheck(headRecenteringThreshold));
                response.text = "Hold Still";
                StartCoroutine("recalibrateTimer", time);
                yield return new WaitUntil(() => !headRotationCheck(headRecenteringThreshold) || recalibrateTimeUp);

            }
            recalibrateTimeUp = false;

            showHUD = false;
            // maxHeadSpeed = 0f;
            // maxHeadAcc = 0f;
            //response.text = null;
        }
        else
        {
            Vector3 optotypePosition = this.transform.localPosition;
            Vector3 soccerPosition = soccer.transform.localPosition;
            Vector3 glovesPosition = gloves.transform.localPosition;
            //Vector3 containerPostion = canvas.transform.localPosition;
            float startTime = Time.time;
            while (Time.time < startTime + time / 2)
            {
                this.transform.localPosition = Vector3.Lerp(optotypePosition, destination, (Time.time - startTime) / (time / 3f));
                soccer.transform.localPosition = Vector3.Lerp(soccerPosition, destination, (Time.time - startTime) / (time / 3f));
                //canvas.transform.localPosition = Vector3.Lerp (containerPostion, destination, (Time.time - startTime)/(time/3f));
                gloves.transform.localPosition = Vector3.Lerp(glovesPosition, destination + glovesInitPosition, (Time.time - startTime) / (time / 3f));
                yield return null;
            }
            yield return new WaitForSeconds(moveBackTime / 2);
        }
        dot.SetActive(false);
        movebackFinish = true;
    }

    //finds the difference between the polhemus value for calibration and the absolute current position
    //TODO
    private bool headRotationCheck(float headRecenteringThreshold)
    {
        // this.evaluationRotation = coilController.currentRotation.eulerAngles.x;
        response.text = "Center Head"; //  (Mathf.Rad2Deg * Mathf.Acos( coilController.currentRotation.w )).ToString();
                                       //return (dataSource.currentRotation.eulerAngles.magnitude < headRecenteringThreshold); // || (Mathf.Abs(Mathf.Abs(this.evaluationRotation) - rotationUpperBound) < headRecenteringThreshold);
        if(Quaternion.Angle(dataSource.currentRotation, new Quaternion(0f, 0f, 0f, 1f)) < headRecenteringThreshold)
        {
            response.text = "Keep Steady";
        }
        return (Quaternion.Angle(dataSource.currentRotation, new Quaternion(0f, 0f, 0f, 1f)) < headRecenteringThreshold);
    }

    /*
	 *
	 * Name: recalibrateTimer
	 * Functionality: helper function to recalibrate() that
	 * helps count time
	 * 
	 */
    IEnumerator recalibrateTimer(float t)
    {
        yield return new WaitForSeconds(t);
        recalibrateTimeUp = true;
    }

    /*
	 * 
	 * Name: evaluatePatientResponse
	 * Functionality: start examining user input
	 * 
	 */
    IEnumerator evaluatePatientResponse()
    {
        evaluatePatientResponseFlag = true;
        optotypeShowTimesRemaining = pl.optotypeShowTimes;
        //StartCoroutine ("TimeSession", optotypeShowTimesRemaining);
        yield return new WaitUntil(() => arrowDisappear);
        while (!evaluationStart && optotypeShowTimesRemaining > 0)
        {
            // if (correctDirection() && validateSpeed())
            if (validateSpeed())
            { // or use checkSpeedAndAcceleration function?
                if (!needNewTrial)
                {
                    showOptotype(true);
                    // pull streamSample at time of optotype appearance to be saved
                    // in trial log

                    //TODO
                    streamSampleAtOptotypeAppearance = dataSource.streamSample;
                    yield return new WaitForSeconds(optotypeShowDelayTime);
                    showOptotype(true);
                }
                optotypeShowTimesRemaining--;
            }
            yield return new WaitForEndOfFrame();
        }
        //StopCoroutine("TimeSession");
        if (needNewTrial)
        {
            showHUD = true;
            showSpeedEvaluationMessage();
            //TODO wait until user response
            yield return new WaitForSeconds(1f);
            DVATestStart = false;
            needNewTrial = false;
        }
        else
        {
            dvaContinue = false;
        }

        showHUD = false;

        evaluatePatientResponseFlag = false;
    }

    /*
	 * 
	 * Name: TimeSession
	 * Functionality: Helper function for evaluateUserResponse(),
	 * Prevent patient from waiting when they are not sure about
	 * their head speed has exceeded the threshold
	 * 
	 */
    IEnumerator TimeSession(int t)
    {
        while (t != 0)
        {
            yield return new WaitForSeconds(2f);
            showHUD = true;
            response.text = speedEvaluationMessage;
            yield return new WaitForSeconds(1f);
            showHUD = false;
        }
    }

    /**
     * 
     * Name: validateSpeed
     * Functionality: tracks when the velocityProfile is above the threshold
     * Sets string message and shows optotype or resets trial depending on if 
     * 
     * */
    bool validateSpeed()
    {
        float difference = Mathf.Abs(headSpeed - lookbackSpeed);
        if (headSpeed > speedThreshold)
        {
            if (difference < speedDifferenceMin)
            {
                speedEvaluationMessage = "Too Slow";
                speedEvaluationRecord = -1;
                needNewTrial = true;
                logTrialData("fast");
                StartCoroutine(logTrial(false));
            }
            else if (difference > speedDifferenceMax)
            {
                speedEvaluationMessage = "Too Fast";
                speedEvaluationRecord = 1;
                needNewTrial = true;
                logTrialData("slow");
                StartCoroutine(logTrial(false));
            }
            else
            {
                speedEvaluationMessage = "Head Speed is Good";
                speedEvaluationRecord = 0;
                needNewTrial = false;
            }
            dataSource.speedEvaluationHash = "" + speedEvaluationRecord;
            return true;
        }
        else
        {
            return false;
        }
    }

    /*
     *   used during a trial evaluation
     *   Convention: 
     *   FixedTime  VariableGain   HeadSpeedTrigger     HeadSpeedLowerWindow    HeadSpeedUpperWindow    LookBackWindow state   Right/Wrong optotypeDirection   optotypeSize    userResponse   Score_saves   Score_goals
     * */
    private void logTrialData(string state)
    {
        //non-dynamic values
        fixTime = Time.fixedTime.ToString();
        trialLogger.Append(Time.fixedTime + "\t" + "\t");
        trialLogger.Append(streamSampleAtOptotypeAppearance + "\t" + "\t");
        trialLogger.Append(pl.sceneGain + "\t" + "\t");
        trialLogger.Append(pl.dvaHeadSpeedTriggerThreshold + "\t" + "\t" + "\t");
        trialLogger.Append(pl.dvaLowerHeadSpeedWindow + "\t" + "\t" + "\t");
        trialLogger.Append(pl.dvaUpperHeadSpeedWindow + "\t" + "\t" + "\t");
        trialLogger.Append(pl.dvaLookBackAmount + "\t" + "\t" + "\t");

        //dynamic values
        trialLogger.Append(state + "\t" + "\t");
        trialLogger.Append(optotypeDirection[randomKey] + "\t" + "\t");
        trialLogger.Append(optotypeSize + "\t");
        trialLogger.Append(keyClicked + "\t" + "\t" + "\t");
        trialLogger.Append(saves + "\t" + "\t");
        trialLogger.Append(goals + "\t");
        trialLogger.AppendLine();
    }

    /*
     * logs the data pulled from Polhemus and that was used in the determination of correct head speed thresholds 
     * Convention
     * FixedTime    velocityMagnitude   acceleration    QuaternionPosition.w    x   y   z  
     * */
    //private void logRawData(float magnitude, float acceleration, Quaternion rotation)
    //{
    //    rawDataLogger.Append(Time.fixedTime + " ");
    //    rawDataLogger.Append(magnitude + " ");
    //    rawDataLogger.Append(rotation.w + " ");
    //    rawDataLogger.Append(rotation.x + " ");
    //    rawDataLogger.Append(rotation.y + " ");
    //    rawDataLogger.Append(rotation.z + " ");
    //    rawDataLogger.AppendLine();
    //}

    /*  Deprecated
	bool checkSpeedAndAcceleration(float speedThresMin, float SpeedThresMax, float accThresMin, float accThresMax, ref string s){
		int result = 0;
		if (maxHeadSpeed >= speedThresMin && maxHeadSpeed <= SpeedThresMax) {
			s = "Head Speed is good";
			result++;
		}
		else if (maxHeadSpeed > SpeedThresMax)
			s = "Head Speed Is Too Fast";
		else
			s = "Head Speed Is Too Slow";
		if (pl.trackerType == 1) {
			if (maxHeadAcc >= accThresMin && accelerationComponent <= accThresMax) {
				s += "\nHead Acceleration Rate Is Good";
				result++;
			} else if (maxHeadAcc < accThresMin) {
				s += "\n Head Acceleration Rate Is Too Low";
				result--;
			} else {
				s += "\n Head Acceleration Rate Is Too High";
				result--;
			}
		}
		return(result == 1 || result == 2);

	}
	*/



    /* 
	 *
	 * Name: waitForEvaluation
	 * Functionality: Cotoutine that controls the display and disappear 
	 * of Direction Indicator and wait to trigger evaluation 
	 * phase. This fucntion is been called between startDVA() and 
	 * evaluatePatientResponse
	 *
	 */
    IEnumerator waitForEvaluation()
    {
        waitForEvaluationFlag = true;
        directionIndicatorDisplayer(); // rotate direction renderer according to head configuration
        yield return new WaitUntil(() => (headSpeed > speedThreshold * 0.4)); // head direction indicator will disappear after reaching 40% of minimum speed thres
        directionIndicatorDestroy();
        yield return new WaitUntil(() => (optotypeShowTimesRemaining == 0) || (needNewTrial));
        if (!needNewTrial)
            evaluationStart = true;
        waitForEvaluationFlag = false;
    }

    /*
     * waitBeforeReset(float)
     * waits set amoutn of seconds before attempting to prompt the user to start a new trial in the calibration stage
     * @param timeToWait: seconds to wait before switchign the flag 
     * */
    IEnumerator waitBeforeReset(float timeToWait)
    {
        waitBeforeResetFlag = true; 
        yield return new WaitForSecondsRealtime(timeToWait);
        if(optotypeShowTimesRemaining != 0 || needNewTrial)
            restartDVAFlag = true;
        waitBeforeResetFlag = false;
    }

    /*
	 * 
	 * Name: directionIndicatorDisplayerHelper
	 * Functionality: Helper function of directionIndicatorDisplayer 
	 * to help display direction arrow
	 * 
	 */
    void directionIndicatorDisplayerHelper(string direction, Vector3 position)
    {
        GameObject arrow = new GameObject(direction);
        arrow.transform.parent = background.transform; // Make it a child of background so it can use its parent world position
        arrow.transform.localPosition = position;
        SpriteRenderer renderer = arrow.AddComponent<SpriteRenderer>();
        renderer.sortingLayerName = "UI";
        Sprite arrowSprite = Resources.Load<Sprite>("Sprites/Arrow/arrow-" + direction);
        renderer.sprite = arrowSprite;
        directionIndicator.Add(arrow);
        directionIndicatorLabel = direction;
        arrow.transform.localScale = new Vector3(1, 1, 1) / backgroundScale;
    }

    

    /*
	 * 
	 * Name: directionIndicatorDisplayer
	 * Functionality: display direction arrows upon configuration
	 * 
	 */
    void directionIndicatorDisplayer()
    {
        arrowDisappear = false;
        switch (pl.headDirection)
        {
            case 0:
                directionIndicatorDisplayerHelper("left", new Vector3(0, 0, 0));
                break;

            case 1:
                directionIndicatorDisplayerHelper("right", new Vector3(0, 0, 0));
                break;

            case 2:
                directionIndicatorDisplayerHelper("up", new Vector3(0, 0, 0));
                break;

            case 3:
                directionIndicatorDisplayerHelper("down", new Vector3(0, 0, 0));
                break;

            case 4:
                directionIndicatorDisplayerHelper("left", new Vector3(-0.6f, 0, 0));
                directionIndicatorDisplayerHelper("right", new Vector3(0.6f, 0, 0));
                break;

            case 5:
                directionIndicatorDisplayerHelper("up", new Vector3(0, 0.6f, 0));
                directionIndicatorDisplayerHelper("down", new Vector3(0, -0.6f, 0));
                break;

            case 6:
                directionIndicatorDisplayerHelper("left", new Vector3(0, 6, 0));
                directionIndicatorDisplayerHelper("right", new Vector3(0, 2, 0));
                directionIndicatorDisplayerHelper("up", new Vector3(0, -2, 0));
                directionIndicatorDisplayerHelper("down", new Vector3(0, -6, 0));
                break;

            case 7:
                float a = UnityEngine.Random.value;
                if (a < .5f)
                {
                    directionIndicatorDisplayerHelper("left", new Vector3(0, 0, 0));
                }
                else
                {
                    directionIndicatorDisplayerHelper("right", new Vector3(0, 0, 0));
                }
                break;

            case 8:
                a = UnityEngine.Random.value;
                if (a < .5f)
                {
                    directionIndicatorDisplayerHelper("up", new Vector3(0, 0, 0));
                }
                else
                {
                    directionIndicatorDisplayerHelper("down", new Vector3(0, 0, 0));
                }
                break;

            case 9:
                a = UnityEngine.Random.value;
                if (a < .25f)
                {
                    directionIndicatorDisplayerHelper("left", new Vector3(0, 0, 0));
                }
                else if (a >= .25f && a < .5f)
                {
                    directionIndicatorDisplayerHelper("right", new Vector3(0, 0, 0));
                }
                else if (a >= .5f && a < .75f)
                {
                    directionIndicatorDisplayerHelper("up", new Vector3(0, 0, 0));
                }
                else
                {
                    directionIndicatorDisplayerHelper("down", new Vector3(0, 0, 0));
                }
                break;
        }
    }

    /*
	 * 
	 * Name: directionIndicatorDestroy
	 * Funtionality: direction arrow is a game object that is created dynamically during run time
	 * It needs to be destroyed before respawning new direction arrow game object for other test sessions
	 * 
	 */
    void directionIndicatorDestroy()
    {
        foreach (GameObject g in directionIndicator)
        {
            Destroy(g);
        }
        directionIndicator.Clear();
        arrowDisappear = true;
    }

    /* 
	 *
	 * Name: giveFeedback
	 * Functionality: giveFeedback() is called every frame
	 * giveFeedback() will call playAnimation() later,
	 * all objects should not be set back to original 
	 * position when animation is on
	 *
	 */
    void giveFeedback()
    {
        if (!animationIsOn)
        {
            paused = true;
            dot.SetActive(false);
            paused = false;
        }

        // Test user response
        if (Input.anyKeyDown && !paused)
        {
            // Here order matters, must first check whether irrelavent key is clicked
            if (getIrreleventKeyClicked())
            {
                StartCoroutine(showIrrelaventResponse());
            }
            else if (getCorrectKeyClicked())
            {
                StartCoroutine(showCorrectResponse());
            }
            else if (getWrongKeyClicked())
            {
                StartCoroutine(showWrongResponse());
            }
        }
    }

    /*
	 * 
	 * Name: showFinishTest
	 * Functionality: Display and store relevant results when
	 * detects DVA test is over
	 * 
	 */
    IEnumerator showFinishTest()
    {
        //response.text = "Your Head Speed Window is: " + lastSpeedThresMin + " - " + lastSpeedThresMax + "\nYour Dynamic Acuity is:\n" + lastOptotypeSize + " in LogMAR";
        response.text = "";
        StartCoroutine(recenterHeadForNewTrial(0f, Vector3.zero, moveBackTime)); // 0f: default setting always recalibrate
        showOptotype(true);
        //pl.dvaSpeedResultMin = lastSpeedThresMin;
        //pl.dvaSpeedResultMax = lastSpeedThresMax;
        pl.dvaAcuityResult = lastOptotypeSize;
        logmarScale = lastOptotypeSize;
        decimalScale = Mathf.Pow(10, logmarScale);
        optotypeSize = decimalScale * pl.objectSizeScalerForStandardVision;

        yield return new WaitForSeconds(1.5f);
        showOptionButtons = true;
    }

    /*
	 * 
	 * Name: showIrrelaventResponse
	 * Functionality: Display response when patient click irrelavent key
	 * A helper fucntion of testResponse()
	 * 
	 */
    IEnumerator showIrrelaventResponse()
    {
        paused = true;

        response.text = "You clicked the wrong key!";
        yield return new WaitForSeconds(0.3f);

        paused = false;
    }

    /*
	 * 
	 * Name: showCorrectResponse
	 * Functionality: Display response when patient responses with the correct key
	 * A helper fucntion of testResponse()
	 * 
	 */
    IEnumerator showCorrectResponse()
    {
        paused = true;
        showHUD = false; // will be reset to TRUE in playAnimation coroutine
        response.text = "You saved the ball!";
        saves++;

        StartCoroutine(playAnimation());
        yield return new WaitUntil(() => animationIsOn == false);
        showResponse("correct");
    }



    /*
	 * 
	 * Name: showWrongResponse
	 * Functionality: Display response when patient responses with the wrong key.
	 * A helper fucntion of testResponse()
	 * 
	 */
    IEnumerator showWrongResponse()
    {
        paused = true;
        showHUD = false; // will be reset to TRUE in playAnimation coroutine
        response.text = "You missed the ball!";
        goals++;
        StartCoroutine(playAnimation());
        yield return new WaitUntil(() => animationIsOn == false);
        showResponse("wrong");
    }

    /*
	 * 
	 * Name: showResponse
	 * Functionality: show response upon patient's input
	 * 
	 */
    void showResponse(string r)
    {
        //lastSpeedThresMin = speedDifferenceMin;
        //lastSpeedThresMax = speedDifferenceMax;
        lastOptotypeSize = logmarScale;

        switch (r)
        {
            case "correct":
                scaleOptotype(decimalScale, -0.1f);
                logTrialData(r);
                if (bestDVA > logmarScale)
                { // new best is reached
                    //TO DO LOGMAR CALCULATION :if(logmarScale)
                    bestDVA = logmarScale;
                }

                break;

            case "wrong":
                if (bestDVA > logmarScale)
                { // new best is reached
                    bestDVA = logmarScale;  // MFW - why make this bestDVA if the response is wrong?
                    bestDVACounter = 0;
                    bestDVAReset = 0;
                }
                else if (Mathf.Approximately(bestDVA, logmarScale))
                { // best is tied
                    bestDVACounter++;
                }
                else
                { // best is not reached
                    bestDVAReset++;
                    if (bestDVAReset >= 2)
                    { // If two more wrongs guesses made without reaching the best, bestSVA will set to current logmarScale   MFW- but it's not doing this
                        bestDVAReset = 0;
                    }
                }
                //print (bestDVA + " " + bestDVACounter + " " + bestDVAReset);
                scaleOptotype(decimalScale, 0.3f);
                logTrialData(r);
                break;
        }
        // If satisfies the stop condition, shows test result
        if (stopCase() && (gameMode != EnumTypes.GameMode.Game))
        {
            StartCoroutine(showFinishTest());
        }
        else
        {
            // rotate back to orginal direction and then randomly rotate again.
            rotateBackAndRandom();
        }
    }

    /*
     * scaleOptotype(float) 
     *  only scales the optotype size if in the in the Calibration mode
     *  @param decimalScale  : the magnitude in logmar by which to scale the optotype size
     */
     private float scaleOptotype(float decimalScale, float logmarAdjustment)
    {
        if (gameMode != EnumTypes.GameMode.Game)
            logmarScale += logmarAdjustment;
        decimalScale = Mathf.Pow(10, logmarScale);
        optotypeSize = decimalScale * pl.objectSizeScalerForStandardVision;
        GameObject Optotype = GameObject.Find("Cube1");
        float Axis_X = Optotype.GetComponent<Renderer>().bounds.size.x;
        //float Axis_y = Optotype.GetComponent<Renderer>().bounds.size.y;
        //Debug.Log(Axis_X);
        //Debug.Log(Axis_y);
        //Debug.Log(optotypeSize);
        if (optotypeSize < 0.05f)
        {
            if(warning != null)
            {
                warning.text = "Reaching Optotype Distortion boundary";
            }
            if (warning == null)
            {
                Debug.Log("1");
            }
            //Debug.Log("Reaching Optotype Distortion boundary");
        }
        if (optotypeSize < 0.0249f)
        {
            if (warning != null)
            {
                warning.text = "Optotype Distorted";
            }
            //Debug.Log("Optotype Distorted");
            Debug.Log(String.Format("Last undistorted Optotype size:,{0}", Axis_X * 5));
        }
        return optotypeSize;
    }

    /*
	 * 
	 * Name: palyAnimation()
	 * Functionality: play soccer ball animation after receive user response
	 * 
	 */
    IEnumerator playAnimation()
    {
        animationIsOn = true;
        showHUD = false;
        gloves.GetComponent<SpriteRenderer>().enabled = true;
        Vector3 movementScale = new Vector3(4, 4, 4);
        Vector3[] ballMovement = new[] {
            new Vector3 (1f, 0f, 0f),
            new Vector3 (0f, 1f, 0f),
            new Vector3 (-1f, 0f, 0f),
            new Vector3 (0f, -1f, 0f)
        };
        Vector3 objectPosition = this.transform.localPosition;
        Vector3 glovesPosition = gloves.transform.localPosition;
        Vector3 currentOptotypeSize = this.transform.localScale;
        Vector3 currentSoccerSize = soccer.transform.localScale;
        //Vector3 currentCanvasScale = canvas.transform.localScale;
        Vector3 gloveMovement = Vector3.zero;
        Vector3 optotypeSizeChanger;
        switch (keyClicked)
        {
            case KeyCode.UpArrow:
                gloveMovement = new Vector3(0f, 1f, 0f);
                break;
            case KeyCode.DownArrow:
                gloveMovement = new Vector3(0f, -1f, 0f);
                break;
            case KeyCode.LeftArrow:
                gloveMovement = new Vector3(-1f, 0f, 0f);
                break;
            case KeyCode.RightArrow:
                gloveMovement = new Vector3(1f, 0f, 0f);
                break;
        }
        ballMovement[randomKey] = Vector3.Scale(ballMovement[randomKey], movementScale);
        gloveMovement = Vector3.Scale(gloveMovement, movementScale);

        showOptotype(true);
        //soccer.GetComponent<SpriteRenderer> ().enabled = true; // Show the soccer ball at this time because its color would mix with the optotype
        yield return new WaitForSeconds(ballAnimationTime / 8);
        float startTime = Time.time;
        while (Time.time < startTime + ballAnimationTime / 2)
        {
            this.transform.localPosition = Vector3.Lerp(objectPosition, objectPosition + ballMovement[randomKey], (Time.time - startTime) / (ballAnimationTime / ballSpeed));
            soccer.transform.localPosition = Vector3.Lerp(objectPosition, objectPosition + ballMovement[randomKey], (Time.time - startTime) / (ballAnimationTime / ballSpeed));
            gloves.transform.localPosition = Vector3.Lerp(glovesPosition, glovesPosition + gloveMovement - glovesInitPosition, (Time.time - startTime) / (ballAnimationTime / ballSpeed));
            optotypeSizeChanger = Vector3.Lerp(Vector3.one, Vector3.one * 2f, (Time.time - startTime) / (ballAnimationTime / ballSpeed));
            soccer.transform.localScale = Vector3.Scale(currentSoccerSize, optotypeSizeChanger);
            this.transform.localScale = Vector3.Scale(currentOptotypeSize, optotypeSizeChanger);
            yield return null;
        }
        yield return new WaitForSeconds(ballAnimationTime / 8);
        showHUD = true;
        yield return new WaitForSeconds((ballAnimationTime / 4));
        showOptotype(true);
        this.transform.localScale = currentOptotypeSize;
        soccer.transform.localScale = currentSoccerSize;
        gloves.GetComponent<SpriteRenderer>().enabled = false;
        animationIsOn = false;
    }

    /* 
	 * 
	 * Name: rotateBackAndRandom
	 * Fucntionality: Clean up work at the end of the 
	 * evaluation procedures
	 *
	 */
    void rotateBackAndRandom()
    {
        this.transform.Rotate(new Vector3(0, 0, -90) * randomKey);
        randomKey = UnityEngine.Random.Range(0, optotypeNumber);
        this.transform.Rotate(new Vector3(0, 0, 90) * randomKey);
        //this.GetComponent<SpriteRenderer> ().sprite = optotypeSprites [randomKey];
        this.transform.localScale = new Vector3(optotypeSize, optotypeSize, optotypeSize) / backgroundScale;

        paused = false;
        evaluationStart = false;
        DVATestStart = false; // Start DVA test one more time
    }

    /*
	 * 
	 * Name: getCorrectKeyClicked
	 * Functionality: Examine whether patient clicked the correct 
	 * key upon optotype direction
	 * 
	 */
    bool getCorrectKeyClicked()
    {
        keyClicked = optotypeDirection[randomKey];
        return Input.GetKeyDown(optotypeDirection[randomKey]) && !paused;
    }

    /*
	 * 
	 * Name: getWrongKeyClicked
	 * Functionality: Examine whether patient clicked the wrong 
	 * key upon optotype direction
	 * 
	 */
    bool getWrongKeyClicked()
    {
        foreach (KeyCode k in optotypeDirection)
        {
            if (Input.GetKeyDown(k))
                keyClicked = k;
        }
        return !Input.GetKeyDown(optotypeDirection[randomKey]) && !paused;
    }

    /*
	 * 
	 * Name: getIrrelaventKeyClicked
	 * Functionality: Examine whether patient clicked the irrelavent 
	 * key upon optotype direction
	 * 
	 */
    bool getIrreleventKeyClicked()
    {
        return !Input.GetKeyDown(KeyCode.UpArrow) && !Input.GetKeyDown(KeyCode.DownArrow) && !Input.GetKeyDown(KeyCode.LeftArrow) && !Input.GetKeyDown(KeyCode.RightArrow) && !paused && bestDVACounter < 2;
    }

    /*
	 * 
	 * Name: stopCase
	 * Functionality: Check if the DVA test has satisfied the 
	 * circumsatance of finishing
	 * 
	 */
    bool stopCase()
    {
        return bestDVACounter >= 2 || logmarScale < -0.1f;  //MFW - changed -0.3f to -0.1f
    }

    /*
	 * 
	 * Name: readFromMouse()
	 * Functionality: this function reads in mouse data of patient's head movement and store it into rawHeadDelta variable,
	 * headDelta variable then uses rawHeadDelta to compute the effective head movement speed according to headDirection option
	 * 
	 */
    

    /*
	 * 
	 * Name: readFromDataSource()
	 * Functionality: this function reads in Data Source's data stream which captures patient's head movement and store it into rawHeadDelta variable,
	 * headDelta variable then uses rawHeadDelta to compute the effective head movement speed according to headDirection option
	 * 
	 */
    void readFromDataSource()
    {

        // WHY DO WE NEED TO DIVIDE INTO X, Y, AND Z AND THEN RECOMBINE?
        currentHeadVelocity = dataSource.angularVelocityRead;//new Vector3(dataSource.angularVelocityRead.x, dataSource.angularVelocityRead.y, dataSource.angularVelocityRead.z);
        headVelocityHistory.Enqueue(currentHeadVelocity);
        if (headVelocityHistory.Count > lookbackWindow)
        {
            headVelocityHistory.Dequeue();
        } 
        pastHeadVelocity = headVelocityHistory.Peek();

        switch (pl.headDirection)
        {
            case 0: // Leftward only
                if (currentHeadVelocity.y > 0)
                {
                    headSpeed = Mathf.Abs(currentHeadVelocity.y);
                    lookbackSpeed = Mathf.Abs(pastHeadVelocity.y);
                }
                else
                {
                    headSpeed = 0f;
                    lookbackSpeed = 0f;
                }
                break;

            case 1: // Rightward only
                if (currentHeadVelocity.y <= 0)
                {
                    headSpeed = Mathf.Abs(currentHeadVelocity.y);
                    lookbackSpeed = Mathf.Abs(pastHeadVelocity.y);
                }
                else
                {
                    headSpeed = 0f;
                    lookbackSpeed = 0f;
                }
                break;

            case 2: // Upward only
                if (currentHeadVelocity.x >= 0)
                {
                    headSpeed = Mathf.Abs(currentHeadVelocity.x);
                    lookbackSpeed = Mathf.Abs(pastHeadVelocity.x);
                }
                else
                {
                    headSpeed = 0f;
                    lookbackSpeed = 0f;
                }
                break;

            case 3: // Downward only
                if (currentHeadVelocity.x < 0)
                {
                    headSpeed = Mathf.Abs(currentHeadVelocity.x);
                    lookbackSpeed = Mathf.Abs(pastHeadVelocity.x);
                }
                else
                {
                    headSpeed = 0f;
                    lookbackSpeed = 0f;
                }
                break;

            case 4: // Rightward or Leftward
                headSpeed = Mathf.Abs(currentHeadVelocity.y);
                lookbackSpeed = Mathf.Abs(pastHeadVelocity.y);
                break;

            case 5: // Upward or Downward
                headSpeed = Mathf.Abs(currentHeadVelocity.x);
                lookbackSpeed = Mathf.Abs(pastHeadVelocity.x);
                break;

            case 6: // Any direction
                Vector3 headSpeedTot = new Vector3(currentHeadVelocity.x, currentHeadVelocity.y, 0);
                headSpeed = headSpeedTot.magnitude;
                Vector3 lookbackSpeedTot = new Vector3(pastHeadVelocity.x, pastHeadVelocity.y, 0);
                lookbackSpeed = lookbackSpeedTot.magnitude;
                break;

            case 7: // Random horizontal direction
                if (directionIndicatorLabel == "left")
                {
                    if (currentHeadVelocity.y < 0)
                    {
                        headSpeed = Mathf.Abs(currentHeadVelocity.y);
                        lookbackSpeed = Mathf.Abs(pastHeadVelocity.y);
                    }
                    else
                    {
                        headSpeed = 0f;
                        lookbackSpeed = 0f;
                    }
                }
                else
                {
                    if (currentHeadVelocity.y >= 0)
                    {
                        headSpeed = Mathf.Abs(currentHeadVelocity.y);
                        lookbackSpeed = Mathf.Abs(pastHeadVelocity.y);
                    }
                    else
                    {
                        headSpeed = 0f;
                        lookbackSpeed = 0f;
                    }
                }
                break;

            case 8: // Random vertical direction
                if (directionIndicatorLabel == "up")
                {
                    if (currentHeadVelocity.x >= 0)
                    {
                        headSpeed = Mathf.Abs(currentHeadVelocity.x);
                        lookbackSpeed = Mathf.Abs(pastHeadVelocity.x);
                    }
                    else
                    {
                        headSpeed = 0f;
                        lookbackSpeed = 0f;
                    }
                }
                else
                {
                    if (currentHeadVelocity.x < 0)
                    {
                        headSpeed = Mathf.Abs(currentHeadVelocity.x);
                        lookbackSpeed = Mathf.Abs(pastHeadVelocity.x);
                    }
                    else
                    {
                        headSpeed = 0f;
                        lookbackSpeed = 0f;
                    }
                }
                break;

            case 9: // Random any direction
                if (directionIndicatorLabel == "left")
                {
                    if (currentHeadVelocity.y < 0)
                    {
                        headSpeed = Mathf.Abs(currentHeadVelocity.y);
                        lookbackSpeed = Mathf.Abs(pastHeadVelocity.y);
                    }
                    else
                    {
                        headSpeed = 0f;
                        lookbackSpeed = 0f;
                    }
                }
                else if (directionIndicatorLabel == "right")
                {
                    if (currentHeadVelocity.y >= 0)
                    {
                        headSpeed = Mathf.Abs(currentHeadVelocity.y);
                        lookbackSpeed = Mathf.Abs(pastHeadVelocity.y);
                    }
                    else
                    {
                        headSpeed = 0f;
                        lookbackSpeed = 0f;
                    }
                }
                else if (directionIndicatorLabel == "up")
                {
                    if (currentHeadVelocity.x >= 0)
                    {
                        headSpeed = Mathf.Abs(currentHeadVelocity.x);
                        lookbackSpeed = Mathf.Abs(pastHeadVelocity.x);
                    }
                    else
                    {
                        headSpeed = 0f;
                        lookbackSpeed = 0f;
                    }
                }
                else
                {
                    if (currentHeadVelocity.x < 0)
                    {
                        headSpeed = Mathf.Abs(currentHeadVelocity.x);
                        lookbackSpeed = Mathf.Abs(pastHeadVelocity.x);
                    }
                    else
                    {
                        headSpeed = 0f;
                        lookbackSpeed = 0f;
                    }
                }
                break;
        }
        // translation on screen stores how many pixels the objects will move per head's angular position
        // translation along the x-axis depends on rotation about the y-axis
        // translation along the y-axis depends on rotation about the x-axis
        float tangentVector = 0;
        if(dataSource.currentRotation.eulerAngles.y != 0)
        {
            tangentVector = -Mathf.Tan(Mathf.Deg2Rad * dataSource.currentRotation.eulerAngles.y);
        }
        translationOnScreen = new Vector3(tangentVector, -Mathf.Tan(Mathf.Deg2Rad * dataSource.currentRotation.eulerAngles.x), 0) * pl.patientToScreenDistance * 12 * Screen.dpi * pixels2World;
        // accelerationComponent = Mathf.Abs(coilController.getAccelerationVector().z);

        //logs raw data from polhemusController
        // logRawData(headSpeed.magnitude, accelerationComponent, polhemusController.getRotation());
    }

    void OnGUI()
    {
        GUIStyle countdownStyle = new GUIStyle();
        countdownStyle.normal.textColor = Color.yellow;
        countdownStyle.fontSize = 30;
        countdownStyle.alignment = TextAnchor.MiddleCenter;

        GUIStyle headupStyle = new GUIStyle();
        headupStyle.alignment = TextAnchor.MiddleCenter;
        headupStyle.fontSize = 20;
        headupStyle.normal.textColor = Color.magenta;

        /*
        info = "Head Speed Window:\n"
        + speedDifferenceMin
        + " - "
        + speedDifferenceMax
        + "\nAcuity:\n"
        + logmarScale
        + " in LogMAR\n"
        + pl.patientToScreenDistance
        + "/"
        + pl.patientToScreenDistance * decimalScale
        + " in foot\nCurrent Head Acceleration: "
        + Mathf.Abs(headSpeed.magnitude - pastSpeed.magnitude).ToString()
        + "\nBest DVA: "
        + bestDVA
        + " has reached "
        + bestDVACounter
        + " times";
        */

        // GUI.Box(new Rect(Screen.width * 2 / 3, Screen.height * 3 / 5, Screen.width / 10, Screen.height / 5), info, headupStyle);

        //if (showTestInfo)
        //{
        //    GUI.Box(new Rect(Screen.width / 3, Screen.height / 5, Screen.width / 3, Screen.height / 10), testInfo, countdownStyle);
        //}

        if (showHUD)
            canvas.gameObject.SetActive(true);
        else
            canvas.gameObject.SetActive(false);

      

        // Change transparency for buttons
        // Color oldColor = GUI.color;
        // GUI.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);

        // if (GUI.Button(new Rect(Screen.width / 20, Screen.height * 1 / 15, Screen.width / 10, Screen.height / 10), "Main Menu"))
        if (GUI.Button(RectMainMenu, "Main Menu"))
        {
            //dataSource.QuitStream();
            //Destroy(dataSource);
            /*
            if (gameMode != EnumTypes.GameMode.Game)
                SceneManager.LoadSceneAsync("DynamicPrep", LoadSceneMode.Single);
            else
                SceneManager.LoadSceneAsync("GamePlayPrep", LoadSceneMode.Single);
            */
            SceneManager.LoadSceneAsync("StartingScene", LoadSceneMode.Single);
        }
        if (GUI.Button(RectRecalibrate, "Reset Head Center"))
        {
            recalibrate();
        }


        if (GUI.Button(RectLogResult, "Log Result"))
        {
            StartCoroutine(logTrial(true));
            StartCoroutine(vrController.logSpeed(true));
        }

        // Change transparency back
        // GUI.color = oldColor;

    }

    /* 
     * 
	 * Name: recalibrate
	 * Functionality: Recalibrates the ball and pollhemus to 
	 * the current head position, zeroes all 
	 * values
	 *
	 */
    void recalibrate()
    {
        dataSource.calibrate();
        // polhemusController.zero();
        background.transform.position = Vector3.zero;
        this.transform.localPosition = Vector3.zero;
        soccer.transform.localPosition = Vector3.zero;
        gloves.transform.localPosition = glovesInitPosition;
        canvas.transform.localPosition = Vector3.zero;
    }


    /*
     * Method call on button press to put current StringBuilder storage into text file
     * */
    IEnumerator logTrial(bool incrementLog)
    {
        StreamWriter file;
        try
        {

            // create log file if it does not already exist. Otherwise open it for appending new trial
            if (!File.Exists(trialLogFile) || incrementLog)
            {
                trialLogFile = "trialLog_" + String.Format("{0:_yyyy_MM_dd_hh_mm_ss}", DateTime.Now) + ".txt";
                file = new StreamWriter(trialLogFile);
                file.WriteLine("FixedTime\tStreamSample\tSceneGain\tHeadSpeedTrigger\tHeadSpeedLowerWindow\tHeadSpeedUpperWindow\tLookBackWindow state\tRight/Wrong\toptotypeDirection\toptotypeSize\tuserResponse\tScore_saves\tScore_goals");
            }
            else
            {
                file = File.AppendText(trialLogFile);
            }

            //file = new StreamWriter("triallogs" + logFileCounter + ".txt");
            file.WriteLine(trialLogger.ToString());
            file.Close();
            trialLogger = new StringBuilder();
            if(incrementLog)
                logFileCounter++;
        }
        catch (System.Exception e)
        {
            Debug.Log("Error in accessing file: " + e);
        }

        yield return new WaitForSeconds(.1f);
    }


    public Vector3 Quaternion2GazeAngleRad(Quaternion q)
    {
        Vector3 g = new Vector3(); // gaze vector for output (using Haustein correction)
        Vector3 r = new Vector3(); // rotation vector for calculations

        float x;
        float y;
        float z;

        // convert quaternion to RHR rotation vector
        r[0] = -q.z / q.w;
        r[1] = q.x / q.w;
        r[2] = -q.y / q.w;

        // convert to gaze and assign back to Unity coordinates with LHR
        z = -2 * Mathf.Atan(r[0]);
        if (!float.IsNaN(z))
            g[2] = z;  // roll z

        x = -2 * Mathf.Atan((r[1] - r[0] * r[2]) / (1 + r[0] * r[0]));
        if (!float.IsNaN(x))
            g[0] = x; // pitch x

        y = -2 * Mathf.Atan((r[2] + r[0] * r[1]) / (1 + r[0] * r[0]));
        if(!float.IsNaN(y))
            g[1] = y;  // yaw y

        return (g);
    }


}