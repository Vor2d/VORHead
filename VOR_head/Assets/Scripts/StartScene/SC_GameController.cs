using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SC_GameController : GeneralGameController {

    [SerializeField] private GameObject sceneManager_prefab;
    [SerializeField] private Controller_Input CI_script;

    private GameObject scene_manager_OBJ;

    //[SerializeField] private bool UsingHeadToMenu = false;

    // Use this for initialization
    void Start () {
        GameObject scene_manager_temp = GameObject.Find("SceneManager");
        if (scene_manager_temp == null)
        {
            scene_manager_OBJ = 
                        Instantiate(sceneManager_prefab, new Vector3(), new Quaternion());
            scene_manager_OBJ.name = "SceneManager";
        }
        else
        {
            scene_manager_OBJ = scene_manager_temp;
        }

        
        if(scene_manager_OBJ.GetComponent<MySceneManager>().using_VR)
        {
            CI_script.Button_Y += recenter_VR;
        }
	}

    public void to_HMTS_scene()
    {
        SceneManager.LoadScene("HeadMInit");
    }

    public void to_BP_scene()
    {
        SceneManager.LoadScene("LoadBubblePoke");
    }

    public void to_MD_scene()
    {
        SceneManager.LoadScene("LoadMissileDefence");
    }

    public void to_TDMD_scene()
    {
        SceneManager.LoadScene("LoadTDMissileDefence");
    }

    public void to_BO_scene()
    {
        SceneManager.LoadScene("LoadBreakout");
    }

    public void to_FS_scene()
    {
        SceneManager.LoadScene("LoadFruitSlice");
    }

    public void to_WAMVR_scene()
    {
        SceneManager.LoadScene("LoadWAMVR");
    }
}
