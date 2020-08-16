using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyParent : MonoBehaviour
{
    [SerializeField] protected bool AutoResetHeight; //Auto matically reset position to 0;
    protected static float refer_height = 0.0f;

    private void Update()
    {

    }

    public void set_ref_hei(float ref_hei)
    {
        refer_height = ref_hei;
    }

    protected void adjust_height()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y + refer_height, 
            transform.position.z);
    }
}
