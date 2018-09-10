using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class cubetest : MonoBehaviour {
    private VRController vrController;
    // Use this for initialization
    [SerializeField] Transform target;
    Vector3 _offset;

    void Start () {
        vrController = GetComponent<VRController>();
        _offset = target.position - transform.position;
    }
	
	// Update is called once per frame
	void Update () {
        //transform.Rotate(0, vrController.angularVelocityRead.y * Time.deltaTime * 1, 0);
        //transform.position = new Vector3(0, 0, this.transform.parent.transform.position.z+5.0f);
        //transform.Rotate(0, 5, 0);
        Debug.Log(InputTracking.GetLocalRotation(XRNode.Head).y);
        //Quaternion rotation = Quaternion.Euler(0, InputTracking.GetLocalRotation(XRNode.Head).eulerAngles.y *0.5f, 0);
        transform.position = target.position - (InputTracking.GetLocalRotation(XRNode.Head) * _offset);
        transform.LookAt(target);
    }
}
