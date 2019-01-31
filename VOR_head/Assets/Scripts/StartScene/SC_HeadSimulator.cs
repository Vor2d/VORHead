using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_HeadSimulator : MonoBehaviour
{
    [SerializeField] private Transform Camera_TRANS;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Camera_TRANS.position;
        transform.rotation = GeneralMethods.getVRrotation();
    }
}
