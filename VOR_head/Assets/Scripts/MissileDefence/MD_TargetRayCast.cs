using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MD_TargetRayCast : MonoBehaviour {

    public float RayCastDistance = 100.0f;

    public Vector3 Hit_position { get; set; }

    private RaycastHit[] hits;

	// Use this for initialization
	void Start () {
        this.Hit_position = new Vector3();
	}
	
	// Update is called once per frame
	void Update () {

        multi_raycast_hit();

        check_multi_hits();

    }

    private void multi_raycast_hit()
    {
        Ray ray = new Ray(transform.position, transform.forward);

        hits = Physics.RaycastAll(ray, RayCastDistance);
    }

    private void check_multi_hits()
    {
        for (int i = 0; i < hits.Length; i++)
        {
            RaycastHit hit = hits[i];
            Transform objectHit = hit.transform;
            {
                if (objectHit != null)
                {
                    if (objectHit.tag == "MD_TargetBorder")
                    {
                        Hit_position = hit.point;
                    }
                }
            }
        }
    }
}
