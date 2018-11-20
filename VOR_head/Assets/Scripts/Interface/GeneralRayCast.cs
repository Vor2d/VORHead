using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralRayCast : MonoBehaviour {

    [SerializeField] private float RayCastDistance = 100.0f;
    [SerializeField] private LayerMask layerMask;

    public Vector3 Hit_position { get; set; }
    public RaycastHit[] Hits { get; set; }

    // Use this for initialization
    void Start()
    {
        this.Hit_position = new Vector3();
    }

    // Update is called once per frame
    void Update()
    {

        multi_raycast_hit();

    }

    private void multi_raycast_hit()
    {
        Ray ray = new Ray(transform.position, transform.forward);

        Hits = Physics.RaycastAll(ray, RayCastDistance, layerMask);

    }
}
