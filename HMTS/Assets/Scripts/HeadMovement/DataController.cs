using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using HMTS_enum;

public class DataController : ParentDataController {

    private const string acuity_path = "Sprites/Acuity/Transparant Cs/WhiteLandC_V2/";
    private const string trial_path = "Default.txt";
    private const string ECTrial_path = "ECTrial.txt";
    private readonly char[] line_spliter = new char[] { ' ', '\t' };

    public List<Section> Sections { get; set; }
    public GameMode Current_GM { get; set; }
    public TrialInfo Current_TI { get; set; }
    public GameSetting SystemSetting { get; set; }
    public EyeInfo Eye_info { get; set; }
    public TrialInfo Eye_TI { get; set; }
    public Sprite[] Acuity_sprites { get; set; }

    //System data;
    public EyeFitMode FitMode;
    public EyeFunction FitFunction;

    public Quaternion Head_origin { get; set; }

    private Dictionary<string, string> init_data;

    public static DataController IS { get; set; }

    protected override void Awake()
    {
        IS = this;

        base.Awake();

        this.init_data = new Dictionary<string, string>();
        this.Head_origin = new Quaternion(0.0f,0.0f,0.0f,1.0f);
        this.Current_GM = new GameMode();
        this.Current_TI = new TrialInfo();
        this.SystemSetting = new GameSetting();
        this.Eye_info = new EyeInfo();
        Eye_info.set_model(FitFunction);
        this.Eye_TI = new TrialInfo();

    }

    private void Start()
    {
        load_acuity_sprites();

        SystemSetting = load_setting<GameSetting>();
        Debug.Log("GameSetting loaded " + SystemSetting.GetType());

        Sections = GeneralMethods.load_game_data_general(trial_path);
        read_eye_trials();

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

    private void Update()
    {

    }

    public override void generate_setting()
    {
        generate_setting<GameSetting>(SystemSetting);
    }

    private void load_acuity_sprites()
    {
        Debug.Log("Loading acuity sprites " + acuity_path);
        try
        {
            Acuity_sprites = Resources.LoadAll<Sprite>(acuity_path);
            Debug.Log("Acuity sprites loaded!");
        }
        catch { Debug.Log("Acuity sprite failed"); }
        
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
