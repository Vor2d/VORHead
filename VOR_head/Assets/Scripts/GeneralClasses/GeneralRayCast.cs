using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralRayCast : MonoBehaviour {

    [SerializeField] protected GeneralGameController GGC_script;
    [SerializeField] protected bool UseForWorldCanvas = false;
    [SerializeField] protected float RayCastDistance = 100.0f;
    [SerializeField] protected LayerMask RayLayerMask;

    public Vector3 Canvas_hit_position { get; set; }
    public bool Canvas_hit_flag { get; private set; }
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

        check_canvas();
    }

    private void multi_raycast_hit()
    {
        Ray ray = new Ray(transform.position, transform.forward);

        Hits = Physics.RaycastAll(ray, RayCastDistance, RayLayerMask);
    }

    private void check_canvas()
    {
        if(UseForWorldCanvas)
        {
            Canvas_hit_flag = false;
            RaycastHit hit;
            Transform hit_TRANS;
            for (int i = 0; i < Hits.Length; i++)
            {
                hit = Hits[i];
                hit_TRANS = hit.transform;
                if (hit_TRANS != null)
                {
                    if(hit_TRANS.CompareTag(GeneralStrDefiner.WorldCanvasCollider_tag))
                    {
                        Canvas_hit_position = hit.point;
                        Canvas_hit_flag = true;
                        break;
                    }
                }
            }
        }
    }

    public bool check_object(string tag)
    {
        foreach (RaycastHit hit in Hits)
        {
            if (hit.transform.CompareTag(tag))
            {
                return true;
            }
        }
        return false;
    }

    public bool check_object(string tag,Transform TRANS)
    {
        foreach(RaycastHit hit in Hits)
        {
            if(hit.transform.CompareTag(tag) && GameObject.ReferenceEquals(hit.transform, TRANS))
            {
                return true;
            }
        }
        return false;
    }
}
