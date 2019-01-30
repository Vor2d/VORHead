using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SC_GameController : GeneralGameController {

    [SerializeField] private GameObject sceneManager_prefab;

    //[SerializeField] private bool UsingHeadToMenu = false;

	// Use this for initialization
	void Start () {
		if(GameObject.Find("SceneManager") == null)
        {
            sceneManager_prefab = Instantiate(sceneManager_prefab, new Vector3(), new Quaternion());
            sceneManager_prefab.name = "SceneManager";
        }
	}
	
	// Update is called once per frame
	void Update () {
		
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
}
