using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class DataController : MonoBehaviour {

    private const string trial_path = "Default.txt";
    private const string setting_path = "SettingDefault.txt";

    public List<Section> Sections { get; set; }
    public GameMode Current_GM { get; set; }
    public TrialInfo Current_TI { get; set; }

    //System data;
    public string Camera1_display { get; set; }
    public string Camera2_display { get; set; }
    public float Player_screen_cm { get; set; }
    public float Screen_width_cm { get; set; }
    public bool UsingCoilFlag { get; set; }
    public bool UsingVRFlag { get; set; }
    public Quaternion Head_origin { get; set; }

    private Dictionary<string, string> init_data;

    void Awake()
    {
        DontDestroyOnLoad(this);

        this.UsingCoilFlag = false;
        this.UsingVRFlag = true;

        this.init_data = new Dictionary<string, string>();
        this.Head_origin = new Quaternion(0.0f,0.0f,0.0f,1.0f);
        this.Current_GM = new GameMode();
        this.Current_TI = new TrialInfo();

        try
        {
            StreamReader reader = new StreamReader(setting_path);
            while (!reader.EndOfStream)
            {
                string[] splitstr = reader.ReadLine().Split(new char[]{' ', '\t'});
                init_data.Add(splitstr[0], splitstr[1]);
            }
            reader.Close();

            this.Player_screen_cm = float.Parse(init_data["PlayerScreenCM"]);
            this.Screen_width_cm = float.Parse(init_data["ScreenWidthCM"]);
            this.Camera1_display = init_data["Camera1Display"];
            this.Camera2_display = init_data["Camera2Display"];
        }
        catch(Exception e)
        {
            Debug.Log(e);

            this.Player_screen_cm = 100.0f;
            this.Screen_width_cm = 50f;
            this.Camera1_display = "0";
            this.Camera2_display = "1";
        }

        Sections = GeneralMethods.load_game_data_general(trial_path);


    }

    private void Start()
    {
        foreach (Section section in Sections)
        {
            Debug.Log("GM "+section.SectionGameMode.VarToString());
            foreach(float turn in section.SectionTrialInfo.Turn_data)
            {
                Debug.Log("TD" + turn);
            }
            Debug.Log("------------------------");
        }
    }

    public string VarToString()
    {
        string result_str = "";

        result_str += "Player_screen_cm" + " " + Player_screen_cm.ToString() + " ";
        result_str += "Screen_width_cm" + " " + Screen_width_cm.ToString() + " ";
        result_str += "Camera1_display" + " " + Camera1_display.ToString() + " ";
        result_str += "Camera2_display" + " " + Camera2_display.ToString() + " ";

        return result_str;
    }


}
