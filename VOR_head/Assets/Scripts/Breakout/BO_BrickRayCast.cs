using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BO_BrickRayCast : MonoBehaviour {

    [SerializeField] private GameObject Emission_Prefab;
    [SerializeField] private Transform Parent_Trans;
    [SerializeField] private float MaxDistance = 10.0f;
    [SerializeField] private float InitAlpha = 50.0f;

    private Vector3 hit_position;
    private float hit_distace;

    // Use this for initialization
    void Start()
    {
        this.hit_position = new Vector3();
        this.hit_distace = 0.0f;

        raycast_hit();
        instantiat_emission();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void raycast_hit()
    {
        Ray ray = new Ray(transform.position, transform.forward);

        RaycastHit hit;
        Physics.Raycast(ray, out hit, MaxDistance);
        hit_position = hit.point;
        hit_distace = hit.distance;

        //Debug.Log("hit distance " + hit_distace);
    }

    private void instantiat_emission()
    {
        GameObject emission_OBJ = Instantiate(Emission_Prefab, hit_position,
                                                new Quaternion(),
                                                Parent_Trans);
        Color parent_color = Parent_Trans.GetComponent<Renderer>().material.color;
        float new_alpha = InitAlpha * (hit_distace / MaxDistance)/255.0f;
        Debug.Log("new_alpha " + new_alpha);
        emission_OBJ.GetComponent<Renderer>().material.SetColor("_Color",
                        new Color(parent_color.r, parent_color.g, parent_color.b, new_alpha));

        //emission_OBJ.GetComponent<Renderer>().material.SetColor("_EmissionColor",
        //                        new Color(parent_color.r, parent_color.g, parent_color.b));

    }
}
