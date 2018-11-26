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
    [SerializeField] private float HorizontalBoundary = 9.0f;
    [SerializeField] private float VerticalBoundary = 4.0f;
    [SerializeField] private float BoundaryPercentage = 0.1f;
    [SerializeField] private float BoundaryTime = 1.0f;

    private BO_GameController BOGC_script;
    private Rigidbody ball_RB;
    private bool start_flag;
    private float boundary_timer;
    private bool boundary_timer_falg;

    //Debug
    //public Vector3 collision_pos;

    // Use this for initialization
    void Start () {
        this.ball_RB = GetComponent<Rigidbody>();
        this.BOGC_script = 
                GameObject.Find("BO_GameController").GetComponent<BO_GameController>();
        this.start_flag = false;
        this.boundary_timer = BoundaryTime;
        this.boundary_timer_falg = false;
	}

    private void Update()
    {
        check_boundary();
        if (boundary_timer_falg)
        {
            boundary_timer -= Time.deltaTime;
        }
        else
        {
            boundary_timer = BoundaryTime;
        }
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

        if(boundary_timer < 0.0f)
        {
            if(HorizontalBoundary - Mathf.Abs(transform.position.x)
                    < HorizontalBoundary * BoundaryPercentage)
            {
                float force = transform.position.x < 0 ? AdjustForce : -AdjustForce;
                ball_RB.AddForce(new Vector3(force, 0.0f, 0.0f));
            }
            else if(VerticalBoundary - Mathf.Abs(transform.position.y)
                    < VerticalBoundary * BoundaryPercentage)
            {
                float force = transform.position.y < 0 ? AdjustForce : -AdjustForce;
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

    private void check_boundary()
    {
        if(HorizontalBoundary - Mathf.Abs(transform.position.x)
                    > HorizontalBoundary * BoundaryPercentage &&
           VerticalBoundary - Mathf.Abs(transform.position.y)
                    > VerticalBoundary * BoundaryPercentage)
        {
            boundary_timer_falg = false;
        }
        else
        {
            boundary_timer_falg = true;
        }

    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.CompareTag("BO_Brick"))
        {
            collision.transform.GetComponent<BO_Brick>().hited();
        }

        Debug.Log("collision.contacts " + collision.contacts.Length);

        foreach (ContactPoint contact in collision.contacts)
        {
            
            // Visualize the contact point
            Debug.DrawRay(contact.point, contact.normal, Color.white);
        }
    }
}
