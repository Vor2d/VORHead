using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BO_RC : MonoBehaviour
{
    //Scripts;
    public BO_GameController GC_script;

    [HideInInspector]
    public BO_DataController DC_script;

    private void Awake()
    {
        GeneralMethods.check_ref<BO_DataController>(ref DC_script, BO_SD.DC_name);
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
