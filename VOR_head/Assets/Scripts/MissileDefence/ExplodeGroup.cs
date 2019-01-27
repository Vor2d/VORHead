using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodeGroup : MonoBehaviour
{
    public float Explode_time { get; set; }
    public float Explode_radius { get; set; }
    public bool Using_bonus{ get; set; }
    public bool Using_outline { get; set; }
    public bool Start_flag { get; set; }
    public bool Destroy_flag { get; set; }

    private void Awake()
    {
        this.Explode_time = 0.0f;
        this.Explode_radius = 0.0f;
        this.Using_bonus = false;
        this.Using_outline = false;
        this.Start_flag = false;
        this.Destroy_flag = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Destroy_flag)
        {
            Destroy(gameObject);
        }
    }

    public void start_explode_group(float ex_time, float ex_radius, bool us_bonus,
                                    bool us_outline)
    {
        Start_flag = true;
        set_parameters(ex_time, ex_radius, us_bonus, us_outline);
    }

    private void set_parameters(float ex_time, float ex_radius, bool us_bonus,
                                    bool us_outline)
    {
        Explode_time = ex_time;
        Explode_radius = ex_radius;
        Using_bonus = us_bonus;
        Using_outline = us_outline;
    }
}
