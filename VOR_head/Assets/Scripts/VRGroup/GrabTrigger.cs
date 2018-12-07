using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabTrigger : MonoBehaviour {

    public Transform other_TRANS { get; set; }

	// Use this for initialization
	void Start () {
		this.other_TRANS = null;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("GrabAble"))
        {
            other.GetComponent<GrabAble>().high_light();
            other_TRANS = other.transform;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("GrabAble"))
        {
            other.GetComponent<GrabAble>().de_high_light();
            other_TRANS = null;
        }
    }

    public void disable_trigger()
    {
        GetComponent<Collider>().enabled = false;
    }

    public void enable_trigger()
    {
        GetComponent<Collider>().enabled = true;
    }
}
