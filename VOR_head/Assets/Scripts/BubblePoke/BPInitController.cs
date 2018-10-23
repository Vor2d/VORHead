using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BPInitController : MonoBehaviour {

    public GameObject BPDCPrefab;

	// Use this for initialization
	void Start () {
        GameObject BPDC_obj = Instantiate(BPDCPrefab) as GameObject;
        BPDC_obj.name = "BP_DataController";

        SceneManager.LoadScene("BubblePoke");
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
