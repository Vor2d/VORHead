using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BP_Path : MonoBehaviour
{
    public Transform[] Stations_TRANS;

    public BP_RC BPRC { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        if(BPRC == null)
        {
            this.BPRC = GameObject.Find(BP_StrDefiner.RC_name).GetComponent<BP_RC>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void init_path(BP_RC _BPRC)
    {
        BPRC = _BPRC;
    }

    public void destroy_path()
    {
        Destroy(gameObject);
    }
}
