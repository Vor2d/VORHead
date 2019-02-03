using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class MD_DataController : ParentDataController
{
    private static char[] file_spliter = new char[] { ' ', '\t' };
    private const string game_data_path = "MD_GameData.txt";

    public MD_WaveInfo Wave_info { get; set; }
    public Dictionary<MD_GameData, string> GameData_Dict;

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    // Use this for initialization
    void Start () {
        init_DC();

        this.Wave_info = new MD_WaveInfo();
        this.GameData_Dict = new Dictionary<MD_GameData, string>();

        load_game_data(game_data_path);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void load_game_data(string path)
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
}

//MS: Missile speed; MR: Missile rate;
public enum MD_GameData
{
    MissileSpeed,MissileRate,AmmoOffset, MSDifficultyIncrease, MRDifficultyIncrease,
    MissileNumberEachWave
};