using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Reference Controller;
public class BP_RC : MonoBehaviour
{
    public BP_GameController BPGC_script;
    public BP_DataController BPDC_script;
    public GameObject BubblePrefab;
    public Transform ScoreText_TRANS;
    public Transform[] Stations_TRANS;
    public Transform Charator_TRANS;

    private void Awake()
    {
        this.BPDC_script = GameObject.Find(BP_StrDefiner.DataController_name).
                                                GetComponent<BP_DataController>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
