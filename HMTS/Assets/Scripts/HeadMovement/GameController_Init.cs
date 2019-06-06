using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController_Init : MonoBehaviour {

    public GameObject DataController_pref;

	// Use this for initialization
	void Start () {
        instant_DataController();

        SceneManager.LoadScene("HeadMovement");
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private void instant_DataController()
    {
        if (GameObject.Find("DataController") == null)
        {
            GameObject DC_instant = Instantiate(DataController_pref) as GameObject;
            DC_instant.name = "DataController";
        }
    }
}
