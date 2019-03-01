using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadIndicator : MonoBehaviour
{
    [SerializeField] private RayCast RC_script;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = RC_script.Hit_position;
    }
}
