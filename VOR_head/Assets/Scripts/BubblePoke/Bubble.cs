using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bubble : MonoBehaviour {

    public float IncreaseSpeed = 1.0f;
    public float LastTime = 3.0f;

    private float last_timer;
    private bool start_flag;
    private float init_scale;

	// Use this for initialization
	void Awake () {
        this.last_timer = LastTime;
        this.start_flag = false;
        this.init_scale = transform.localScale.x;
	}

    private void Start()
    {
        start_bubble();
    }

    // Update is called once per frame
    void Update () {
		
        if(start_flag)
        {
            last_timer -= Time.deltaTime;
            float radius = (LastTime - last_timer) * IncreaseSpeed + init_scale;
            transform.localScale = new Vector3(radius, radius, radius);

            if (last_timer < 0.0f)
            {
                Destroy(gameObject);
            }
        }

	}

    public void start_bubble()
    {
        start_flag = true;
    }
}
