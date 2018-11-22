using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FS_GameController : MonoBehaviour {

    private const string score_init_str = "Score: ";

    public GameObject FruitPrefab;
    [SerializeField] private GameObject ScoreText;
    [SerializeField] private GameObject DebugText1;
    //[SerializeField] private GameObject Debug_OBJ1;

    public bool is_slicing { get; set; }

    public float SliceSpeed = 50.0f;

    public float FruitIntervalTime = 3.0f;
    public float RandRangeX = 15.0f;
    public float RandRangeY = 15.0f;
    public float RandRangeZ1 = 5.0f;
    public float RandRangeZ2 = 15.0f;
    public int ScoreIncrease = 10;

    private float fruit_inte_timer;
    private bool fruit_Itimer_flag;
    private Animator FSGCAnimator;
    private int score;
    private bool score_changed;

	// Use this for initialization
	void Start () {
        this.fruit_inte_timer = FruitIntervalTime;
        this.fruit_Itimer_flag = false;
        this.FSGCAnimator = GetComponent<Animator>();
        this.score = 0;
        this.score_changed = true;
        this.is_slicing = false;
	}
	
	// Update is called once per frame
	void Update () {
		if(fruit_Itimer_flag)
        {
            fruit_inte_timer -= Time.deltaTime;
        }

        update_score();

        slice();

        DebugText1.GetComponent<TextMesh>().text =
                                GeneralMethods.getVRspeed().ToString("F2");
        //DebugText1.GetComponent<TextMesh>().text =
        //                        Debug_OBJ1.GetComponent<FS_Fruit>().speed_cal.ToString();

    }

    public void ToInstantiateFruit()
    {
        instantiate_fruit();

        FSGCAnimator.SetTrigger("NextStep");
    }

    private void instantiate_fruit()
    {
        GameObject fruit_obj = 
                    Instantiate(FruitPrefab, rand_pos_generator(), new Quaternion());
        fruit_obj.GetComponent<FS_Fruit>().start_bubble();
    }

    private Vector3 rand_pos_generator()
    {
        return new Vector3(Random.Range(-RandRangeX, RandRangeX),
                            Random.Range(-RandRangeY, RandRangeY),
                            Random.Range(RandRangeZ1, RandRangeZ2));
    }

    public void ToWaitTime()
    {
        fruit_Itimer_flag = true;
    }

    public void WaitTime()
    {
        if(fruit_inte_timer < 0.0f)
        {
            fruit_inte_timer = FruitIntervalTime;

            FSGCAnimator.SetTrigger("NextStep");
        }
    }

    public void fruit_destroyed()
    {
        score += ScoreIncrease;
        score_changed = true;
    }

    private void update_score()
    {
        if(score_changed)
        {
            ScoreText.GetComponent<TextMesh>().text = 
                                score_init_str + score.ToString();
            score_changed = false;
        }
    }

    private void slice()
    {
        if(Input.GetAxis("Oculus_CrossPlatform_SecondaryIndexTrigger") > 0.5f)
        {
            is_slicing = true;
        }
        else
        {
            is_slicing = false;
        }
    }
}
