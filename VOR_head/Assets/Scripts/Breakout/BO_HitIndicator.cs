using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BO_HitIndicator : MonoBehaviour {

    [SerializeField] private float DecayTime = 1.0f;

    private Color init_color;
    private float timer;

	// Use this for initialization
	void Start () {
        this.init_color = GetComponent<MeshRenderer>().material.color;
        this.timer = DecayTime;

        StartCoroutine(decay_n_destroy());
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    IEnumerator decay_n_destroy()
    {
        Color present_color = init_color;
        while(timer > 0.0f)
        {
            timer -= Time.deltaTime;
            present_color.a = init_color.a * (timer / DecayTime);
            GetComponent<MeshRenderer>().material.color = present_color;
            yield return null;
        }
        Destroy(gameObject);
    }
}
