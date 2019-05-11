using UnityEngine;

/// <summary>
/// Reference Controller Object;
/// </summary>
public class FS_RC : MonoBehaviour
{
    //Objects;
    public FS_GameController GC_script;
    public Transform ScoreText_TRANS;
    public Controller_Input CI_script;
    public FS_RayCast RC_script;
    public Transform Fruit_TRANS;
    //Prefabs;
    public GameObject Fruit_Prefab;

    [HideInInspector]
    public FS_DataController DC_script;

    private void Awake()
    {
        GeneralMethods.check_ref<FS_DataController>(ref DC_script, FS_SD.DC_OBJ_name);
    }

}
