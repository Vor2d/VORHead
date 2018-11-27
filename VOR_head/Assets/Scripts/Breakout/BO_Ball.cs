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
    [SerializeField] private float BallDiameter = 1.0f;

    [SerializeField] ParticleSystem Left_PS;
    [SerializeField] ParticleSystem Right_PS;
    [SerializeField] ParticleSystem Up_PS;
    [SerializeField] ParticleSystem Down_PS;

    private BO_GameController BOGC_script;
    private Rigidbody ball_RB;
    private bool start_flag;
    private float boundary_timer;
    private bool boundary_timer_flag;

    private bool last_left_boundary_flag;
    private bool left_boundary_flag;
    private bool last_right_boundary_flag;
    private bool right_boundary_flag;
    private bool last_up_boundary_flag;
    private bool up_boundary_flag;
    private bool last_down_boundary_flag;
    private bool down_boundary_flag;

    //Debug
    //public Vector3 collision_pos;

    // Use this for initialization
    void Start () {
        this.ball_RB = GetComponent<Rigidbody>();
        this.BOGC_script = 
                GameObject.Find("BO_GameController").GetComponent<BO_GameController>();
        this.start_flag = false;
        this.boundary_timer = BoundaryTime;
        this.boundary_timer_flag = false;
        this.last_left_boundary_flag = false;
        this.left_boundary_flag = false;
        this.last_right_boundary_flag = false;
        this.right_boundary_flag = false;
        this.last_up_boundary_flag = false;
        this.up_boundary_flag = false;
        this.last_down_boundary_flag = false;
        this.down_boundary_flag = false;
    }

    private void Update()
    {
        check_boundary();
        if (boundary_timer_flag)
        {
            boundary_timer -= Time.deltaTime;
        }
        else
        {
            boundary_timer = BoundaryTime;
        }

        if(!last_left_boundary_flag && left_boundary_flag)  //left
        {
            turn_on_left_PS();
            //BOGC_script.DebugText1.GetComponent<TextMesh>().text = "left";
        }
        else if(last_left_boundary_flag && !left_boundary_flag)
        {
            turn_off_left_PS();
        }
        if (!last_right_boundary_flag && right_boundary_flag) //right
        {
            turn_on_right_PS();
            //BOGC_script.DebugText1.GetComponent<TextMesh>().text = "right";

        }
        else if (last_right_boundary_flag && !right_boundary_flag)
        {
            turn_off_right_PS();
        }
        if (!last_up_boundary_flag && up_boundary_flag) //up
        {
            turn_on_up_PS();
            //BOGC_script.DebugText1.GetComponent<TextMesh>().text = "up";

        }
        else if (last_up_boundary_flag && !up_boundary_flag)
        {
            turn_off_up_PS();
        }
        if (!last_down_boundary_flag && down_boundary_flag) //down
        {
            turn_on_down_PS();
            //BOGC_script.DebugText1.GetComponent<TextMesh>().text = "down";

        }
        else if (last_down_boundary_flag && !down_boundary_flag)
        {
            turn_off_down_PS();
        }
        last_left_boundary_flag = left_boundary_flag;
        last_right_boundary_flag = right_boundary_flag;
        last_up_boundary_flag = up_boundary_flag;
        last_down_boundary_flag = down_boundary_flag;
    }

    // Update is called once per frame
    void FixedUpdate () {

        if(start_flag)
        {
            check_velocity();
            adjust_boundary();
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

    private void check_boundary()
    {
        if(HorizontalBoundary - Mathf.Abs(transform.position.x)
                    > HorizontalBoundary * BoundaryPercentage &&
            VerticalBoundary - Mathf.Abs(transform.position.y)
                    > VerticalBoundary * BoundaryPercentage)
        {
            boundary_timer_flag = false;
            left_boundary_flag = false;
            right_boundary_flag = false;
            up_boundary_flag = false;
            down_boundary_flag = false;
        }
        else
        {
            if (HorizontalBoundary - Mathf.Abs(transform.position.x)
                    < HorizontalBoundary * BoundaryPercentage)
            {
                if (transform.position.x < 0.0f)
                {
                    left_boundary_flag = true;
                }
                else
                {
                    right_boundary_flag = true;
                }
                boundary_timer_flag = true;
            }
            else
            {
                left_boundary_flag = false;
                right_boundary_flag = false;
            }
            if (VerticalBoundary - Mathf.Abs(transform.position.y)
                        < VerticalBoundary * BoundaryPercentage)
            {
                if (transform.position.y > 0.0f)
                {
                    up_boundary_flag = true;
                }
                else
                {
                    down_boundary_flag = true;
                }
                boundary_timer_flag = true;
            }
            else
            {
                up_boundary_flag = false;
                down_boundary_flag = false;
            }

        }

    }

    public void adjust_boundary()
    {
        if (boundary_timer < 0.0f)
        {
            //
            if (HorizontalBoundary - Mathf.Abs(transform.position.x)
                    < HorizontalBoundary * BoundaryPercentage)
            {
                float force = transform.position.x < 0 ? AdjustForce : -AdjustForce;
                ball_RB.AddForce(new Vector3(force, 0.0f, 0.0f));
            }
            else if (VerticalBoundary - Mathf.Abs(transform.position.y)
                    < VerticalBoundary * BoundaryPercentage)
            {
                float force = transform.position.y < 0 ? AdjustForce : -AdjustForce;
                ball_RB.AddForce(new Vector3(0.0f, force, 0.0f));
            }

        }
    }

    private void turn_on_left_PS()
    {
        Left_PS.Play();
    }

    private void turn_off_left_PS()
    {
        Left_PS.Stop();
    }

    private void turn_on_right_PS()
    {
        Right_PS.Play();
    }

    private void turn_off_right_PS()
    {
        Right_PS.Stop();
    }

    private void turn_on_up_PS()
    {
        Up_PS.Play();
    }

    private void turn_off_up_PS()
    {
        Up_PS.Stop();
    }

    private void turn_on_down_PS()
    {
        Down_PS.Play();
    }

    private void turn_off_down_PS()
    {
        Down_PS.Stop();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.CompareTag("BO_Brick"))
        {
            collision.transform.GetComponent<BO_Brick>().hited();
            BOGC_script.brick_destroied();
        }

        //Debug.Log("collision.contacts " + collision.contacts.Length);

        //foreach (ContactPoint contact in collision.contacts)
        //{
            
        //    // Visualize the contact point
        //    Debug.DrawRay(contact.point, contact.normal, Color.white);
        //}
    }
}
