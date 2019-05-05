using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FS_DataController : ParentDataController {

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        init_DC();
    }

    // Use this for initialization
    void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
