using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestLine : MonoBehaviour
{
    [SerializeField] private Transform Head_simulator_TRANS;
    [SerializeField] private MD_TargetRayCast MDTRC_script;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3[] positions = new Vector3[] { Head_simulator_TRANS.position,
                                            MDTRC_script.Canvas_hit_position};
        GetComponent<LineRenderer>().SetPositions(positions);
    }
}
