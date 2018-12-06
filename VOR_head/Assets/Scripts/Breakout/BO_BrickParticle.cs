using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BO_BrickParticle : MonoBehaviour {

    [SerializeField] private float DesTime = 0.2f;

	// Use this for initialization
	void Start () {
        Destroy(gameObject, DesTime);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
