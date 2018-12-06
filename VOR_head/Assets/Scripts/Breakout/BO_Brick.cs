using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BO_Brick : MonoBehaviour {

    [SerializeField] private GameObject BrickParticle_Prefab;

    private float alpha;

    private void Awake()
    {
        this.alpha = GetComponent<Renderer>().material.color.a;

        random_color();
    }

    // Use this for initialization
    void Start () {

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
        GetComponent<Collider>().enabled = false;
        Instantiate(BrickParticle_Prefab, transform.position,new Quaternion());
        Destroy(gameObject);
    }

    public void update_shadow()
    {
        foreach(Transform child in transform)
        {
            if(child.CompareTag("BO_BrickRayCasts"))
            {
                foreach(Transform cchild in child)
                {
                    cchild.GetComponent<BO_BrickRayCast>().update_shadow();
                    //Debug.Log("1 Called!");
                }
            }
        }
    }
}
