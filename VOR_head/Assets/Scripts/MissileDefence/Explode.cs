using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explode : MonoBehaviour {


    public float Duration = 1.5f;
    public float ExpendSpeed = 1.0f;
    public float radius_scale = 1.0f;

    private bool start_flag;
    private float explode_timer;

    private void Awake()
    {
        this.explode_timer = 0.0f;
        this.start_flag = false;
    }

    // Use this for initialization
    void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
        if (start_flag)
        {
            explode_timer += Time.deltaTime;
            float radius = explode_timer * ExpendSpeed;
            //Debug.Log("radius " + radius);
            radius *= radius_scale;
            transform.localScale = new Vector3(radius, radius, radius);
            if (explode_timer >= Duration)
            {
                Destroy(gameObject);
            }
        }
	}

    public void start_exp()
    {
        start_flag = true;
    }

    public void set_radius_scale(float scale)
    {
        radius_scale = scale;
    }
}
