using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Reference Controller;
public class BP_RC : MonoBehaviour
{
    public BP_GameController GC_script;
    public BP_DataController DC_script;
    public GameObject Bubble_Prefab;
    public GameObject Charator_Prefeb;
    public GameObject Path_Prefeb;
    public Transform ScoreText_TRANS;
    public Transform[] Paths_TRANS;
    public Transform[] Charators_TRANS;

    private void Awake()
    {
        this.DC_script = GameObject.Find(BP_StrDefiner.DataController_name).
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
