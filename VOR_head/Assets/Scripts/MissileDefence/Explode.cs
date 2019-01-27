using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explode : MonoBehaviour {

    [SerializeField] private GameObject BonusCheck_Prefab;
    [SerializeField] private ExplodeGroup EG_script;
    //[SerializeField] private Transform Outline_TRANS;

    //private MD_GameController MDGC_script;

    private float explode_timer;
    private int missile_hitted_number;


    private void Awake()
    {
        this.explode_timer = 0.0f;
        this.missile_hitted_number = 0;
    }

    // Use this for initialization
    void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
        if (EG_script.Start_flag)
        {
            expand();
        }
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

    private void expand()
    {
        explode_timer += Time.deltaTime;
        float radius = (explode_timer / EG_script.Explode_time) * EG_script.Explode_radius;
        transform.localScale = new Vector3(radius, radius, radius);
        if (explode_timer >= EG_script.Explode_time)
        {
            if (EG_script.Using_bonus)
            {
                instantiate_bonus_system();
            }
            //Destroy(gameObject);
            EG_script.Destroy_flag = true;
        }
    }


}
