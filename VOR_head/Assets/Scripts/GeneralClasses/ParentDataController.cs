using UnityEngine;
using System.IO;
using System;

/// <summary>
/// Persistence class, will contain a single game data for the game play;
/// </summary>
public class ParentDataController : MonoBehaviour {

    //Customize it for each data controller;
    public string setting_file_name = "Setting.json";
    public string setting_path = "";

    //Flags to use to turn on and off the system;
    public bool using_VR { get; set; }
    public bool using_coil { get; set; }

    public MySceneManager MSM_script { get; private set; }

    public Guid PDC_ID { get; private set; }

    protected virtual void Awake()
    {
        DontDestroyOnLoad(gameObject);

        init_DC();
    }

    public virtual void init_DC()
    {
        PDC_ID = Guid.NewGuid();

        GameObject temp_SM_OBJ = GameObject.Find(GeneralStrDefiner.SceneManagerGO_name);
        if(temp_SM_OBJ != null)
        {
            MSM_script = temp_SM_OBJ.GetComponent<MySceneManager>();
            this.using_VR = MSM_script.using_VR;
            this.using_coil = MSM_script.using_coil;
        }
    }

    public virtual T load_setting<T>()
    {
        return GeneralMethods.load_setting<T>(setting_path, setting_file_name);
    }

    public virtual void generate_setting<T>(T setting_class)
    {
        GeneralMethods.generate_setting<T>(setting_class, setting_path, setting_file_name);
    }

    public virtual void generate_setting()
    {
        Debug.Log("Class " + name + " generate_setting not implemented");
    }

}
