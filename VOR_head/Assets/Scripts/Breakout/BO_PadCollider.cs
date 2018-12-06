using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BO_PadCollider : MonoBehaviour {

    public Vector3 contact_point { get; set; }
    public bool contacted_flag { get; set; }

	// Use this for initialization
	void Start () {
        contact_point = new Vector3();
        contacted_flag = false;

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("OnCollisionEnter " + collision.transform.name);
        if (collision.transform.CompareTag("BO_Ball"))
        {
            //Instantiate(HitIndicator,collision.contacts)
            contact_point = collision.contacts[0].point;
            contacted_flag = true;
        }
    }
}
