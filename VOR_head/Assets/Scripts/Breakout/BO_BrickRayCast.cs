using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BO_BrickRayCast : MonoBehaviour {

    [SerializeField] private GameObject Emission_Prefab;
    [SerializeField] private Transform Parent_Trans;
    [SerializeField] private float MaxDistance = 10.0f;
    [SerializeField] private float InitAlpha = 50.0f;
    [SerializeField] private LayerMask RayMask;

    private Transform shadow_TRANS;
    private Vector3 hit_position;
    private float hit_distace;
    private bool hited_flag;

    // Use this for initialization
    void Start()
    {
        this.hit_position = new Vector3();
        this.hit_distace = 0.0f;
        this.shadow_TRANS = null;
        this.hited_flag = false;

        raycast_hit();
        if(hited_flag)
        {
            instantiat_emission();
            set_color();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void raycast_hit()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;
        Physics.Raycast(ray, out hit, MaxDistance, RayMask);
        hit_position = hit.point;
        hit_distace = hit.distance;

        if(hit_position == new Vector3() || hit_position == null)
        {
            hited_flag = false;
        }
        else
        {
            hited_flag = true;
        }

        //Debug.Log("hit distance " + hit_distace);
    }

    private void instantiat_emission()
    {
        shadow_TRANS = Instantiate(Emission_Prefab, hit_position,
                                    new Quaternion(),
                                    Parent_Trans).transform;
    }

    private void set_color()
    {
        Color parent_color = Parent_Trans.GetComponent<Renderer>().material.color;
        float new_alpha = parent_color.a * ((MaxDistance - hit_distace) / MaxDistance);
        shadow_TRANS.GetComponent<Renderer>().material.SetColor("_Color",
                        new Color(parent_color.r, parent_color.g, parent_color.b, new_alpha));

        //emission_OBJ.GetComponent<Renderer>().material.SetColor("_EmissionColor",
        //                        new Color(parent_color.r, parent_color.g, parent_color.b));
    }

    public void update_shadow()
    {
        raycast_hit();
        Debug.Log("hit flag " + hited_flag);
        if(hited_flag)
        {
            shadow_TRANS.position = hit_position;
            set_color();
        }
        else if(shadow_TRANS != null)
        {
            Destroy(shadow_TRANS.gameObject);
        }
    }
}
