using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explode : MonoBehaviour {

    [SerializeField] private GameObject BonusCheck_Prefab;

    private MD_GameController MDGC_script;

    //[SerializeField] private bool UsingBonusSystem = false;
    public float Duration = 1.5f;
    public float ExpendSpeed = 1.0f;
    public float radius_scale = 1.0f;

    private bool start_flag;
    private float explode_timer;
    private int missile_hitted_number;

    private void Awake()
    {
        this.explode_timer = 0.0f;
        this.start_flag = false;
        this.missile_hitted_number = 0;
        if(MDGC_script == null)
        {
            MDGC_script = GameObject.Find(MD_StrDefiner.GameController_name).
                                            GetComponent<MD_GameController>();
        }
    }

    // Use this for initialization
    void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
        if (start_flag)
        {
            explode_timer += Time.deltaTime;
            float radius = explode_timer * ExpendSpeed;
            //Debug.Log("radius " + radius);
            radius *= radius_scale;
            transform.localScale = new Vector3(radius, radius, radius);
            if (explode_timer >= Duration)
            {
                if(MDGC_script.UsingBonusSystem)
                {
                    instantiate_bonus_system();
                }
                Destroy(gameObject);
            }
        }
	}

    public void start_exp()
    {
        start_flag = true;
    }

    public void set_radius_scale(float scale)
    {
        radius_scale = scale;
    }

    public void missile_hitted()
    {
        missile_hitted_number++;
    }

    private void instantiate_bonus_system()
    {
        GameObject bonus_GO =
                    Instantiate(BonusCheck_Prefab, transform.position, Quaternion.identity);
        bonus_GO.GetComponent<BonusCheck>().init(missile_hitted_number);
    }
}
