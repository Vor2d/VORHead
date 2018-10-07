using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class City : MonoBehaviour {

    public int Health = 3;

    public MD_GameController MD_GC_Script;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void get_hit()
    {
        Health--;
        check_health();
    }

    private void check_health()
    {
        if(Health <= 0)
        {
            MD_GC_Script.City_destroied = true;
            Destroy(gameObject);
        }
    }
}
