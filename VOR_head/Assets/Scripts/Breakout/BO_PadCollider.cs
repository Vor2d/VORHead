using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BO_PadCollider : MonoBehaviour {

    public Vector3 Contact_point { get; private set; }
    public System.Action OnCEnter_CB { get; set; }
    public Transform Contact_TRANS { get; private set; }

    private void Awake()
    {
        Contact_point = new Vector3();
        OnCEnter_CB = null;
    }

    // Use this for initialization
    void Start () 
    {

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("BO_Ball"))
        {
            Contact_TRANS = collision.transform;
            Contact_point = collision.contacts[0].point;
            OnCEnter_CB?.Invoke();
        }
    }


}
