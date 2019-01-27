using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodeOutline : MonoBehaviour
{
    [SerializeField] private ExplodeGroup EG_script;

    // Start is called before the first frame update
    void Start()
    {
        set_scale();
        set_mesh();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void set_scale()
    {
        if(EG_script.Using_outline)
        {
            transform.localScale = new Vector3(EG_script.Explode_radius,
                                    EG_script.Explode_radius,
                                    EG_script.Explode_radius);
        }
    }

    private void set_mesh()
    {
        if(EG_script.Using_outline)
        {
            GetComponent<MeshRenderer>().enabled = true;
        }
        else
        {
            GetComponent<MeshRenderer>().enabled = false;
        }
    }
}
