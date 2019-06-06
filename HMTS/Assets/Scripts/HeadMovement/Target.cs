using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour {

    private enum ChildName { TPlane,TCrossBar }
    //private enum direction { left, right };

    //private float init_dist;
    private TargetIndicator tar_ind_script;
    private TargetOBJ tar_obj_script;
    private bool tar_mesh_flag = false;
    private bool tar_obj_mesh_flag = true;

    // Use this for initialization
    void Start () {
        //this.init_dist = transform.position.z;
        this.tar_ind_script = transform.GetChild((int)ChildName.TPlane)
                                        .gameObject.GetComponent<TargetIndicator>();
        this.tar_obj_script = transform.GetChild((int)ChildName.TCrossBar)
                                        .gameObject.GetComponent<TargetOBJ>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    //public void changePosition(float ang_deg, int direc)
    //{
    //    //Debug.Log("changePosition");
    //    float ang_rand = ang_deg * Mathf.PI / 180.0f;

    //    float newx = Mathf.Cos(ang_rand) * this.init_dist;
    //    float newy = Mathf.Sin(ang_rand) * this.init_dist;

    //    if (direc == (int)(direction.left))
    //    {
    //        newy = -newy;
    //    }

    //    transform.position = new Vector3(newy, 0.0f, newx);

    //    transform.LookAt(Camera.main.transform);
    //}

    public void turn_on_tmesh()
    {
        if(!tar_mesh_flag)
        {
            tar_ind_script.turn_on_mesh();
            tar_mesh_flag = true;
        }
    }

    public void turn_off_tmesh()
    {
        if(tar_mesh_flag)
        {
            tar_ind_script.turn_off_mesh();
            tar_mesh_flag = false;
        }
    }

    public void turn_on_tobjmesh()
    {
        if (!tar_obj_mesh_flag)
        {
            tar_obj_script.turn_on_mesh();
            tar_obj_mesh_flag = true;
        }
    }

    public void turn_off_tobjmesh()
    {
        if (tar_obj_mesh_flag)
        {
            tar_obj_script.turn_off_mesh();
            tar_obj_mesh_flag = false;
        }
    }

    public void turn_on_all_tmesh()
    {
        turn_on_tmesh();
        turn_on_tobjmesh();
    }

    public void turn_off_all_tmesh()
    {
        turn_off_tmesh();
        turn_off_tobjmesh();
    }
}
