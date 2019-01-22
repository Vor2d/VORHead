using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aim : MonoBehaviour {

    [SerializeField] private Controller_Input CI_script;

    public MD_GameController MDGC_script;

    public bool state_one_flag { get; set; }

    private void Awake()
    {
        CI_script.Button_A += shoot;
    }

    // Use this for initialization
    void Start () {
        this.state_one_flag = true;
	}
	
	// Update is called once per frame
	void Update () {

    }

    private void shoot()
    {
        if(MDGC_script.ammo_spend())
        {
            if (state_one_flag)
            {
                MDGC_script.state_one_Iexplosion();
            }
            else
            {
                MDGC_script.IE_with_raycast();
            }
        }
        else
        {

        }

    }
}
