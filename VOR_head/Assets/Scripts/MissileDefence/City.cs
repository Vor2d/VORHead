using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class City : MonoBehaviour {

    public TextMesh BIndicatorText;

    public int Health = 3;

    private MD_GameController MD_GC_Script;

	// Use this for initialization
	void Start () {
        if(MD_GC_Script == null)
        {
            MD_GC_Script = 
                GameObject.Find("MD_GameController").GetComponent<MD_GameController>();
        }

    }
	
	// Update is called once per frame
	void Update () {
        BIndicatorText.text = Health.ToString();
	}

    public void get_hit()
    {
        Health--;
        check_health();

        GetComponent<AudioSource>().Play();
    }

    private void check_health()
    {
        if(Health <= 0)
        {
            MD_GC_Script.City_destroied = true;
            Destroy(gameObject,1.0f);
        }
    }
}
