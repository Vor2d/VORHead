using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BP_GameController : MonoBehaviour {

    private const string score_init_str = "Score: ";

    public GameObject BubblePrefab;
    public Text ScoreText;

    public float BubbleIntervalTime = 3.0f;
    public float RandRangeX = 15.0f;
    public float RandRangeY = 15.0f;
    public float RandRangeZ1 = 5.0f;
    public float RandRangeZ2 = 15.0f;
    public int ScoreIncrease = 10;

    private float bubble_inte_timer;
    private bool bubble_Itimer_flag;
    private Animator BPGCAnimator;
    private int score;
    private bool score_changed;

	// Use this for initialization
	void Start () {
        this.bubble_inte_timer = BubbleIntervalTime;
        this.bubble_Itimer_flag = false;
        this.BPGCAnimator = GetComponent<Animator>();
        this.score = 0;
        this.score_changed = true;
	}
	
	// Update is called once per frame
	void Update () {
		if(bubble_Itimer_flag)
        {
            bubble_inte_timer -= Time.deltaTime;
        }

        update_score();
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

    public void bubble_destroyed()
    {
        score += ScoreIncrease;
        score_changed = true;
    }

    private void update_score()
    {
        if(score_changed)
        {
            ScoreText.text = score_init_str + score.ToString();
            score_changed = false;
        }
    }
}
