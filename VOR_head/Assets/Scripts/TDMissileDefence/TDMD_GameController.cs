using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TDMD_GameController : MonoBehaviour {

    public GameObject MissilePrefab;
    public Transform PlayerTransform;

    public bool Fire_flag { get; set; }

    public float MissileIntervalTime = 3.0f;
    public float MissileInitZ;
    public float InstRandomRangeX = 9.0f;
    public float InstRandomRangeY = 9.0f;

    private bool wait_missile_timer;
    private float miss_interval_timer;

    private Animator TDMD_GCAnimator;


    // Use this for initialization
    void Start () {
        this.wait_missile_timer = false;
        this.miss_interval_timer = MissileIntervalTime;
        this.TDMD_GCAnimator = GetComponent<Animator>();
        this.Fire_flag = false;
	}
	
	// Update is called once per frame
	void Update () {
        Fire_flag = false;


        if (wait_missile_timer)
        {
            miss_interval_timer -= Time.deltaTime;
        }

        if(Input.GetKeyDown(KeyCode.JoystickButton0))
        {
            Fire_flag = true;
        }
	}

    public void ToSpawnMissile()
    {
        GameObject missile =
            Instantiate(MissilePrefab, RandomPosGenerate(), Quaternion.identity);
        TDMissile missile_script = missile.GetComponent<TDMissile>();
        missile_script.set_target(PlayerTransform);
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
}
