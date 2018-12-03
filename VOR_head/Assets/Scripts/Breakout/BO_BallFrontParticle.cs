using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BO_BallFrontParticle : MonoBehaviour {

    private bool particle_on;

	// Use this for initialization
	void Start () {
        this.particle_on = false;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        if( other.name == "PadColliders" &&  !particle_on)
        {
            GetComponent<ParticleSystem>().Play();
            //particle_on = true;
            Debug.Log("OnTriggerEnter ball");
        }
    }

    //private void OnTriggerExit(Collider other)
    //{
    //    if (other.name == "PadColliders" && particle_on)
    //    {
    //        GetComponent<ParticleSystem>().Stop();
    //        particle_on = false;
    //    }

    //}
}
