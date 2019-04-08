using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BP_DebugController : MonoBehaviour
{
    [SerializeField] private BP_RC BPRC;
    [SerializeField] private Transform DebugText1_TRANS;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        DebugText1_TRANS.GetComponent<TextMesh>().text = BPRC.Bubble_TRANSs.Count.ToString();
    }
}
