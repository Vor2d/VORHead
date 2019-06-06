//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class LastPosIndicator : MonoBehaviour {

//    private float init_dist;
//    private enum direction1 { left, right };

//    // Use this for initialization
//    void Start () {
//        this.init_dist = transform.position.z;

//        //changePosition(30.0f, 1);
//    }
	
//	// Update is called once per frame
//	void Update () {
		
//	}

//    public void changePosition(float ang_deg, int direc)
//    {
//        //Debug.Log("changePosition");
//        float ang_rand = ang_deg * Mathf.PI / 180.0f;

//        float newx = Mathf.Cos(ang_rand) * this.init_dist;
//        float newy = Mathf.Sin(ang_rand) * this.init_dist;

//        if (direc == (int)(direction1.left))
//        {
//            newy = -newy;
//        }

//        transform.position = new Vector3(newy, 0.0f, newx);

//        transform.LookAt(Camera.main.transform);
//    }
//}
