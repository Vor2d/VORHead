using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour {


    public float Speed = 0.5f;

    private bool start_flag;
    private Vector3 target_pos;

    //Need to init objects here since it is a prefab;
    private void Awake()
    {
        this.target_pos = new Vector3();
        this.start_flag = false;
    }

    // Use this for initialization
    void Start () {

	}
	
	// Update is called once per frame
	void FixedUpdate () {
        if (start_flag)
        {
            transform.Translate(target_pos * Time.deltaTime * Speed, Space.World);
        }
    }

    public void set_target(Transform tar_transform)
    {
        target_pos = tar_transform.position - transform.position;
    }

    public void start_move()
    {
        face(target_pos);
        start_flag = true;
    }

    public void face(Vector3 pos)
    {
        transform.up = -pos;
    }

    private void OnTriggerEnter(Collider other)
    {
        GameObject other_GO = other.transform.gameObject;
        if (other_GO.tag == "City")
        {
            other_GO.GetComponent<City>().get_hit();
            Destroy(gameObject);
        }
        else if(other_GO.tag == "Explode")
        {
            Destroy(gameObject);
        }
    }
}
