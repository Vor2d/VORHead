using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndiText1 : MonoBehaviour
{
    [SerializeField] private Transform Target;

    [SerializeField] private Vector3 Offset;
    [SerializeField] private bool UsingLookAt;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Target.position + Offset;
        if (UsingLookAt) 
        {
            transform.LookAt(Camera.main.transform);
            transform.Rotate(Vector3.up, 180.0f);
        }
    }
}
