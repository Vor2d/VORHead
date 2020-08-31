using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.UI;
using System.Net.Sockets;
using System.Net;
using System.Text;

public class GameController_Setting : MonoBehaviour {

    private static readonly byte[] SIM_RZ_byte = new byte[] { 123};
    private static readonly IPAddress SIM_RZ_IP = 
                                    new IPAddress(new byte[] { 192, 168, 2, 50 });
    private const int SIM_RZ_port = 8001;
    private List<string> camera_list_str = new List<string>() { "0", "1", "2", "3", "4" };

    public GameObject Game_Controller;
    public GameObject Page1;
    public GameObject Page2;
    public GameObject Page3;
    public GameObject UIIndicatorText1_OBJ;
    [SerializeField] private Transform EyeIndi_TRNAS1;
    [SerializeField] private Transform EyeIndi_TRNAS2;
    //Page1;
    [Header("Page1")]
    //public Toggle LoopTrialsToggle;
    public Text ReadFileIndicator;
    public InputField PTS_IF;   //Player to screen input field;
    public InputField SW_IF;    //Screen width;
    public Dropdown Camera1_DD; //Camera dropdown menu;
    public Dropdown Camera2_DD;
    public Camera camera1;
    public Camera camera2;
    public Camera camera3;
    public InputField LoopNumber_IF;
    //Page2;
    [Header("Page2")]
    public Toggle UsingCurvedScreenToggle;
    public Toggle HideFlagToggle;
    public Toggle JumpFlagToggle;
    public Toggle ShowTargetFlagToggle;
    public Toggle HeadIndicatorChangeToggle;
    public Toggle SkipCenterFlagToggle;
    public Toggle HideHeadIndicatorToggle;
    public Toggle ChangeTargetByTimeToggle;
    public InputField GazeTime_IF;
    public InputField HideTime_IF;
    public InputField ErrorTime_IF;
    public InputField SpeedThreshold_IF;
    public InputField StopWinodow_IF;
    public InputField RandomGazeTime_IF;
    public InputField Gain_IF;
    public InputField TargetChangeTime_IF;
    public InputField TargetChangeTimeRRange_IF;

    //Variables;
    private DataController DC_script;
    private string path;
    private List<Vector2> turn_data;
    private List<Vector2> jump_data;
    private List<GameObject> pages;
    private int current_page;
    private GameController GC_script;
    private bool eye_enabled_flag;

    // Use this for initialization
    void Start () {
        this.DC_script = GameObject.Find("DataController").GetComponent<DataController>();
        this.path = "";
        this.turn_data = new List<Vector2>();
        this.jump_data = new List<Vector2>();
        this.pages = new List<GameObject>();
        this.current_page = 0;
        this.GC_script = Game_Controller.GetComponent<GameController>();
        this.camera1.targetDisplay = Int32.Parse(DC_script.SystemSetting.Camera1_display);
        this.camera2.targetDisplay = Int32.Parse(DC_script.SystemSetting.Camera2_display);
        if (Display.displays.Length == 3)
        {
            this.camera3.targetDisplay = Int32.Parse(DC_script.SystemSetting.Camera3_display);
        }
        else { this.camera3.gameObject.SetActive(false); }
        
        this.eye_enabled_flag = EyeIndi_TRNAS1.GetComponent<MeshRenderer>().enabled;

        UIIndicatorText1_OBJ.GetComponent<Text>().text = "";

        init_dropdown(Camera1_DD, camera_list_str);
        init_dropdown(Camera2_DD, camera_list_str);
        pages.Add(Page1);
        pages.Add(Page2);
        pages.Add(Page3);
        init_pages();

        update_page1();

        //set_cam_fov(camera1, camera2);
    }

    // Update is called once per frame
    void Update () {

	}

    private void set_cam_fov(Camera Mcamera,Camera Scamera)
    {
        float fov = Mcamera.fieldOfView;
        //Scamera.fieldOfView = fov * DC_script.SystemSetting.CameraFOVFactor;
    }

    public void ToFinish()
    {
        UIIndicatorText1_OBJ.SetActive(true);
        UIIndicatorText1_OBJ.GetComponent<Text>().text = "Finished!";
    }

    public void apply_change_page2()
    {
        try
        {
            apply_game_mode();
            apply_variable_IF();
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }

        //update_page2();
        GC_script.update_data_restart();
    }

    public void apply_change_page1()
    {
        try
        {
            apply_screen_IF();
            apply_camerachange();
            //DC_script.Loop_trial_flag = LoopTrialsToggle.isOn;
            DC_script.Current_TI.Loop_number = Int32.Parse(LoopNumber_IF.text);
        }
        catch(Exception e)
        {
            Debug.Log(e);
        }

        //update_page1();
        GC_script.update_data_restart();
    }

    public void next_page()
    {
        current_page++;
        if(current_page >= pages.Count)
        {
            current_page = 0;
        }
        for (int i = 0; i < pages.Count; i++)
        {
            if (i == current_page)
            {
                pages[i].SetActive(true);
            }
            else
            {
                pages[i].SetActive(false);
            }
        }

        Debug.Log("current page" + current_page);
        switch(current_page)
        {
            case 0:
                update_page1();
                break;
            case 1:
                update_page2();
                break;
        }
    }

    [Obsolete("Not Using file browser anymore")]
    public void load_turn_data()
    {

    }

    private void apply_game_mode()
    {
        DC_script.Current_GM.HideFlag = HideFlagToggle.isOn;
        DC_script.Current_GM.JumpFlag = JumpFlagToggle.isOn;
        DC_script.Current_GM.ShowTargetFlag = ShowTargetFlagToggle.isOn;
        DC_script.Current_GM.HeadIndicatorChange = HeadIndicatorChangeToggle.isOn;
        DC_script.Current_GM.SkipCenterFlag = SkipCenterFlagToggle.isOn;
        DC_script.Current_GM.HideHeadIndicator = HideHeadIndicatorToggle.isOn;
        DC_script.Current_GM.ChangeTargetByTime = ChangeTargetByTimeToggle.isOn;
    }

    private void apply_variable_IF()
    {
        try
        {
            DC_script.SystemSetting.GazeTime = float.Parse(GazeTime_IF.text);
            DC_script.SystemSetting.HideTime = float.Parse(HideTime_IF.text);
            DC_script.SystemSetting.ErrorTime = float.Parse(ErrorTime_IF.text);
            DC_script.SystemSetting.SpeedThreshold = float.Parse(SpeedThreshold_IF.text);
            DC_script.SystemSetting.StopWinodow = float.Parse(StopWinodow_IF.text);
            DC_script.SystemSetting.RandomGazeTime = float.Parse(RandomGazeTime_IF.text);
            DC_script.SystemSetting.TargetChangeTime = float.Parse(TargetChangeTime_IF.text);
            DC_script.SystemSetting.TargetChangeTimeRRange = 
                                                float.Parse(TargetChangeTimeRRange_IF.text);

            DC_script.Current_GM.Gain = float.Parse(Gain_IF.text);
        }
        catch(Exception e)
        {
            Debug.Log(e);
        }
    }

    private void apply_camerachange()
    {
        string camera1_dis = Camera1_DD.captionText.text;
        string camera2_dis = Camera2_DD.captionText.text;

        camera1.targetDisplay = Int32.Parse(camera1_dis);
        camera2.targetDisplay = Int32.Parse(camera2_dis);

        DC_script.SystemSetting.Camera1_display = camera1_dis;
        DC_script.SystemSetting.Camera2_display = camera2_dis;
    }

    private void apply_screen_IF()
    {
        try
        {
            DC_script.SystemSetting.Using_curved_screen = UsingCurvedScreenToggle.isOn;
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
        try
        {
            DC_script.SystemSetting.Player_screen_cm = float.Parse(PTS_IF.text);
        }
        catch(Exception e)
        {
            Debug.Log(e);
        }
        try
        {
            DC_script.SystemSetting.Screen_width_cm = float.Parse(SW_IF.text);
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }

    }

    private void update_page1()
    {
        UsingCurvedScreenToggle.isOn = DC_script.SystemSetting.Using_curved_screen;
        PTS_IF.text = DC_script.SystemSetting.Player_screen_cm.ToString();
        SW_IF.text = DC_script.SystemSetting.Screen_width_cm.ToString();
        try
        {
            Camera1_DD.value = Int32.Parse(DC_script.SystemSetting.Camera1_display);
            Camera2_DD.value = Int32.Parse(DC_script.SystemSetting.Camera2_display);
        }
        catch(Exception e)
        {
            Debug.Log(e);
        }
        //LoopTrialsToggle.isOn = DC_script.Loop_trial_flag;
        LoopNumber_IF.text = DC_script.Current_TI.Loop_number.ToString();
    }

    private void update_page2()
    {
        HideFlagToggle.isOn = DC_script.Current_GM.HideFlag;
        JumpFlagToggle.isOn = DC_script.Current_GM.JumpFlag;
        ShowTargetFlagToggle.isOn = DC_script.Current_GM.ShowTargetFlag;
        HeadIndicatorChangeToggle.isOn = DC_script.Current_GM.HeadIndicatorChange;
        SkipCenterFlagToggle.isOn = DC_script.Current_GM.SkipCenterFlag;
        HideHeadIndicatorToggle.isOn = DC_script.Current_GM.HideHeadIndicator;
        ChangeTargetByTimeToggle.isOn = DC_script.Current_GM.ChangeTargetByTime;
        try
        {
            GazeTime_IF.text = DC_script.SystemSetting.GazeTime.ToString("F2");
            HideTime_IF.text = DC_script.SystemSetting.HideTime.ToString("F2");
            ErrorTime_IF.text = DC_script.SystemSetting.ErrorTime.ToString("F2");
            SpeedThreshold_IF.text = DC_script.SystemSetting.SpeedThreshold.ToString("F2");
            StopWinodow_IF.text = DC_script.SystemSetting.StopWinodow.ToString("F2");
            RandomGazeTime_IF.text = DC_script.SystemSetting.RandomGazeTime.ToString("F2");
            Gain_IF.text = DC_script.Current_GM.Gain.ToString("F2");
            TargetChangeTime_IF.text = DC_script.SystemSetting.TargetChangeTime.ToString("F2");
            TargetChangeTimeRRange_IF.text = 
                            DC_script.SystemSetting.TargetChangeTimeRRange.ToString("F2");
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
        
    }

    private void init_pages()
    {
        for(int i = 0;i<pages.Count;i++)
        {
            if(i == 0)
            {
                pages[i].SetActive(true);
            }
            else
            {
                pages[i].SetActive(false);
            }
        }
    }

    private void load_turn_jump_data(string path)
    {
        try
        {
            //GeneralMethods.load_turn_jump_data_general(path,out turn_data, out jump_data);
        }
        catch (Exception e)
        {
            Debug.Log("Reading file error! " + e);
            ReadFileIndicator.text = e.ToString();
        }
        Debug.Log("Loading complete! ");
        ReadFileIndicator.text = "Loading complete!";

        DC_script.Current_TI.Turn_data = new List<Vector2>(this.turn_data);
        DC_script.Current_TI.Jump_data = new List<Vector2>(this.jump_data);
    }

    private void display_list(IEnumerable list)
    {
        int counter = 0;
        foreach(var element in list)
        {
            Debug.Log(counter + " " + element.ToString());
            counter++;
        }
    }

    private void init_dropdown(Dropdown dropdown, List<string> content)
    {
        dropdown.ClearOptions();
        dropdown.AddOptions(content);
    }

    public void change_indicate_text(string text)
    {
        UIIndicatorText1_OBJ.SetActive(true);
        UIIndicatorText1_OBJ.GetComponent<Text>().text = "Pause for: " + text;
    }

    public void turn_off_text()
    {
        UIIndicatorText1_OBJ.SetActive(false);
    }

    public void ToECSceneButton()
    {
        SceneManager.LoadScene("EyeCalibration");
    }

    public void ToggleReddotButton()
    {
        if(GC_script.HeadIndicator.GetComponent<MeshRenderer>().enabled)
        {
            GC_script.HeadIndicator.GetComponent<MeshRenderer>().enabled = false;
        }
        else
        {
            GC_script.HeadIndicator.GetComponent<MeshRenderer>().enabled = true;
        }
    }

    public void BackToMainMenuButton()
    {
        GC_script.back_to_main_menu();
    }

    public void ToggleEyeButton()
    {
        if (eye_enabled_flag)
        {
            EyeIndi_TRNAS1.GetComponent<MeshRenderer>().enabled = false;
            EyeIndi_TRNAS2.GetComponent<MeshRenderer>().enabled = false;
            eye_enabled_flag = false;
        }
        else
        {
            EyeIndi_TRNAS1.GetComponent<MeshRenderer>().enabled = true;
            EyeIndi_TRNAS2.GetComponent<MeshRenderer>().enabled = true;
            eye_enabled_flag = true;
        }
    }

    public void rezero_simulink()
    {
        GeneralMethods.send_udp(SIM_RZ_IP, SIM_RZ_port, SIM_RZ_byte);
    }

    public void ExitButton()
    {
        Application.Quit();
    }

    public void RecenterMinitor2Button()
    {
        GC_script.recenter_monitor2();
    }
}
