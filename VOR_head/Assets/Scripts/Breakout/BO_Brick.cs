using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BO_Brick : MonoBehaviour {

    private float alpha;

	// Use this for initialization
	void Start () {
        this.alpha = GetComponent<Renderer>().material.color.a;

        random_color();
	}
	
	// Update is called once per frame
	void Update () {
        //random_color();
    }

    private void random_color()
    {
        float r = Random.Range(0.0f, 1.0f);
        float g = Random.Range(0.0f, 1.0f);
        float b = Random.Range(0.0f, 1.0f);

        //GetComponent<Renderer>().material.color = Color.blue;
        GetComponent<Renderer>().material.color = (new Color(r, g, b, alpha));
    }

    public void hited()
    {
        Destroy(gameObject);
    }
}
