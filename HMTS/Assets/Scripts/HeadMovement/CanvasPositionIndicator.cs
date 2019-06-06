using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasPositionIndicator : MonoBehaviour {

    public Transform RelatedTransform;
    public Camera ShowingCamera;
    public Canvas ShowingCanvas;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        //Debug.Log("worldtoscreen "
        //        + ShowingCamera.WorldToViewportPoint(RelatedTransform.transform.position));

        Vector3 screenPos = ShowingCamera.
                                WorldToViewportPoint(RelatedTransform.transform.position);
        screenPos.x *= ShowingCanvas.GetComponent<RectTransform>().rect.width;
        screenPos.x -= ShowingCanvas.GetComponent<RectTransform>().rect.width / 2.0f;
        screenPos.y *= ShowingCanvas.GetComponent<RectTransform>().rect.height;
        screenPos.y -= ShowingCanvas.GetComponent<RectTransform>().rect.height / 2.0f;
        screenPos.z = 0.0f;

        //Debug.Log("screenpos " + screenPos);

        GetComponent<RectTransform>().localPosition = screenPos;
    }
}
