using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseRotate : MonoBehaviour {

    public float sensitivity = 10f;

    // Use this for initialization
    void Start () {
		
	}

    // Update is called once per frame
    
    void Update()
    {
        transform.Rotate(0, Input.GetAxis("Mouse X") * sensitivity, 0);
        transform.Rotate(-Input.GetAxis("Mouse Y") * sensitivity, 0, 0);
        //transform.Rotate(0, 0, -Input.GetAxis("QandE") * 90 * Time.deltaTime);
        //if (Input.GetMouseButtonDown(0))
        //    Cursor.lockState = CursorLockMode.Locked;
    }
}
