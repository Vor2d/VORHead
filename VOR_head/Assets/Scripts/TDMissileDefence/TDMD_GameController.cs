using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TDMD_GameController : MonoBehaviour {

    private const string score_init_str = "Score: ";
    private const string life_init_str = "Life: ";

    public GameObject[] Cities;
    public GameObject MissilePrefab;
    public Transform PlayerTransform;
    public GameObject ScoreText_OBJ;
    public GameObject LifeText_OBJ;
    public TDMD_Player TDMDPlyaer_script;

    public bool Text_changed_flag { get; set; }
    public bool Fire_flag { get; set; }

    public float MissileIntervalTime = 3.0f;
    public float MissileInitZ = 10.0f;
    public float InstRandomRangeX = 9.0f;
    public float InstRandomRangeY = 9.0f;
    public int ScoreIncrease = 10;

    private bool wait_missile_timer;
    private float miss_interval_timer;
    private int score;

    private Animator TDMD_GCAnimator;


    // Use this for initialization
    void Start () {
        this.wait_missile_timer = false;
        this.miss_interval_timer = MissileIntervalTime;
        this.TDMD_GCAnimator = GetComponent<Animator>();
        this.Fire_flag = false;
        this.score = 0;
        this.Text_changed_flag = true;
	}
	
	// Update is called once per frame
	void Update () {
        Fire_flag = false;


        if (wait_missile_timer)
        {
            miss_interval_timer -= GeneralGameController.GameDeltaTime;
        }

        if(Input.GetKeyDown(KeyCode.JoystickButton0))
        {
            Fire_flag = true;
        }

        update_text();
	}

    public void ToSpawnMissile()
    {
        GameObject missile =
            Instantiate(MissilePrefab, RandomPosGenerate(), Quaternion.identity);
        TDMissile missile_script = missile.GetComponent<TDMissile>();
        int randomindex = Random.Range(0, 5);
        missile_script.set_target(Cities[randomindex].transform);
        missile_script.start_move();

        TDMD_GCAnimator.SetTrigger("NextStep");
    }

    private Vector3 RandomPosGenerate()
    {
        return new Vector3(Random.Range(-InstRandomRangeX, InstRandomRangeX),
                            Random.Range(-InstRandomRangeY, InstRandomRangeY),
                            MissileInitZ);
    }

    public void ToWaitTimeInterval()
    {
        wait_missile_timer = true;
    }

    public void WaitTimeInterval()
    {
        if (miss_interval_timer <= 0.0f)
        {
            miss_interval_timer = MissileIntervalTime;
            TDMD_GCAnimator.SetTrigger("NextStep");
        }
    }

    public void missile_destroyed()
    {
        score += ScoreIncrease;
        Text_changed_flag = true;
    }

    private void update_text()
    {
        if(Text_changed_flag)
        {
            ScoreText_OBJ.GetComponent<TextMesh>().text = score_init_str + score.ToString();
            LifeText_OBJ.GetComponent<TextMesh>().text = 
                                        life_init_str + TDMDPlyaer_script.life.ToString();
        }
    }
}
