using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BO_Brick : MonoBehaviour {

    [SerializeField] private GameObject BrickParticle_Prefab;
    [SerializeField] private bool Using_brick_shadows;

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
        Instantiate(BrickParticle_Prefab, transform.position,new Quaternion());
        clean_destroy();
    }

    private void clean_destroy()
    {
        GetComponent<Collider>().enabled = false;
        BO_RC.IS.Bricks_pool.Remove(transform);
        Destroy(gameObject);
    }

    public void update_shadow()
    {
        if (!Using_brick_shadows) { return; }
        foreach(Transform child in transform)
        {
            if(child.CompareTag("BO_BrickRayCasts") && child.gameObject.activeSelf)
            {
                foreach(Transform cchild in child)
                {
                    cchild.GetComponent<BO_BrickRayCast>().update_shadow();
                    //Debug.Log("1 Called!");
                }
            }
        }
    }

    public void despawn_by_pos(float z)
    {
        if(transform.position.z <= z)
        {
            BO_GameController.IS.brick_despawned();
            clean_destroy();
        }
    }
}
