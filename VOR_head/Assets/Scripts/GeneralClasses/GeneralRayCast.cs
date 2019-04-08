using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralRayCast : MonoBehaviour {

    [SerializeField] protected float RayCastDistance = 100.0f;
    [SerializeField] protected LayerMask RayLayerMask;

    public Vector3 Canvas_hit_position { get; set; }
    public Vector3 Hit_position { get; set; }
    public RaycastHit[] Hits { get; set; }

    // Use this for initialization
    protected virtual void Start()
    {
        this.Hit_position = new Vector3();
        this.Canvas_hit_position = new Vector3();
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        multi_raycast_hit();
    }

    private void multi_raycast_hit()
    {
        Ray ray = new Ray(transform.position, transform.forward);

        Hits = Physics.RaycastAll(ray, RayCastDistance, RayLayerMask);

    }
}
