using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayCast : MonoBehaviour {

    public bool hit_flag { get; set; }
    public bool hit_center_flag { get; set; }
    public bool hit_centest_flag { get; set; }
    public bool hit_border_flag { get; set; }
    public bool hit_hideDetector_flag { get; set; }

    public float ray_cast_distance = 100.0f;

    //private RaycastHit hit;
    private Transform objectHit;
    private RaycastHit[] hits;


    // Use this for initialization
    void Start () {

	}
	
	// Update is called once per frame
	void Update () {

    }

    private void FixedUpdate()
    {
        multi_raycast_hit();
        check_multi_hits();
    }

    private void multi_raycast_hit()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        
        hits = Physics.RaycastAll(ray, ray_cast_distance);
    }

    private void check_multi_hits()
    {
        hit_flag = false;
        hit_center_flag = false;
        hit_centest_flag = false;
        hit_border_flag = false;
        hit_hideDetector_flag = false;

        //Debug.Log("hits.Length " + hits.Length);

        for (int i = 0; i < hits.Length; i++)
        {
            RaycastHit hit = hits[i];
            objectHit = hit.transform;

            if (objectHit != null)
            {
                if (objectHit.tag == "TargetSensor")
                {
                    hit_flag = true;
                }
                //if (objectHit.tag == "CenterSensor")
                //{
                //    hit_center_flag = true;
                //    return;
                //}
                //if (objectHit.tag == "CenterTest")
                //{
                //    hit_centest_flag = true;
                //    return;
                //}
                if(objectHit.tag == "TurnBorder")
                {
                    hit_border_flag = true;
                }
                if(objectHit.tag == "HideDetector")
                {
                    hit_hideDetector_flag = true;
                }

            }
        }

    }

    private void raycast_hit()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            objectHit = hit.transform;
        }
        else
        {
            objectHit = null;
        }

        //Debug.Log(objectHit);

        hit_flag = false;
        hit_center_flag = false;
        hit_centest_flag = false;

        if (objectHit != null)
        {
            if (objectHit.tag == "TargetSensor")
            {
                //Debug.Log("TargetSensor");
                hit_flag = true;
                return;
            }
            if (objectHit.tag == "CenterSensor")
            {
                hit_center_flag = true;
                return;
            }
            if (objectHit.tag == "CenterTest")
            {
                hit_centest_flag = true;
                return;
            }
        }
    }

    //public bool get_hit_flag()
    //{
    //    return hit_flag;
    //}

    //public bool get_hit_center_flag()
    //{
    //    return hit_center_flag;
    //}
    //public bool get_hit_centest_flag()
    //{
    //    return hit_centest_flag;
    //}
}
