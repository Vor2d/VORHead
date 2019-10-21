using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetOBJ : MonoBehaviour {

    private Renderer mesh_render;

    // Use this for initialization
    void Start()
    {
        this.mesh_render = GetComponent<Renderer>();
        this.mesh_render.enabled = true;

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void turn_on_mesh()
    {
        mesh_render.enabled = true;
    }

    public void turn_off_mesh()
    {
        mesh_render.enabled = false;
    }

    public void change_color(Color color)
    {
        mesh_render.material.color = color;
    }

}
