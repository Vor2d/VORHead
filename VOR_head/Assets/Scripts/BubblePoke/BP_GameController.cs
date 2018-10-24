using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BP_GameController : MonoBehaviour {

    public GameObject BubblePrefab;

    public float BubbleIntervalTime = 3.0f;
    public float RandRangeX = 15.0f;
    public float RandRangeY = 15.0f;
    public float RandRangeZ1 = 5.0f;
    public float RandRangeZ2 = 15.0f;

    private float bubble_inte_timer;
    private bool bubble_Itimer_flag;
    private Animator BPGCAnimator;

	// Use this for initialization
	void Start () {
        this.bubble_inte_timer = BubbleIntervalTime;
        this.bubble_Itimer_flag = false;
        this.BPGCAnimator = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
		if(bubble_Itimer_flag)
        {
            bubble_inte_timer -= Time.deltaTime;
        }
	}

    public void ToInstantiateBubble()
    {
        instantiate_bubble();

        BPGCAnimator.SetTrigger("NextStep");
    }

    private void instantiate_bubble()
    {
        GameObject bubble_obj = 
                    Instantiate(BubblePrefab, rand_pos_generator(), new Quaternion());
        bubble_obj.GetComponent<Bubble>().start_bubble();
    }

    private Vector3 rand_pos_generator()
    {
        return new Vector3(Random.Range(-RandRangeX, RandRangeX),
                            Random.Range(-RandRangeY, RandRangeY),
                            Random.Range(RandRangeZ1, RandRangeZ2));
    }

    public void ToWaitTime()
    {
        bubble_Itimer_flag = true;
    }

    public void WaitTime()
    {
        if(bubble_inte_timer < 0.0f)
        {
            bubble_inte_timer = BubbleIntervalTime;

            BPGCAnimator.SetTrigger("NextStep");
        }
    }
}
