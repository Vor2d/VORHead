using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bubble : MonoBehaviour {

    public GameObject BubbleRender;
    public GameObject BubbleAimIndicator;

    public bool Is_aimed_flag { get; set; }

    public float IncreaseSpeed = 1.0f;
    public float LastTime = 3.0f;
    //public float CriticalPercent = 0.8f;
    public float LastTransparent = 0.3f;
    public float InitTransparent = 0.6f;
    
    private float last_timer;
    private bool start_flag;
    private float BR_init_scale;
    private Color init_aimIn_color;
    private bool last_aimed_flag;
    //private bool critial_changed;

    private BP_InputManager BPIM_script;

    // Use this for initialization
    void Awake () {
        this.start_flag = false;
        this.BPIM_script = 
                    GameObject.Find("BP_InputManager").GetComponent<BP_InputManager>();
        this.last_aimed_flag = false;
        //this.critial_changed = false;
	}

    private void Start()
    {
        this.last_timer = LastTime;
        this.BR_init_scale = BubbleRender.transform.localScale.x;
        this.Is_aimed_flag = false;
        this.init_aimIn_color = 
                    BubbleAimIndicator.GetComponent<MeshRenderer>().material.color;

        start_bubble();
    }

    // Update is called once per frame
    void Update () {
		
        if(start_flag)
        {
            last_timer -= Time.deltaTime;
            float radius = (LastTime - last_timer) * IncreaseSpeed + BR_init_scale;
            BubbleRender.transform.localScale = new Vector3(radius, radius, radius);

            float time_scale = last_timer / LastTime;

            BubbleRender.GetComponent<MeshRenderer>().material.color =
                                                    new Color(1.0f * time_scale, 
                                                                1.0f * time_scale,
                                                                0.0f, 
                        (InitTransparent - LastTransparent) * time_scale + LastTransparent);

            //if(last_timer < LastTime * (1 - CriticalPercent) && !critial_changed)
            //{
            //    BubbleRender.GetComponent<MeshRenderer>().material.color = 
            //                                            new Color(1.0f,1.0f,0.0f,0.3f);
            //    critial_changed = true;
            //}

            if (last_timer < 0.0f)
            {
                Destroy(gameObject);
            }
        }

	}

    private void LateUpdate()
    {
        if (Is_aimed_flag)
        {
            if(last_aimed_flag != Is_aimed_flag)
            {
                BubbleAimIndicator.GetComponent<MeshRenderer>().material.color = Color.red;
                last_aimed_flag = Is_aimed_flag;
            }

            Is_aimed_flag = false;

            if(BPIM_script.Key_pressed)
            {
                Destroy(gameObject);
            }
        }
        else
        {
            if (last_aimed_flag != Is_aimed_flag)
            {
                BubbleAimIndicator.GetComponent<MeshRenderer>().material.color =
                                                                        init_aimIn_color;
                last_aimed_flag = Is_aimed_flag;
            }
        }
    }

    public void start_bubble()
    {
        start_flag = true;
    }

}
