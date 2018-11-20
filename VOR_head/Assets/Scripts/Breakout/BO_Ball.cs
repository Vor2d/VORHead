using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BO_Ball : MonoBehaviour {

    [SerializeField] private float MaxPlaneSpeed = 100.0f;
    [SerializeField] private float MaxDepthSpeed = 100.0f;
    [SerializeField] private float MinPlaneSpeed = 0.0f;
    [SerializeField] private float MinDepthSpeed = 10.0f;
    [SerializeField] private float AdjustForce = 10.0f;
    [SerializeField] private float StartForce = 1000.0f;

    private BO_GameController BOGC_script;
    private Rigidbody ball_RB;
    private bool start_flag;

    // Use this for initialization
    void Start () {
        this.ball_RB = GetComponent<Rigidbody>();
        this.BOGC_script = 
                GameObject.Find("BO_GameController").GetComponent<BO_GameController>();
        this.start_flag = false;

	}
	
	// Update is called once per frame
	void FixedUpdate () {

        if(start_flag)
        {
            check_velocity();
        }

    }

    private void check_velocity()
    {
        Vector3 speed = ball_RB.velocity;

        //Over max plane speed;
        if(Mathf.Abs(speed.x) > MaxPlaneSpeed)
        {
            //Debug.Log("speed.x " + speed.x);
            float force = speed.x > 0 ? -AdjustForce : AdjustForce;
            ball_RB.AddForce(new Vector3(force, 0.0f, 0.0f));
        }
        if (Mathf.Abs(speed.y) > MaxPlaneSpeed)
        {
            //Debug.Log("speed.y " + speed.y);
            float force = speed.y > 0 ? -AdjustForce : AdjustForce;
            ball_RB.AddForce(new Vector3(0.0f, force, 0.0f));
        }

        //Over max depth speed;
        if (Mathf.Abs(speed.z) > MaxDepthSpeed)
        {
            //Debug.Log("speed.z " + speed.z);
            float force = speed.z > 0 ? -AdjustForce : AdjustForce;
            ball_RB.AddForce(new Vector3(0.0f, 0.0f, force));
        }

        //Lower min depth speed;
        if (Mathf.Abs(speed.z) < MinDepthSpeed)
        {
            //Debug.Log("speed.z small " + speed.z);
            float dep_force = speed.z > 0 ? AdjustForce : -AdjustForce;
            ball_RB.AddForce(new Vector3(0.0f, 0.0f, dep_force));
            if (Mathf.Abs(speed.x) >= Mathf.Abs(speed.y))
            {
                float force = speed.x > 0 ? -AdjustForce : AdjustForce;
                ball_RB.AddForce(new Vector3(force, 0.0f, 0.0f));
            }
            else
            {
                float force = speed.y > 0 ? -AdjustForce : AdjustForce;
                ball_RB.AddForce(new Vector3(0.0f, force, 0.0f));
            }
        }
    }

    public void start_ball()
    {
        ball_RB.AddForce(Vector3.forward * StartForce);
        start_flag = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("BO_DeSpawnWall"))
        {
            BOGC_script.restart_game();
            Destroy(gameObject);
        }
    }
}
