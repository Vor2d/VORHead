using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MD_HeadTarget : MonoBehaviour {

    [SerializeField] private Transform HS_TRANS;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        transform.LookAt(HS_TRANS.position);
        //transform.Rotate(new Vector3(180.0f, 0.0f, 0.0f));
        //transform.forward = Vector3.down;
        //transform.up = -transform.position;
    }
}
