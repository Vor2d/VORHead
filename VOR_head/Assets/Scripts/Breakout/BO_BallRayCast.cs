using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BO_BallRayCast : MonoBehaviour {

    [SerializeField] private Transform Ball_TRANS;

    //[SerializeField] private float MaxDistance = 100.0f;

    private RaycastHit hit;
    private Vector3 last_ball_vel;
    //private bool vel_changed_flag;

    // Use this for initialization
    void Start () {
        this.hit = new RaycastHit();
        this.last_ball_vel = new Vector3();
        //this.vel_changed_flag = false;

    }

    private void Update()
    {

    }

    // Update is called once per frame
    void FixedUpdate () {

        if (last_ball_vel != Ball_TRANS.GetComponent<Rigidbody>().velocity)
        {
            ray_cast();
            draw_line();
        }
        last_ball_vel = Ball_TRANS.GetComponent<Rigidbody>().velocity;

    }

    private void ray_cast()
    {
        //Debug.Log("transform.position " + transform.position);
        //Debug.Log("direction " + (transform.position + Ball_TRANS.GetComponent<Rigidbody>().velocity.normalized));
        Ray ray = new Ray(transform.position,
                            Ball_TRANS.GetComponent<Rigidbody>().velocity.normalized);
        //Debug.Log("ray " + ray.origin);
        //Debug.Log("direction " + ray.direction);
        //Debug.DrawLine(ray.origin, ray.origin+ray.direction,Color.blue,1.0f);
        Physics.Raycast(ray, out hit);
        //Debug.Log("hit object " + hit.transform.name);
        //Debug.Log("Ball_TRANS.GetComponent<Rigidbody>().velocity " + Ball_TRANS.GetComponent<Rigidbody>().velocity.normalized);
    }

    private void draw_line()
    {
        transform.parent.Find("BallLineRenderer").GetComponent<LineRenderer>().
                    SetPositions(new Vector3[] { transform.position, hit.point });
    }

}
