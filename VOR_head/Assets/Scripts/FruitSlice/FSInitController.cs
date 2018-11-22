using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FSInitController : MonoBehaviour {

    public GameObject FSDCPrefab;

	// Use this for initialization
	void Start () {
        GameObject BPDC_obj = Instantiate(FSDCPrefab) as GameObject;
        BPDC_obj.name = "FS_DataController";

        SceneManager.LoadScene("FruitSlice");
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
