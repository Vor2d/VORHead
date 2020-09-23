using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BP_GameMode { UsingFile, AlongPath, Random };

public class BP_DataController : ParentDataController
{
    private const string file_path = "BP_TrialData.txt";

    public BP_GameMode GameMode = BP_GameMode.Random;
    public BP_TrailInfo trial_info { get; set; }

    // Use this for initialization
    void Start () {
        this.trial_info = new BP_TrailInfo();
        if (GameMode == BP_GameMode.UsingFile)
        {
            read_data_fromfile(setting_path + file_path);
        }
    }
	
	// Update is called once per frame
	void Update () {

	}

    private void read_data_fromfile(string path)
    {
        trial_info.set_deg_info(GeneralMethods.read_trial_file_VNH(path));
    }
}