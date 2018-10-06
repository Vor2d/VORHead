using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour {


    public float Speed = 0.5f;

    public bool start_flag;
    private Vector3 Target_pos;


    // Use this for initialization
    void Start () {
        this.Target_pos = new Vector3();
        this.start_flag = false;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        Debug.Log("start_flag " + start_flag);
        if (start_flag)
        {
            Debug.Log("Moving Target_pos "+ Target_pos);
            transform.Translate(Target_pos * Time.deltaTime * Speed, Space.World);
        }
    }

    public void set_target(Transform tar_transform)
    {
        Debug.Log("tar_transform.position " + tar_transform.position);
        Debug.Log("transform.position " + transform.position);

        Target_pos = tar_transform.position - transform.position;
        Debug.Log("Target_pos " + Target_pos);
    }

    public void start_move()
    {
        face(Target_pos);
        Debug.Log("Target_pos2 " + Target_pos);
        start_flag = true;
        Debug.Log("start_flag2 " + start_flag);

    }

    public void face(Vector3 pos)
    {
        transform.up = -pos;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("11111");
        if(other.transform.gameObject.tag == "City")
        {
            Destroy(gameObject);
        }
    }
}
