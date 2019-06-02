using System.ComponentModel;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// The most persistence class, will exit for the whole game;
/// </summary>
public class MySceneManager : MonoBehaviour {

    private readonly string[] DC_names = {"DataController","BP_DataController",
                        "MD_DataController","TDMD_DataController","BO_DataController",
                        "FS_DataController","WAMDataController"};

    public bool using_VR;
    public bool using_coil;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
	
	// Update is called once per frame
	void Update () {
		
        //Emergency setup?
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            to_start_scene();
        }

	}

    public void to_start_scene()
    {
        clean_up();
        SceneManager.LoadScene("StartScene");
    }


    private void clean_up()
    {
        foreach(string DC_name in DC_names)
        {
            GameObject temp_DC_OBJ = GameObject.Find(DC_name);
            if (temp_DC_OBJ != null)
            {
                Destroy(temp_DC_OBJ);
            }
            
        }
    }


}
