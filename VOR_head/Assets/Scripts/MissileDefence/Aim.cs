using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aim : MonoBehaviour {

    [SerializeField] private Controller_Input CI_script;
    [SerializeField] private MD_GameController MDGC_script;

    [SerializeField] private int AimKey;    //0 for button_0, 1 for index trigger;

    public bool state_one_flag { get; set; }

    // Use this for initialization
    void Start () {
        if (AimKey == 0)
        {
            CI_script.Button_X += shoot;
        }
        else if (AimKey == 1)
        {
            CI_script.IndexTrigger += shoot;
        }

        this.state_one_flag = false;
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

    private void OnDestroy()
    {
        if (AimKey == 0)
        {
            CI_script.Button_X -= shoot;
        }
        else if (AimKey == 1)
        {
            CI_script.IndexTrigger -= shoot;
        }
    }
}
