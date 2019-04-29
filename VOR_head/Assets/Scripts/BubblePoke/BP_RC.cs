using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//Reference Controller;
public class BP_RC : MonoBehaviour
{
    public enum AcuityDir { Up, Right, Down, Left };

    public BP_GameController GC_script;
    public GameObject Bubble_Prefab;
    public GameObject[] Charator_Prefebs;
    public GameObject[] Path_Prefebs;
    public Transform[] PathPoseIs_TRANSs;
    public Transform ScoreText_TRANS;
    public Controller_Input CI_script;
    public GameObject TrailController_Prefab;
    public GameObject[] Acuitys_Prefabs;

    public BP_DataController DC_script { get; set; }
    [Obsolete("Infomation will be included in TrailController")]
    public List<Transform> Paths_TRANSs { get; set; }
    [Obsolete("Infomation will be included in TrailController")]
    public List<Transform> Charators_TRANSs { get; set; }
    public List<Transform> Bubble_TRANSs { get; set; }
    public List<Transform> TrailControllers_TRANSs { get; set; }

    private void Awake()
    {
        this.DC_script = GameObject.Find(BP_StrDefiner.DataController_name).
                                                        GetComponent<BP_DataController>();

        this.Paths_TRANSs = new List<Transform>();
        this.Charators_TRANSs = new List<Transform>();
        this.Bubble_TRANSs = new List<Transform>();
        this.TrailControllers_TRANSs = new List<Transform>();
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
