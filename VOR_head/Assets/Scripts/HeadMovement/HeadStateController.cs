using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.XR;

public class HeadStateController : MonoBehaviour
{
    public int lookbackWindow = 10;
    public float speedThreshold = 100;
    public float speedDifferenceMax = 40;
    public float speedDifferenceMin = 25;
    public float TurnBorderDegree = 20.0f;

    public int speedEvaluationRecord { get; set; }
    public bool Check_speed_flag { get; set; }
    public string speedEvaluationMessage { get; set; }

    private Quaternion head_rotation;
    private Vector3 head_velocity;
    private Queue<Vector3> headVelocityHistory;
    private float headSpeedy;
    private float lookbackSpeedy;


    // Use this for initialization
    void Start()
    {
        this.head_rotation = new Quaternion();
        this.head_velocity = new Vector3();
        this.headVelocityHistory = new Queue<Vector3>();
        this.headSpeedy = 0.0f;
        this.lookbackSpeedy = 0.0f;
        this.speedEvaluationRecord = 0;
        this.Check_speed_flag = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        head_rotation = InputTracking.GetLocalRotation(XRNode.Head);
        if (Check_speed_flag)
        {
            head_velocity = OVRPlugin.GetNodeAngularVelocity(OVRPlugin.Node.Head,
                                    OVRPlugin.Step.Render).FromFlippedZVector3f() * Mathf.Rad2Deg;
        }
    }

    public bool check_speed()
    {
        readFromDataSource();
        return validateSpeed();
    }

    public bool check_from_center(float center_rotatey)
    {
        float head_rotate_angley = head_rotation.eulerAngles.y;
        head_rotate_angley = (head_rotate_angley > 180) ?
                                (360 - head_rotate_angley) : head_rotate_angley;
        //Debug.Log("head_rotate_angley" + head_rotate_angley);
        if (Mathf.Abs(Mathf.Abs(center_rotatey) - Mathf.Abs(head_rotate_angley))
                                                                            > TurnBorderDegree)
        {
            //Debug.Log("here?");
            speedEvaluationMessage = "Too Slow";
            return true;
        }
        else
        {
            return false;
        }
    }

    public void reset_data()
    {
        headVelocityHistory = new Queue<Vector3>();
    }

    private void readFromDataSource()
    {
        headVelocityHistory.Enqueue(head_velocity);
        if (headVelocityHistory.Count > lookbackWindow)
        {
            headVelocityHistory.Dequeue();
        }
        Vector3 pastHeadVelocity = headVelocityHistory.Peek();
        headSpeedy = Mathf.Abs(head_velocity.y);
        lookbackSpeedy = Mathf.Abs(pastHeadVelocity.y);
    }

    private bool validateSpeed()
    {
        float difference = Mathf.Abs(headSpeedy - lookbackSpeedy);
        if (headSpeedy > speedThreshold)
        {
            int counter = 0;
            foreach (Vector3 turn in headVelocityHistory)
            {
                Debug.Log("turny " + turn.y + " counter " + counter);
                counter++;
            }
            Debug.Log("difference " + difference);
            if (difference < speedDifferenceMin)
            {
                speedEvaluationMessage = "Too Slow";
                speedEvaluationRecord = -1;
            }
            else if (difference > speedDifferenceMax)
            {
                speedEvaluationMessage = "Too Fast";
                speedEvaluationRecord = 1;
            }
            else
            {
                speedEvaluationMessage = "Head Speed is Good";
                speedEvaluationRecord = 0;
            }
            return true;
        }
        else
        {
            return false;
        }
    }


}
