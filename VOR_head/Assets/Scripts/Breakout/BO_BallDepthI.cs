using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BO_BallDepthI : MonoBehaviour {

    private Transform Ball_TRANS;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        update_pos();
	}

    private void update_pos()
    {
        if (Ball_TRANS == null)
        {
            try
            {
                Ball_TRANS = GameObject.Find("BO_Ball").transform;
            }
            catch { Debug.Log("faild"); }
        }
        else
        {
            transform.position = new Vector3(transform.position.x,
                                                transform.position.y,
                                                Ball_TRANS.position.z);
        }
    }
}
