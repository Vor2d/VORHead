using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StaticAcuityController : MonoBehaviour {
	// Private variables
	private float testResult; // test result stores the scale in logMAR;
	private const int optotypeNumber = 4;
	private KeyCode[] optotype = new KeyCode[optotypeNumber];
	private int randomKey = 0;
	private bool paused = false; // For enable/disable input
	private bool showResponse = false;
	private bool showOptionButton = false;
	private string response = "";
	private float decimalScale; // decimalScale * distance to get the real result
	private float logmarScale;  // logmarScale = log 10 base decimalScale;
	private MeshRenderer[] meshRenderer; // array of MeshRenderers for all elements in Optotype prefab
	private PreferenceLoader pl;
	private float bestSVA;
	private int bestSVACounter;
	private int bestSVAReset;
	private int afterWrongCounter;
	private bool afterWrong;
	private int wrongCounter;
	private bool stop;
	private string info;

    // public float newScale;

	// Use this for initialization
	void Start () {
		if (GameObject.Find ("PreferenceLoader") == null) {
			GameObject obj = Instantiate (Resources.Load("PreferenceLoader")) as GameObject;
			obj.name = "PreferenceLoader";
		}
		pl = GameObject.Find ("PreferenceLoader").GetComponent<PreferenceLoader>(); // Get PreferenceLoader script which stores all user setting.

		meshRenderer = this.GetComponentsInChildren<MeshRenderer> ();
		Rect r = pl.GUIRectWithObject (meshRenderer[0]);
		pl.objectSizeScalerForStandardVision = PreferenceLoader.tanFiveOverSixty / ((r.width * 5) / ( Screen.dpi * pl.patientToScreenDistance * 12));
			
        optotype [0] = KeyCode.RightArrow;
		optotype [1] = KeyCode.UpArrow;
		optotype [2] = KeyCode.LeftArrow;
		optotype [3] = KeyCode.DownArrow;

		logmarScale = pl.svaInitialOpSize;
		decimalScale = Mathf.Pow (10, logmarScale);

		randomKey = Random.Range (0, optotypeNumber);
		this.transform.Rotate(new Vector3(0, 0, 90) * randomKey);
		this.transform.localScale = new Vector3 (decimalScale, decimalScale, decimalScale) * pl.objectSizeScalerForStandardVision;
		bestSVA = logmarScale;
		bestSVACounter = 0;
		bestSVAReset = 0;

		afterWrong = false;
		afterWrongCounter = 0;
		wrongCounter = 0;
		stop = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (stopCase ()) {
			StartCoroutine (showFinishTest ());
		}
        
        if (paused == true)
        {
            showOptotype(false);
        }
        else
        {
            showOptotype(true);
        }

		if (Input.anyKeyDown && !paused) {
			if (getirrelevantKeyClicked ())
				StartCoroutine ("showirrelevantResponse");
			else if (getCorrectKeyClicked ())
				StartCoroutine ("ShowCorrectResponse");
			else if (getWrongKeyClicked ())
				StartCoroutine ("ShowWrongResponse");

		}
	}

	// Check if pause key is clicked
	bool getPauseKeyClicked(){
		return Input.GetKeyDown (KeyCode.Escape);
	}
	// Check if patient enters correct response
	bool getCorrectKeyClicked(){
		return Input.GetKeyDown (optotype[randomKey]) && !paused;
	}
	// Check if patient enters wrong response
	bool getWrongKeyClicked(){
		return !Input.GetKeyDown (optotype [randomKey]) && !paused && !Input.GetMouseButtonDown(0) && !Input.GetMouseButtonDown(1);
    }
	// Check if patient enters irrelevant response
	bool getirrelevantKeyClicked(){
		return !Input.GetKeyDown(KeyCode.UpArrow) && !Input.GetKeyDown(KeyCode.DownArrow) && !Input.GetKeyDown(KeyCode.LeftArrow) && !Input.GetKeyDown(KeyCode.RightArrow) && !paused && bestSVACounter < 2 && !Input.GetMouseButtonDown(0) && !Input.GetMouseButtonDown(1);
	}

	bool stopCase(){
		return logmarScale < -0.1f || bestSVACounter >= 2;
	}

	// if patient recognizes correctly, pick another stimulus direction, 
	// make the stimulus smaller, increase the counter
	IEnumerator ShowCorrectResponse(){
		paused = true;
        showResponse = false; // true;
		response = "You are correct!";

		yield return new WaitForSeconds (1.5f);
		testResponse ("correct");
	}

	void testResponse(string r){
		switch (r) {
		case "correct":
			testResult = logmarScale;
			if (!afterWrong) { // In case of getting too small
				if (logmarScale >= 0f) {
					logmarScale -= 0.1f;
					decimalScale = Mathf.Pow (10, logmarScale);
				}
			} else if (afterWrong) {
				afterWrongCounter++;
				if(afterWrongCounter == 5){
					if (wrongCounter > 2) {
						if (logmarScale <= 1.0f) {
							logmarScale += 0.3f;
							decimalScale = Mathf.Pow (10, logmarScale);
						}
					} else {
						if (logmarScale >= -.3f) {
							logmarScale -= 0.1f;
							decimalScale = Mathf.Pow (10, logmarScale);
						}
					}
					afterWrongCounter = 0;
					afterWrong = false;
				}
			}
			if (stop) {
				showFinishTest ();
			}
			break;

		case "wrong":
			wrongCounter++;
			if (afterWrong) {
				afterWrongCounter += 1;
				if (afterWrongCounter == 5) {
					if (wrongCounter > 2) {
						if (logmarScale <= 1.0f) {
							logmarScale += 0.3f;
							decimalScale = Mathf.Pow (10, logmarScale);
						}
					} else {
						if (logmarScale >= -.3f) {
							logmarScale -= 0.1f;
							decimalScale = Mathf.Pow (10, logmarScale);
						}
					}
					afterWrongCounter = 0;
					wrongCounter = 0;
					afterWrong = false;
				}
			} else if (!afterWrong && afterWrongCounter == 0) {
				afterWrong = true;
			}
			if (stop) {
				showFinishTest ();
			}
			print (afterWrongCounter);
			break;
		}
		// rotate back to orginal direction and then randomly rotate again.
		rotateBackAndRandom();
	}

	void rotateBackAndRandom(){
        // float newScale; 

		this.transform.Rotate(new Vector3(0, 0, -90) * randomKey);
		randomKey = Random.Range (0, optotypeNumber);
		this.transform.Rotate(new Vector3(0, 0, 90) * randomKey);
        // newScale = Mathf.Round(decimalScale);
        this.transform.localScale = new Vector3 (decimalScale, decimalScale, decimalScale) * pl.objectSizeScalerForStandardVision;
        // this.transform.localScale = new Vector3(newScale, newScale, newScale) * pl.objectSizeScalerForStandardVision;
        paused = false;
		showResponse = false;
	}

	// if patient recognizes incorrectly, pick another stimulus direction, 
	// make the stimulus bigger, reset the counter
	IEnumerator ShowWrongResponse(){
		paused = true;
        showResponse = false; //true;
		response = "You are wrong!";
		yield return new WaitForSeconds (1.5f);
		testResponse ("wrong");
	}

	// If patient makes irrelevant input, show warning message and do nothing
	IEnumerator showirrelevantResponse (){
		paused = true;
        showResponse = false; // true;
		response = "Invalid response";

		yield return new WaitForSeconds (0.3f);

		paused = false;
		showResponse = false;
	}

	// If patient correctly recognizes stimulus for consecutive times
	IEnumerator showFinishTest(){
		paused = true;
		showResponse = true;
		response = "You have finished your static acuity test.\n Your test score is\n" + testResult + " in LogMAR\n"+ pl.patientToScreenDistance + "/" + pl.patientToScreenDistance * decimalScale + " in foot.";

		// Stores static acuity test result for later use
		pl.svaTestResult = testResult;
		logmarScale = testResult;
		decimalScale = Mathf.Pow (10, logmarScale);
		this.transform.localScale = new Vector3 (decimalScale, decimalScale, decimalScale) * pl.objectSizeScalerForStandardVision;

		yield return new WaitForSeconds (1.5f);

		// showOptionButton = true;  // GO back to main menu instead
		paused = false;
	}

	void OnGUI(){
		GUIStyle responseStyle = new GUIStyle ();
		responseStyle.normal.textColor = Color.red;
		responseStyle.fontSize = 24;
		responseStyle.alignment = TextAnchor.MiddleCenter;

		info = "Acuity:\n"
		+ logmarScale
		+ " in LogMAR\n"
		+ pl.patientToScreenDistance
		+ "/"
		+ pl.patientToScreenDistance * decimalScale
		+ " in foot\n";

		// GUI.Box (new Rect (Screen.width* 3/4, Screen.height * 2 / 5, Screen.width/10, Screen.height/10), info, responseStyle);

		if (showResponse) 
			GUI.Box (new Rect (Screen.width / 3, Screen.height/2, Screen.width/3, Screen.height/4), response, responseStyle);

        /* if (showOptionButton) {
			if (GUI.Button (new Rect (Screen.width *  15/ 60, Screen.height *4 / 5, Screen.width / 10, Screen.height / 10), "Restart")) {
				SceneManager.LoadSceneAsync ("Static", LoadSceneMode.Single);
			}
			if (GUI.Button (new Rect (Screen.width * 27/60, Screen.height *4 / 5, Screen.width / 10, Screen.height / 10), "Contine to Dynamic\n Acuity Test")) {
				SceneManager.LoadSceneAsync ("DynamicPrep", LoadSceneMode.Single);
			}

		} */

        //skip test
        /*
        if (GUI.Button(new Rect(Screen.width * 39 / 60, Screen.height * 4 / 5, Screen.width / 10, Screen.height / 10), "Skip"))
        {
            SceneManager.LoadSceneAsync("DynamicPrep", LoadSceneMode.Single);
        }
        */
        //go back to Main Menu
        // Color oldColor = GUI.color;
        // GUI.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
        
        // if (GUI.Button(new Rect(Screen.width / 2 - Screen.width / 16, Screen.height / 2 - Screen.height / 20, Screen.width / 8, Screen.height / 10), "MainMenu"))
        if (GUI.Button(new Rect(Screen.width / 15, Screen.height / 15, Screen.width / 8, Screen.height / 10), "MainMenu"))
        {
            SceneManager.LoadSceneAsync("StartingScene", LoadSceneMode.Single);
        } 
        // GUI.color = oldColor;
    }

    void showOptotype(bool status)
    {
        if (status)
        {
            foreach (MeshRenderer m in meshRenderer)
            {
                m.enabled = true;
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

}