using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TDMD_DataController : MonoBehaviour {

    public bool Using_VR_flag { get; set; }

    private void Awake()
    {
        DontDestroyOnLoad(this);

    }

    // Use this for initialization
    void Start () {
        this.Using_VR_flag = true;

    }

    // Update is called once per frame
    void Update () {
    }
}
