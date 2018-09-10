using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class CameraScale: MonoBehaviour {

    //public GameObject main_camera;
    private PreferenceLoader pl;


	// Use this for initialization
	void Start () {
        if (GameObject.Find("PreferenceLoader") == null)
        {
            GameObject obj = Instantiate(Resources.Load("PreferenceLoader")) as GameObject;
            obj.name = "PreferenceLoader";
        }
        pl = GameObject.Find("PreferenceLoader").GetComponent<PreferenceLoader>(); // Get PreferenceLoader script which stores all user setting.
        //Debug.Log("Screen height : " + Screen.height);
        //Debug.Log("Screen Width : " + Screen.width);
    }
	
	// Update is called once per frame
	void Update () {
        
        transform.rotation = new Quaternion(-pl.sceneGain * InputTracking.GetLocalRotation(XRNode.Head).x,
                                            -pl.sceneGain * InputTracking.GetLocalRotation(XRNode.Head).y,
                                            -pl.sceneGain * InputTracking.GetLocalRotation(XRNode.Head).z,
                                            InputTracking.GetLocalRotation(XRNode.Head).w);

        /*transform.position = new Vector3(-1 * (InputTracking.GetLocalPosition(XRNode.Head).x),
                            -1 * (InputTracking.GetLocalPosition(XRNode.Head).y),
                            -1 * (InputTracking.GetLocalPosition(XRNode.Head).z));*/       

    }
}
