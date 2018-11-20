using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BO_DataController : ParentDataController {

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    // Use this for initialization
    void Start()
    {
        init_DC();
    }

    // Update is called once per frame
    void Update()
    {

    }


}
