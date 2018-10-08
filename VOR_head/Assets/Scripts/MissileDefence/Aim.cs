using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aim : MonoBehaviour {

    public GameObject ExplodePrefab;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mouse_pos = Input.mousePosition;
            Vector3 click_pos = new Vector3();
            click_pos = Camera.main.ScreenToWorldPoint(
                                new Vector3(mouse_pos.x, mouse_pos.y, 10.0f));

            GameObject explode =
                        Instantiate(ExplodePrefab, click_pos, Quaternion.identity);
            explode.GetComponent<Explode>().start_exp();

        }
    }
}
