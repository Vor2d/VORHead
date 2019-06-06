using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadIndicator : MonoBehaviour
{
    [SerializeField] private RayCast RC_script;
    private DataController DC_script;

    // Start is called before the first frame update
    void Start()
    {
        this.DC_script = GameObject.Find("DataController").GetComponent<DataController>();
    }

    // Update is called once per frame
    void Update()
    {
        if(DC_script.using_VR)
        {
            transform.position = RC_script.Hit_position;
        }
    }
}
