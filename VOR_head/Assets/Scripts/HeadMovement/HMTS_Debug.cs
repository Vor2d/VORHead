using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HMTS_Debug : MonoBehaviour
{
    public GameController GC_script;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("Target_raycast_flag " + GC_script.Target_raycast_flag);
    }
}
