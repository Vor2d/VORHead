using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TDMD_TargetRayCast : MonoBehaviour {

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
        foreach (RaycastHit hit in hits)
        {
            GameObject hit_OBJ = hit.transform.gameObject;
            if (hit_OBJ.tag == "TDMissile")
            {
                hit_OBJ.GetComponent<TDMissile>().Aimed_flag = true;
            }

        }
    }
}
