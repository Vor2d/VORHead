using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explode : MonoBehaviour {


    public float Duration = 1.5f;
    public float ExpendSpeed = 1.0f;

    private bool start_flag;
    private float explode_time;

    private void Awake()
    {
        this.explode_time = 0.0f;
        this.start_flag = false;
    }

    // Use this for initialization
    void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
        if (start_flag)
        {
            explode_time += Time.deltaTime;
            float radius = explode_time * ExpendSpeed;
            //Debug.Log("radius " + radius);
            transform.localScale = new Vector3(radius, radius, radius);
            if (explode_time >= Duration)
            {
                Destroy(gameObject);
            }
        }
	}

    public void start_exp()
    {
        start_flag = true;
    }
}
