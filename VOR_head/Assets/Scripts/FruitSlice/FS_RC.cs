using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Reference Controller;
public class FS_RC : MonoBehaviour
{
    //Objects;
    public FS_GameController GC_script;
    public Transform ScoreText_TRANS;
    public Controller_Input CI_script;
    //Prefabs;
    public GameObject Fruit_Prefab;

    [HideInInspector]
    public FS_DataController DC_script;

    private void Awake()
    {
        GeneralMethods.check_ref<FS_DataController>(ref DC_script, FS_SD.DC_OBJ_name);
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
