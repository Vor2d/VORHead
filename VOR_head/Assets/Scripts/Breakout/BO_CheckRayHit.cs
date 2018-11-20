using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BO_CheckRayHit : MonoBehaviour {

    private const string HitTag = "BO_PadLayer";

    [SerializeField] private GeneralRayCast GRC_script;

    public Vector3 Hit_position { get; set; }

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void FixedUpdate () {

        check_hits();

    }

    private void check_hits()
    {
        if(GRC_script.Hits != null)
        {
            for (int i = 0; i < GRC_script.Hits.Length; i++)
            {
                RaycastHit hit = GRC_script.Hits[i];
                Transform objectHit = hit.transform;
                {
                    if (objectHit != null)
                    {
                        if (objectHit.tag == HitTag)
                        {
                            Hit_position = hit.point;
                        }
                    }
                }
            }
        }
    }


}
