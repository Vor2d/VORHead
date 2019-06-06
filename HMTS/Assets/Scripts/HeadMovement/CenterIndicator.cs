using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CenterIndicator : MonoBehaviour {

    private Renderer mesh_render;
    private Color mesh_init_color;
    private bool normal_flag;

    // Use this for initialization
    void Start()
    {
        this.mesh_render = GetComponent<Renderer>();
        this.mesh_init_color = mesh_render.material.color;
        this.normal_flag = true;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void chan_col_toGreen()
    {
        if(normal_flag)
        {
            mesh_render.material.color = Color.green;
            normal_flag = false;
        }
    }

    public void chan_col_toNormal()
    {
        if (!normal_flag)
        {
            mesh_render.material.color = mesh_init_color;
            normal_flag = true;
        }
    }


}
