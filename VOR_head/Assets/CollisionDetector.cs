using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class CollisionDetector : MonoBehaviour {

    OVRDisplay camera = new OVRDisplay();
    /*private void OnCollisionEnter(Collision col)
    {
        if(col.gameObject.name == "Face")
        {
            Debug.Log("1");
        }
    }*/
    private void OnTriggerEnter(Collider other)
    {
        print("enter"+other.name + "," + InputTracking.GetLocalRotation(XRNode.CenterEye).eulerAngles.y);

    }
    private void OnTriggerExit (Collider other)
    {
        print("leave" + other.name + "," + InputTracking.GetLocalRotation(XRNode.CenterEye).eulerAngles.y);
    }
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
