using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;

public class TrainController : MonoBehaviour {
	// Public Variables
    public Vector3 headSpeed = Vector3.zero;
	public Vector3 DirectionSwitch; // Used for extracting speed component from a single axis
    //public DataSource.Source sourceType;
    public float accelerationComponent;

    // slider total value and bottom value for adjusting speed threshold
    public float sliderThresMax = 3f;
	public float sliderThresMin = .5f;
    StreamWriter file;

	// Private Variables
	private PreferenceLoader pl;
    //private DataSource dataSource;
    private VRController vrController;

    [SerializeField]
    private GameObject background;
    [SerializeField]
    private GameObject dot;
	[SerializeField]
	private Text response;
	[SerializeField]
	private GameObject canvas;
    [SerializeField]
    private Text HeadSpeed;
    //int i = 0;


	private float moveBackTime = .5f;
    private float resetTime = 0f;
	private bool showHUD = true;
	private bool movebackFinish = false; // indicated whether a moveBack() coroutine is finished
	private bool recalibrateTimeUp = false; // check if time is up in recalibrate() function
	private bool trainingStarted = false;
	private bool movingHead = false;
	private bool showTestResult = false;

	// private float accelerationComponent;
	private Vector3 lastMousePosition = Vector3.zero;

    //calculations for moving screeen 
    private float pixels2World;
    private Vector3 translationOnScreen = new Vector3();

	// For test purpose
	public bool mouseEnable = true;
	private float maxHeadSpeed;
	private float maxHeadAcc;
    
	// Use this for initialization
	void Start () {
        file = new StreamWriter("MaxSpeed.txt");
		if (GameObject.Find ("PreferenceLoader") == null) {
			GameObject obj = Instantiate (Resources.Load("PreferenceLoader")) as GameObject;
			obj.name = "PreferenceLoader";
		}
		pl = GameObject.Find ("PreferenceLoader").GetComponent<PreferenceLoader>(); // Get PreferenceLoader script which stores all user setting.


		DirectionSwitch = new Vector3 (1, 0, 0);

        
        pixels2World = (Camera.main.orthographicSize * 2.0f) / Camera.main.pixelHeight; // screen space to world space ratio
		maxHeadAcc = 0f;
		maxHeadSpeed = 0f;


        //Connect to other scripts
        //dataSource = GetComponent<DataSource>();
        vrController = GetComponent<VRController>();
        /*if(dataSource != null)
            dataSource.initializeSource(sourceType);*/

    }
	
	// Update is called once per frame
	void FixedUpdate () {
        // Run once then wait for boolean to set to false
        //to do
        //Debug.Log(vrController.angularVelocityRead.y);
        background.transform.Rotate(0, vrController.angularVelocityRead.y * Time.fixedDeltaTime * pl.sceneGain, 0);
            
        if (!trainingStarted)
			StartCoroutine (startTraining ());

		if (movingHead)
			StartCoroutine (testing (4f, Vector3.zero));
		
		if (mouseEnable) {
			readFromMouse ();
			dot.transform.Translate (headSpeed * 0.1f, Space.World);
			translationOnScreen = dot.transform.localPosition;
		} else {
			readFromTransmsitter ();

            //TODO
            //translationOnScreen = new Vector3 (Mathf.Tan (Mathf.Deg2Rad * dataSource.currentRotation.eulerAngles.y), Mathf.Tan (Mathf.Deg2Rad * dataSource.currentRotation.eulerAngles.x), 0) * pl.patientToScreenDistance * 12 * Screen.dpi * pixels2World;
            translationOnScreen = new Vector3(Mathf.Tan(Mathf.Deg2Rad * vrController.currentRotation.eulerAngles.y), Mathf.Tan(Mathf.Deg2Rad * vrController.currentRotation.eulerAngles.x), 0)  * 12 * Screen.dpi * pixels2World;

            //Debug.Log(string.Format("currentRotation{0}", vrController.currentRotation));
        }
        HeadSpeed.text = "HeadSpeed:" + headSpeed.magnitude;
        //recordMaxSpeedAndAcc ();
        //Debug.Log(maxHeadSpeed);
	}

    void OnApplicationQuit()
    {
        if (file != null)
        {
            file.Close();
        }
    }

    void readFromTransmsitter(){
		headSpeed = Vector3.Scale(new Vector3(vrController.angularVelocityRead.x, vrController.angularVelocityRead.y, vrController.angularVelocityRead.z), DirectionSwitch);
        // Debug.Log(headSpeed);
        //accelerationComponent = Mathf.Abs(vrController.GetAcceleration());
        //Debug.Log(string.Format("Calculated Acc{0}, System Acc{1}",maxHeadAcc,vrController.angularAcc));
    }

    IEnumerator startTraining(){
		trainingStarted = true;
		StartCoroutine(recalibrate (4f, Vector3.zero, pl.keepHeadSteadyTime));
		yield return new WaitUntil (() => movebackFinish == true);
		response.text = "Now Start Moving Head Around. To View Training Result, Recalibrate To Center Again.";
		maxHeadSpeed = 0f;
		maxHeadAcc = 0f;
		movingHead = true;
	}

	/*
	 *
	 * Name: recalibrate
	 * Functionality: detects if objects are within a certain range of destination
	 * for a certain amount of time.
	 * 
	 */
	IEnumerator recalibrate(float autoRecalibrateThres, Vector3 destination, float time){
		movebackFinish = false;
		while (!recalibrateTimeUp) {
			showHUD = true;
			StopCoroutine ("recalibrateTimer"); // Only use StartCoroutine(string name, object value) and StopCoroutine(string name) would work here.
			dot.GetComponent<MeshRenderer> ().enabled = true;
			response.text = "Please Adjust Your Head Position\nUntil The Ball Overlaps The Red Dot";
			yield return new WaitUntil (() => Vector3.Magnitude(translationOnScreen) < autoRecalibrateThres);
			response.text = "Keep Head Steady For A While";
			StartCoroutine ("recalibrateTimer", time);
			yield return new WaitWhile (() => Vector3.Magnitude (translationOnScreen) < autoRecalibrateThres && !recalibrateTimeUp);
		}
		recalibrateTimeUp = false;
		if (showTestResult) {
			response.text = checkSpeedAndAcceleration(pl.trainSpeedThresMin, pl.trainSpeedThresMax, pl.trainAccThresMin, pl.trainAccThresMax);
			yield return new WaitForSeconds (1f);
			showTestResult = false;
		}
		movebackFinish = true;
	}

	IEnumerator testing(float autoRecalibrateThres, Vector3 destination){
		movingHead = false;
		yield return new WaitUntil (() => Vector3.Magnitude(translationOnScreen) > autoRecalibrateThres * 2);
		yield return new WaitUntil (() => Vector3.Magnitude(translationOnScreen) < autoRecalibrateThres);
		showTestResult = true;
		trainingStarted = false;
	}

	/*
	 *
	 * Name: recalibrateTimer
	 * Functionality: helper function to recalibrate() that
	 * helps count time
	 * 
	 */ 
	IEnumerator recalibrateTimer(float t){
		yield return new WaitForSeconds (t);
		recalibrateTimeUp = true;
	}

	void readFromMouse(){
		headSpeed = Input.mousePosition - lastMousePosition;
		lastMousePosition = Input.mousePosition;
	}

	void recordMaxSpeedAndAcc(){
		if (headSpeed.magnitude > maxHeadSpeed)
			maxHeadSpeed = headSpeed.magnitude;
        file.WriteLine(maxHeadSpeed.ToString());
        if (accelerationComponent > maxHeadAcc)
        	maxHeadAcc = accelerationComponent;
    }

	string checkSpeedAndAcceleration(float speedThresMin, float SpeedThresMax, float accThresMin, float accThresMax){
		string ret;
		if (maxHeadSpeed >= speedThresMin && maxHeadSpeed <= SpeedThresMax)
			ret = "Head Speed is good";
		else if (maxHeadSpeed> SpeedThresMax)
			ret = "Head Speed Is Too Fast";
		else
			ret = "Head Speed Is Too Slow";
		if (!mouseEnable) {
			if (maxHeadAcc >= accThresMin && maxHeadAcc <= accThresMax)
				ret += "\nHead Acceleration Rate Is Good.";
			else if (maxHeadAcc < accThresMin)
				ret += "\n Head Acceleration Rate Is Too Low";
			else
				ret += "\n Head Acceleration Rate Is Too High";
		}
		return ret;
	}

	void OnGUI(){
		GUIStyle countdownStyle = new GUIStyle ();
		countdownStyle.normal.textColor = Color.red;
		countdownStyle.fontSize = 20;
		countdownStyle.alignment = TextAnchor.MiddleCenter;

		if (showHUD)
			canvas.gameObject.SetActive (true);
		else
			canvas.gameObject.SetActive (false);

		//GUI.Box (new Rect (Screen.width / 3, Screen.height / 6, Screen.width / 3, Screen.height / 6), "\nHead Speed:\n" + headSpeed.magnitude, countdownStyle);
		//speedThres = GUI.VerticalSlider (new Rect (Screen.width / 3, Screen.height / 2, Screen.width / 2, Screen.height / 4), speedThres, sliderThresMax, sliderThresMin);
		/* if (GUI.Button (new Rect (Screen.width *2 / 5, Screen.height *13/15, Screen.width / 5, Screen.height / 10), "Start Static Acurity Test")) {
			coilController.quitPlStream();
			Destroy(coilController);
			SceneManager.LoadSceneAsync ("StaticPrep", LoadSceneMode.Single);
		} */

        //back button
        if (GUI.Button(new Rect(Screen.width / 16, Screen.height * 1 / 15, Screen.width / 10, Screen.height / 10), "Back"))
        {
			//dataSource.QuitStream();
			//Destroy(dataSource);
            SceneManager.LoadSceneAsync("StartingScene", LoadSceneMode.Single);
        }
        //goes to next attempt for head movement
        if (GUI.Button(new Rect(Screen.width *3/4, Screen.height * 7/15, Screen.width / 10, Screen.height / 10), "New Attempt"))
        {
			recalibrate ();
        }

    }

	/* 
	 * 
	 * Recalibrates the ball and pollhemus to 
	 * the current head position, zeroes all 
	 * values
	 *
	 */
	void recalibrate()
	{
		//dataSource.calibrate();
		//coilController.zero();
		background.transform.position = Vector3.zero;
		this.transform.localPosition = Vector3.zero;
		canvas.transform.localPosition = Vector3.zero;
	}
	
	
}
