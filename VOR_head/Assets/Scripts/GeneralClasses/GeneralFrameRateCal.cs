using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class GeneralFrameRateCal : MonoBehaviour
{
    [SerializeField] private bool Run;

    public float Frame_rate { get { return frame_rate; } }

    private float frame_rate;
    private float last_time;

    private void Awake()
    {
        this.frame_rate = 0.0f;
        this.last_time = 0.0f;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Run)
        {
            FR_cal();
        }
        
    }

    private void FR_cal()
    {
        float curr_time = Time.time;
        frame_rate = 1.0f / (curr_time - last_time);
        last_time = curr_time;
    }
}
