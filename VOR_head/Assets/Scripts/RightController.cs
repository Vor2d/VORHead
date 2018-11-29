using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RightController : MonoBehaviour {

    private const string UILayerMaskName = "WorldUI";

    [SerializeField] private LayerMask RCRayMask;
    [SerializeField] private Transform HitIndicator;

    [SerializeField] private float MaxDistance = 100.0f;

    public Vector3 hit_point { get; set; }

    private RaycastHit hit;
    private bool using_controller;

	// Use this for initialization
	void Start () {
        this.hit = new RaycastHit();

        turn_on_controller();
	}
	
	// Update is called once per frame
	void Update () {
        update_hand();
        if(using_controller)
        {
            update_indicator();
        }
    }

    private void FixedUpdate()
    {
        if(using_controller)
        {
            controller_raycast();
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
        Vector3[] positions = { transform.position, hit.point };
        GetComponent<LineRenderer>().SetPositions(positions);
        transform.parent.Find("HitIndicator").position = hit.point;
    }

    private void controller_raycast()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        Physics.Raycast(ray, out hit, MaxDistance, RCRayMask);
        if (hit.transform.gameObject.layer == LayerMask.NameToLayer(UILayerMaskName))
        {
            hit_point = hit.point;
        }
    }

    public void turn_on_controller()
    {
        GetComponent<LineRenderer>().enabled = true;
        HitIndicator.GetComponent<MeshRenderer>().enabled = true;
        using_controller = true;
    }

    public void turn_off_controller()
    {
        GetComponent<LineRenderer>().enabled = false;
        HitIndicator.GetComponent<MeshRenderer>().enabled = false;
        using_controller = false;
    }
}
