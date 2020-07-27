using UnityEngine;
using System.Collections.Generic;
using MeshSystem;

/// <summary>
/// Reference Controller Object;
/// </summary>
public class FS_RC : MonoBehaviour
{
    //Objects;
    //public FS_GameController GC_script;
    public Transform ScoreText_TRANS;
    public Controller_Input CI_script;
    public FS_RayCast RC_script;
    public Transform Fruit_TRANS;
    public Material FruitCommon_MTRL;
    //Prefabs;
    public GameObject Fruit_Prefab;
    public GameObject Fruit_Indi_Prefab;
    public GameObject RB_sam_Prefab;
    //Animators;
    public Animator GC_Ani;
    public Animator UI_Ani;
    public Animator Fruit_Ani;

    [HideInInspector]
    //public FS_DataController DC_script;
    public Dictionary<MeshData,Transform> MeshDataPool { get; set; }

    public static FS_RC IS { get; set; }

    private void Awake()
    {
        IS = this;

        this.MeshDataPool = new Dictionary<MeshData, Transform>();
        //GeneralMethods.check_ref<FS_DataController>(ref DC_script, FS_SD.DC_OBJ_name);
    }

}
