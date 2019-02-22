﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MySceneManager : MonoBehaviour {

    private readonly string[] DC_names = {"DataController","BP_DataController",
                        "MD_DataController","TDMD_DataController","BO_DataController",
                        "FS_DataController"};

    public bool using_VR;
    public bool using_coil;

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }
	
	// Update is called once per frame
	void Update () {
		
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            to_start_scene();
        }

	}

    public void to_start_scene()
    {
        back_to_start_scene();
        SceneManager.LoadScene("StartScene");
    }

    private void back_to_start_scene()
    {
        foreach(string DC_name in DC_names)
        {
            GameObject temp_DC_obj = GameObject.Find(DC_name);
            if (temp_DC_obj != null)
            {
                Destroy(temp_DC_obj);
            }
            
        }
    }


}
