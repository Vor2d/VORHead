﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangePosition : MonoBehaviour {

    private enum direction { left, right };

    [SerializeField] private bool UsingLookAt;
    [SerializeField] private bool UsingSphere;

    private float init_dist;

    // Use this for initialization
    void Start () {
        this.init_dist = transform.position.z;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    //public void changePosition(float ang_deg, int direc)
    //{
    //    //Debug.Log("changePosition");
    //    float ang_rand = ang_deg * Mathf.PI / 180.0f;

    //    float distance = init_dist + offset_dist;

    //    float newx = Mathf.Cos(ang_rand) * distance;
    //    float newy = Mathf.Sin(ang_rand) * distance;

    //    if (direc == (int)(direction.left))
    //    {
    //        newy = -newy;
    //    }

    //    transform.position = new Vector3(newy, 0.0f, newx);

    //    transform.LookAt(Camera.main.transform);
    //}

    //x is horizontal, y is vertical;
    public void changePosition(float ang_degX, float ang_degY, int direcX, int direcY,float scale = 1.0f)
    {
        float init_dist = this.init_dist * scale;
        if(!UsingSphere)
        {
            transform.position =
                GeneralMethods.PositionCal(init_dist, ang_degX, ang_degY, direcX, direcY);
        }
        else
        {
            transform.position =
                GeneralMethods.PositionCalSphere(init_dist, ang_degX, ang_degY, direcX, direcY);
        }

        //Debug.Log("changePosition " + ang_degX);

        if (UsingLookAt) { transform.LookAt(Camera.main.transform); }
    }

    public void changePosition(float ang_degX, float ang_degY)
    {
        transform.position =
                GeneralMethods.PositionCal(init_dist, ang_degX, ang_degY);
        //Debug.Log("changePosition2 " + ang_degX);
    }
}
