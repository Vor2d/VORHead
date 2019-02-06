using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class MD_DataController : ParentDataController
{
    private static char[] file_spliter = new char[] { ' ', '\t' };
    private const string game_data_path = "MD_GameData.txt";

    public MD_WaveInfo Wave_info { get; set; }
    public Dictionary<MD_GameData, string> GameData_Dict { get; private set; }

    [Header("Variables")]
    public bool ReadDataFromFile = false;
    [Header("Missile")]
    public float MissileSpeed = 0.2f;
    public bool OneMissilePreTime = false;
    public float MissileInterTime = 3.0f;
    [Header("Explode")]
    public float ExplodeRaduis = 3.0f;
    public float ExplodeTime = 1.5f;
    public bool UsingExplodeOutline = false;
    [Header("Random")]
    public bool UsingRandomSeed = false;
    public int RandomSeed = 0;
    [Header("InfiniteWaves")]
    public bool InfiniteWaves = false;
    public float MSDifficultyIncrease = 0.1f;
    public float MRDifficultyIncrease = 0.1f;
    public int MissileNumberEachWave;
    [Header("AmmoSystem")]
    public bool UsingAutoAmmoNumber = false;
    public int AmmoOffSet = 0;
    public int AmmoConstant = 10;
    public bool UsingDualHeadIndicator = false;
    [Header("ReloadSystem")]
    public bool UsingReloadSystem = false;
    public bool UsingReloadAutoNumber = false;
    public int ReloadAmmoOffset = 0;
    public int ReloadAmmoNumber = 0;

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    // Use this for initialization
    void Start () {
        init_DC();

        this.Wave_info = new MD_WaveInfo();
        this.GameData_Dict = new Dictionary<MD_GameData, string>();

        if (ReadDataFromFile)
        {
            load_game_data_file(game_data_path);
            parse_variables();
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void load_game_data_file(string path)
    {
        int line_counter = -1;
        StreamReader reader = new StreamReader(path);
        while (!reader.EndOfStream)
        {
            line_counter++;
            string[] splitstr = reader.ReadLine().Split(file_spliter);
            switch(splitstr[0])
            {
                case "Wave":
                    break;
                case "MissileSpeed":
                    GameData_Dict.Add(MD_GameData.MissileSpeed, splitstr[1]);
                    break;
                case "MissileRate":
                    GameData_Dict.Add(MD_GameData.MissileRate, splitstr[1]);
                    break;
                case "AmmoOffset":
                    GameData_Dict.Add(MD_GameData.AmmoOffset, splitstr[1]);
                    break;
                case "MSDifficultyIncrease":
                    GameData_Dict.Add(MD_GameData.MSDifficultyIncrease, splitstr[1]);
                    break;
                case "MRDifficultyIncrease":
                    GameData_Dict.Add(MD_GameData.MRDifficultyIncrease, splitstr[1]);
                    break;
                case "MissileNumberEachWave":
                    GameData_Dict.Add(MD_GameData.MissileNumberEachWave, splitstr[1]);
                    break;
            }
        }
    }

    private void parse_variables()
    {
        try
        {
            MissileInterTime =
                    float.Parse(GameData_Dict[MD_GameData.MissileRate]);
        }
        catch { }
        try
        {
            MissileSpeed =
                    float.Parse(GameData_Dict[MD_GameData.MissileSpeed]);
        }
        catch { }
        try
        {
            AmmoOffSet =
                    int.Parse(GameData_Dict[MD_GameData.AmmoOffset]);
        }
        catch { }
        try
        {
            MSDifficultyIncrease =
                float.Parse(GameData_Dict[MD_GameData.MSDifficultyIncrease]);
        }
        catch { }
        try
        {
            MRDifficultyIncrease =
                float.Parse(GameData_Dict[MD_GameData.MRDifficultyIncrease]);
        }
        catch { }
        try
        {
            MissileNumberEachWave =
                    int.Parse(GameData_Dict[MD_GameData.MissileNumberEachWave]);
        }
        catch { }
    }

    public string var_to_string()
    {
        string result_str = "";

        result_str += "Missile Rate " + MissileInterTime.ToString("F2") + " ";
        result_str += "Missile Speed " + MissileSpeed.ToString("F2") + " ";

        return result_str;
    }
}

//MS: Missile speed; MR: Missile rate;
public enum MD_GameData
{
    MissileSpeed,MissileRate,AmmoOffset, MSDifficultyIncrease, MRDifficultyIncrease,
    MissileNumberEachWave
};