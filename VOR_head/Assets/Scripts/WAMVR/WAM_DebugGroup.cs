using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WAM_DebugGroup : MonoBehaviour
{
    [SerializeField] private bool Debuging;
    [SerializeField] private WAMRC RC;
    [SerializeField] private TextMesh TM1;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Debuging)
        {
            TM1.text = WAM_GameController.IS.Check_stop_instance.ToString();
        }
    }
}
