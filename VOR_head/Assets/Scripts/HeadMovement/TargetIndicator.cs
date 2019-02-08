using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetIndicator : MonoBehaviour {

    private Renderer mesh_render;

    // Use this for initialization
    void Start()
    {
        this.mesh_render = GetComponent<Renderer>();
        this.mesh_render.enabled = false;

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void turn_on_mesh()
    {
        //Debug.Log("circle turned on");
        mesh_render.enabled = true;
    }

    public void turn_off_mesh()
    {
        //Debug.Log("circle turned off!!!");
        mesh_render.enabled = false;
    }

}
