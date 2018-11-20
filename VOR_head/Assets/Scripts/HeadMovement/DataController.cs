using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class DataController : ParentDataController {

    private const string trial_path = "Default.txt";
    private const string setting_path = "SettingDefault.txt";

    public List<Section> Sections { get; set; }
    public GameMode Current_GM { get; set; }
    public TrialInfo Current_TI { get; set; }
    public GameSetting SystemSetting { get; set; }

    //System data;

    public Quaternion Head_origin { get; set; }

    private Dictionary<string, string> init_data;

    void Awake()
    {
        DontDestroyOnLoad(this);

        init_DC();

        this.init_data = new Dictionary<string, string>();
        this.Head_origin = new Quaternion(0.0f,0.0f,0.0f,1.0f);
        this.Current_GM = new GameMode();
        this.Current_TI = new TrialInfo();
        this.SystemSetting = new GameSetting();

        Sections = GeneralMethods.load_game_data_general(trial_path);
        SystemSetting = GeneralMethods.read_game_setting_general(setting_path);


    }

    private void Start()
    {
        Debug.Log("SystemSetting " + SystemSetting.VarToString());

        foreach (Section section in Sections)
        {
            Debug.Log("GM " + section.SectionGameMode.VarToString());
            Debug.Log("-------------------");
        }

        foreach (Section section in Sections)
        {
            Debug.Log(section.SectionTrialInfo.VarToString());
            Debug.Log("-------------------");
        }
    }




}
