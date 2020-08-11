using UnityEngine;
using System.Collections.Generic;
using MeshSystem;
using System;

/// <summary>
/// Reference Controller Object;
/// </summary>
public class FS_RC : MonoBehaviour
{
    public readonly Color Color_white = Color.white;

    //Objects;
    //public FS_GameController GC_script;
    public Transform ScoreText_TRANS;
    public Transform ScoreStar_TRANS;
    public Controller_Input CI_script;
    public FS_RayCast RC_script;
    public Transform Fruit_TRANS;
    public Material FruitCommon_MTRL;
    public FS_Player player;
    //Prefabs;
    [Obsolete("Fruit static")]
    public GameObject Fruit_Prefab;
    public GameObject Fruit_Indi_Prefab;
    public GameObject RB_sam_Prefab;
    public GameObject TrialResultFrame;
    public GameObject Star_Prefab;
    public GameObject Star_unfilled_Prefab;
    //Animators;
    public Animator GC_Ani;
    public Animator UI_Ani;
    public Animator Fruit_Ani;
    //Global vars;
    public int PPU;

    [HideInInspector]
    //public FS_DataController DC_script;
    public Dictionary<MeshData,Transform> MeshDataPool { get; set; }
    public GameObject Selected_GO { get 
        { return FS_DataController.IS.TrialGroup_prefabs[FS_GameController.IS.Trial_index]; } }
    public List<FS_TrialGroup> Level_infos { get; set; }

    public static FS_RC IS { get; set; }

    private void Awake()
    {
        IS = this;

        this.MeshDataPool = new Dictionary<MeshData, Transform>();
        this.Level_infos = new List<FS_TrialGroup>();
        init_level_infos();
        this.player = FS_DataController.IS.player;
    }

    private void Update()
    {

    }

    private void init_level_infos()
    {
        foreach (GameObject level_GO in FS_DataController.IS.TrialGroup_prefabs)
        {
            Level_infos.Add(level_GO.GetComponent<FS_TrialGroup>());
        }
    }

}
