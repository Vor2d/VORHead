using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using HMTS_enum;

public class DataController : ParentDataController {

    private const string trial_path = "Default.txt";
    private const string ECTrial_path = "ECTrial.txt";
    private readonly char[] line_spliter = new char[] { ' ', '\t' };

    public List<Section> Sections { get; set; }
    public GameMode Current_GM { get; set; }
    public TrialInfo Current_TI { get; set; }
    public GameSetting SystemSetting { get; set; }
    public EyeInfo Eye_info { get; set; }
    public TrialInfo Eye_TI { get; set; }

    //System data;
    public EyeFitMode FitMode;
    public EyeFunction FitFunction;

    public Quaternion Head_origin { get; set; }

    private Dictionary<string, string> init_data;

    protected override void Awake()
    {
        base.Awake();

        this.init_data = new Dictionary<string, string>();
        this.Head_origin = new Quaternion(0.0f,0.0f,0.0f,1.0f);
        this.Current_GM = new GameMode();
        this.Current_TI = new TrialInfo();
        this.SystemSetting = new GameSetting();
        this.Eye_info = new EyeInfo();
        Eye_info.set_model(FitFunction);
        this.Eye_TI = new TrialInfo();

        Sections = GeneralMethods.load_game_data_general(trial_path);
        SystemSetting = GeneralMethods.read_game_setting_general(setting_path+setting_file_name);
        read_eye_trials();
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

    public void read_eye_trials()
    {
        Debug.Log("Loading ECTrial_path " + ECTrial_path);
        try
        {
            StreamReader reader = new StreamReader(ECTrial_path);
            string[] line_str;
            string x_degree = "";
            string y_degree = "";
            while (!reader.EndOfStream)
            {
                line_str = reader.ReadLine().Split(line_spliter);
                x_degree = line_str[0];
                y_degree = line_str[1];
                Eye_TI.Turn_data.
                    Add(new Vector2(float.Parse(x_degree),float.Parse(y_degree)));
            }
        }
        catch (Exception e) { Debug.Log(e); }
        Debug.Log("Loading ECTrial_path finished");
    }



}
