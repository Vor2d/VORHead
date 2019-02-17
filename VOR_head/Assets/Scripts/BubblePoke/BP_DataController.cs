using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BP_DataController : ParentDataController
{
    private const string file_path = "BP_TrialData.txt";

    public BP_GameMode GameMode = BP_GameMode.Random;
    public BP_TrailInfo trial_info { get; set; }

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    // Use this for initialization
    void Start () {
        init_DC();

        this.trial_info = new BP_TrailInfo();
        read_data_fromfile(file_path);
    }
	
	// Update is called once per frame
	void Update () {

	}

    private void read_data_fromfile(string path)
    {
        if(GameMode == BP_GameMode.UsingFile)
        {
            trial_info.set_deg_info(GeneralMethods.read_trial_file_VNH(path));
        }
    }
}

public enum BP_GameMode { UsingFile, Random };