using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RightController : GeneralRayCast {

    [SerializeField] private Transform HitIndicator_TRANS;

    private bool using_controller;
    private bool canvas_hitted;

	// Use this for initialization
	protected override void Start () {
        base.Start();

        this.using_controller = false;
        this.canvas_hitted = false;

        turn_on_controller();
	}
	
	// Update is called once per frame
	void Update () {
        //update_hand();
        if(using_controller)
        {
            update_indicator();
        }
    }

    protected override void FixedUpdate()
    {
        if(using_controller)
        {
            base.FixedUpdate();

            multiple_ray_check();
        }
    }

    private void update_hand()
    {
        transform.localPosition =
                        OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch);
        transform.localRotation =
                        OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTouch);
    }

    private void update_indicator()
    {
        Vector3[] positions;
        if (canvas_hitted)
        {
            Debug.Log("canvas_hitted");
            positions = new Vector3[] { transform.position, Canvas_hit_position };
        }
        else
        {
            positions = new Vector3[] { transform.position, Hit_position };
        }
        GetComponent<LineRenderer>().SetPositions(positions);
        HitIndicator_TRANS.position = positions[1];
    }

    //private void controller_raycast()
    //{
    //    Ray ray = new Ray(transform.position, transform.forward);
    //    Physics.Raycast(ray, out hit, MaxDistance, RCRayMask);
    //    if (hit.transform.gameObject.layer == LayerMask.NameToLayer(UILayerMaskName))
    //    {
    //        hit_point = hit.point;
    //    }
    //}

    private void multiple_ray_check()
    {
        canvas_hitted = false;
        if(Hits != null)
        {
            foreach (RaycastHit hit in Hits)
            {
                if (hit.transform.CompareTag(GeneralStrDefiner.WorldCanvasCollider_tag))
                {
                    Canvas_hit_position = hit.point;
                    canvas_hitted = true;
                }
                else
                {
                    Hit_position = hit.point;
                }
            }
        }


    }

    public void turn_on_controller()
    {
        GetComponent<LineRenderer>().enabled = true;
        HitIndicator_TRANS.GetComponent<MeshRenderer>().enabled = true;
        using_controller = true;
    }

    public void turn_off_controller()
    {
        GetComponent<LineRenderer>().enabled = false;
        HitIndicator_TRANS.GetComponent<MeshRenderer>().enabled = false;
        using_controller = false;
    }
}
