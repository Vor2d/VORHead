using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TDMDInitController : MonoBehaviour {

    public GameObject TDMDDC_Prefab;

    private GameObject TDMDDC_GO;

    // Use this for initialization
    void Start () {
        instant_TDMDDataController();

        SceneManager.LoadScene("TDMissileDefence");

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private void instant_TDMDDataController()
    {
        if (GameObject.Find("TDMD_DataController") == null)
        {
            TDMDDC_GO = Instantiate(TDMDDC_Prefab) as GameObject;
            TDMDDC_GO.name = "TDMD_DataController";
        }
    }
}
