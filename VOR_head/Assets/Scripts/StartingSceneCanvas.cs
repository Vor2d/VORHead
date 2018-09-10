using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;

public class StartingSceneCanvas : MonoBehaviour
{

    public Dropdown headDirectionDropdown;
    static public PreferenceLoader pl;
    private VRController vrController;

    // private PolhemusController polhemusController;
    private bool showConfigurationPanel = false;
    private bool showConfigurationButton = true;
    private bool showPlayModes = true;
    private string screenDistanceString; // user to screen distance which is needed to calculate variable gain coefficient

    // Use this for initialization
    void Start()
    {
        // polhemusController = DynamicAcuityController.polhemusController;

        if (GameObject.Find("PreferenceLoader") == null)
        {
            GameObject obj = Instantiate(Resources.Load("PreferenceLoader")) as GameObject;
            obj.name = "PreferenceLoader";
        }
        pl = GameObject.Find("PreferenceLoader").GetComponent<PreferenceLoader>(); // Get PreferenceLoader script which stores all user setting.

        headDirectionDropdown.gameObject.SetActive(false);
        headDirectionDropdown.AddOptions(pl.headDirectionOptions);
        headDirectionDropdown.value = 4;

        screenDistanceString = (pl.patientToScreenDistance).ToString();
        vrController = GetComponent<VRController>();
    }

    void OnGUI()
    {
        Rect Label1 = new Rect(Screen.width / 10, Screen.height / 10, Screen.width / 8, Screen.height / 10);
        Rect Content1 = new Rect(Screen.width / 10, Screen.height / 5 + 10, Screen.width / 8, Screen.height / 15);
        Rect Label2 = new Rect(Screen.width * 3 / 10, Screen.height / 10, Screen.width / 8, Screen.height / 10);
        Rect Content2 = new Rect(Screen.width * 3 / 10, Screen.height / 6, Screen.width / 8, Screen.height / 10);
        Rect Label3 = new Rect(Screen.width * 5 / 10, Screen.height / 10, Screen.width / 8, Screen.height / 10);
        Rect Content3 = new Rect(Screen.width * 5 / 10, Screen.height / 5 + 10, Screen.width / 8, Screen.height / 10);
        Rect Label4 = new Rect(Screen.width * 7 / 10, Screen.height / 10, Screen.width / 8, Screen.height / 10);
        Rect Content4 = new Rect(Screen.width * 7 / 10, Screen.height / 5 + 10, Screen.width / 8, Screen.height / 10);
        Rect Label5 = new Rect(Screen.width / 10, Screen.height * 5 / 10, Screen.width / 8, Screen.height / 10);
        Rect Content5 = new Rect(Screen.width / 10, Screen.height * 6 / 10 + 10, Screen.width / 8, Screen.height / 10);
        Rect Label6 = new Rect(Screen.width * 3 / 10, Screen.height * 5 / 10, Screen.width / 8, Screen.height / 10);
        Rect Content6 = new Rect(Screen.width * 3 / 10, Screen.height * 6 / 10 + 10, Screen.width / 8, Screen.height / 10);
        Rect Label7 = new Rect(Screen.width * 5 / 10, Screen.height * 5 / 10, Screen.width / 8, Screen.height / 10);
        Rect Content7 = new Rect(Screen.width * 5 / 10, Screen.height * 6 / 10 + 10, Screen.width / 8, Screen.height / 10);
        Rect Label8 = new Rect(Screen.width * 7 / 10, Screen.height * 5 / 10, Screen.width / 8, Screen.height / 10);
        Rect Content8 = new Rect(Screen.width * 7 / 10, Screen.height * 6 / 10 + 10, Screen.width / 8, Screen.height / 10);

        // CAN GET RID OF SHOWCONFIGURATION BUTTON AND PUT WITH SHOWPLAYMODES
        if (showConfigurationButton && GUI.Button(new Rect((int) Screen.width * 6.5f / 10, Screen.height * 8 / 10, Screen.width / 8, Screen.height / 10), "Configure"))
        {
            showConfigurationButton = false;
            showConfigurationPanel = true;
            showPlayModes = false;
        }
        // showConfigurationButton = false;
        if (showPlayModes)
        {
            if (GUI.Button(new Rect(Screen.width * 8 / 10, Screen.height * 8 / 10, Screen.width / 8, Screen.height / 10), "Quit"))
            {
                // polhemusController.quitPlStream();
                // Destroy(polhemusController);
                Application.Quit();
            }
            if (GUI.Button(new Rect(Screen.width / 6, Screen.height * 2 / 5, Screen.width / 7, Screen.height / 10), "Training"))
            {
                showPlayModes = false;
                showConfigurationButton = false;
                SceneManager.LoadSceneAsync("Train", LoadSceneMode.Single);
            }

            if (GUI.Button(new Rect(Screen.width * 2 / 6, Screen.height * 2 / 5, Screen.width / 7, Screen.height / 10), "Static Acuity"))
            {
                showPlayModes = false;
                showConfigurationButton = false;
                SceneManager.LoadSceneAsync("Static", LoadSceneMode.Single);
            }

            if (GUI.Button(new Rect(Screen.width * 3 / 6, Screen.height * 2 / 5, Screen.width / 7, Screen.height / 10), "Dynamic Acuity"))
            {
                showPlayModes = false;
                showConfigurationButton = false;
                SceneManager.LoadSceneAsync("DynamicVR", LoadSceneMode.Single);
            }

            if (GUI.Button(new Rect(Screen.width * 4 / 6, Screen.height * 2 / 5, Screen.width / 7, Screen.height / 10), "Game"))
            {
                showPlayModes = false;
                showConfigurationButton = false;
                SceneManager.LoadSceneAsync("NewGamePlayScene", LoadSceneMode.Single);
            }
            //if (GUI.Button (new Rect ()))

        }
        if (showConfigurationPanel == true)
        {
            bool isNumeric;

            GUIStyle optionStyle = new GUIStyle();
            optionStyle.normal.textColor = Color.red;
            optionStyle.fontSize = 16;
            optionStyle.alignment = TextAnchor.MiddleCenter;

            // Upper Left  -  Head Direction for enabled directions that the head can be moved to track head Speed Thresholds 
            GUI.Box(Label1, "Head Direction\n", optionStyle);
            rectTransformReconstruct(headDirectionDropdown.gameObject.GetComponent<RectTransform>(), Content1);
            headDirectionDropdown.gameObject.SetActive(true);

            // Upper Middle Left - Patient to Screen Distance to determine optotype size and scene motion
            GUI.Box(Label2, "Screen Distance (m)\n", optionStyle);
            screenDistanceString = GUI.TextField(Content2, screenDistanceString, optionStyle);
            screenDistanceString = Regex.Replace(screenDistanceString, @"[^0-9.]", "");
            float f;
            isNumeric = float.TryParse(screenDistanceString, out f);
            if (isNumeric)
            {
                pl.patientToScreenDistance = f; //* PreferenceLoader.cm2ft; // convert to inches
            }
            else if (screenDistanceString != "")
            {
                screenDistanceString = (pl.patientToScreenDistance /* PreferenceLoader.cm2ft*/).ToString();
            }

            // Upper Middle Right - Initial optotype size in logMAR
            GUI.Box(Label3, "Initial Optotype Size (logMAR)\n\n" + pl.svaTestResult, optionStyle);
            pl.svaTestResult = Mathf.Round(10f * GUI.HorizontalSlider(Content3, pl.svaTestResult, 0f, 1f)) / 10f;

            // Upper Right - Scene gain
            GUI.Box(Label4, "Scene Gain\n\n" + pl.sceneGain, optionStyle);
            //pl.sceneGain = Mathf.Round(10f * GUI.HorizontalSlider(Content4, pl.sceneGain, -1f, 1f)) / 10f;
            pl.sceneGain = (GUI.HorizontalSlider(Content4, pl.sceneGain, -1f, 1f));

            // Center Left - Head Speed Threshold
            GUI.Box(Label5, "Speed Threshold\n\n" + pl.dvaHeadSpeedTriggerThreshold, optionStyle);
            pl.dvaHeadSpeedTriggerThreshold = Mathf.Round(GUI.HorizontalSlider(Content5, pl.dvaHeadSpeedTriggerThreshold, 0f, 200f));

            // Center Middle Left - Velocity Look Back Frames
            GUI.Box(Label6, "Lookback Frames\n\n" + pl.dvaLookBackAmount, optionStyle);
            pl.dvaLookBackAmount = (int)Mathf.Round(GUI.HorizontalSlider(Content6, pl.dvaLookBackAmount, 0f, 20f));

            // Center Middle Right - Lookback Window Lower Bound
            GUI.Box(Label7, "Lookback Lower Bound\n\n" + pl.dvaLowerHeadSpeedWindow, optionStyle);
            pl.dvaLowerHeadSpeedWindow = Mathf.Round(GUI.HorizontalSlider(Content7, pl.dvaLowerHeadSpeedWindow, 0f, 200f));

            // Center Right - Lookback Window Upper Bound
            GUI.Box(Label8, "Lookback Upper Bound\n\n" + pl.dvaUpperHeadSpeedWindow, optionStyle);
            pl.dvaUpperHeadSpeedWindow = Mathf.Round(GUI.HorizontalSlider(Content8, pl.dvaUpperHeadSpeedWindow, 0f, 200f));

            if (GUI.Button(new Rect(Screen.width * 6 / 10, Screen.height * 8 / 10, Screen.width / 8, Screen.height / 10), "Restore Defaults"))
            {
                Destroy(GameObject.Find("PreferenceLoader"));
                GameObject obj = Instantiate(Resources.Load("PreferenceLoader")) as GameObject;
                obj.name = "PreferenceLoader";
                pl = GameObject.Find("PreferenceLoader").GetComponent<PreferenceLoader>(); // Reload PreferenceLoader
            }
            if (GUI.Button(new Rect(Screen.width * 8 / 10, Screen.height * 8 / 10, Screen.width / 8, Screen.height / 10), "Return"))
            {
                headDirectionDropdown.gameObject.SetActive(false);
                showConfigurationPanel = false;
                showConfigurationButton = true;
                showPlayModes = true;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    // Helper function used in reconstruct postion for dropdown menus
    void rectTransformReconstruct(RectTransform trans, Rect position)
    {
        trans.sizeDelta = new Vector2(position.width, position.height);
        trans.anchoredPosition = new Vector2(position.x + position.width / 2, -position.y);
    }

    public void headDirectionDropdownEventHandler(int index)
    {
        pl.headDirection = index;
    }
}
