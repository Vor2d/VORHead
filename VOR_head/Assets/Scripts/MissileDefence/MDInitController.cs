using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MDInitController : MonoBehaviour {

    public GameObject MDDC_Prefab;

    private GameObject MDDC_GO;


    // Use this for initialization
    void Start () {
        instant_MDDataController();

        SceneManager.LoadScene("MissileDefence");

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private void instant_MDDataController()
    {
        if (GameObject.Find("MD_DataController") == null)
        {
            MDDC_GO = Instantiate(MDDC_Prefab) as GameObject;
            MDDC_GO.name = "MD_DataController";
        }
    }
}
