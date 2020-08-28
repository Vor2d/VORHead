using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BO_Pad : MonoBehaviour {

	[SerializeField] private BO_PadCollider PTrigger_script;
	[SerializeField] private bool Using_collider;
	[SerializeField] private bool Using_trigger;
	[SerializeField] private Transform Collider_TRANS;
	[SerializeField] private Transform Trigger_TRANS;
	[SerializeField] private float Trigger_center_radius;
	[SerializeField] private float Trigger_max_angle;
	[SerializeField] private float Trigger_max_radius;

	private Vector3 hit_pos;

    private void Awake()
    {
		hit_pos = new Vector3();
		turn_on_trigger_collider();
	}

    // Use this for initialization
    void Start () 
	{
		register_bounce();
	}
	
	// Update is called once per frame
	void Update () 
	{
		check_ray_pos();
	}

    private void OnDestroy()
    {
		deregister_bounce();
	}

    private void register_bounce()
    {
		if (Using_trigger) { PTrigger_script.OnCEnter_CB += bounce; }
    }

	private void deregister_bounce()
    {
		if (Using_trigger) { PTrigger_script.OnCEnter_CB -= bounce; }
	}

	private void turn_on_trigger_collider()
    {
		if (Using_collider) { Collider_TRANS.gameObject.SetActive(true); }
		if (Using_trigger) { Trigger_TRANS.gameObject.SetActive(true); }
    }

	private void check_ray_pos()
    {
		hit_pos = BO_RC.IS.CRH_script.Hit_position;
		if (hit_pos.z != 0.0f && !BO_GameController.IS.Game_paused)
		{
			transform.position = hit_pos;
		}
	}

	public void bounce()
    {
		Transform c_TRANS = PTrigger_script.Contact_TRANS;
		Vector3 speed = c_TRANS.GetComponent<Rigidbody>().velocity;
		float momentum = speed.magnitude;
		Vector3 point = PTrigger_script.Contact_point;
		Vector3 dir = dir_cal(point).normalized;
		c_TRANS.GetComponent<Rigidbody>().velocity = dir * momentum;
	}

	private Vector3 dir_cal(Vector3 point)
    {
		float dist = Vector3.Distance(point, transform.position);
		if (dist <= Trigger_center_radius) { return Vector3.forward; }
		else
        {
			float ang_prop = Trigger_max_angle / 45.0f;
			Vector3 pad_vec = point - transform.position;
			Vector3 dir_to45deg = pad_vec / Trigger_max_radius;
			Vector3 dir = dir_to45deg * ang_prop;
			dir.z = 1.0f;
			return dir;
		}
    }

}
