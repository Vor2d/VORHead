using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultTarget : MonoBehaviour {

    private enum direction { left, right };

    private float init_dist;
    private bool mesh_flag;
    private Renderer mesh_render;

    // Use this for initialization
    void Start () {
        this.init_dist = transform.position.z;
        this.mesh_render = GetComponent<Renderer>();

        this.mesh_flag = true;
        turn_off_mesh();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void changePosition(float ang_deg, int direc)
    {
        //Debug.Log("changePosition");
        float ang_rand = ang_deg * Mathf.PI / 180.0f;

        float newx = Mathf.Cos(ang_rand) * this.init_dist;
        float newy = Mathf.Sin(ang_rand) * this.init_dist;

        if (direc == (int)(direction.left))
        {
            newy = -newy;
        }

        transform.position = new Vector3(newy, 0.0f, newx);

        transform.LookAt(Camera.main.transform,Vector3.up);
        transform.Rotate(new Vector3(90.0f, 0.0f, 0.0f));
    }

    public void turn_on_mesh()
    {
        if(!mesh_flag)
        {
            mesh_render.enabled = true;
            mesh_flag = true;
        }
        
    }

    public void turn_off_mesh()
    {
        if(mesh_flag)
        {
            mesh_render.enabled = false;
            mesh_flag = false;
        }
    }
}
