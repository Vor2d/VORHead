using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FS_CheckRayHit : MonoBehaviour {

    [SerializeField] private GeneralRayCast GRC_script;

    private RaycastHit[] raycastHits;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        if (raycastHits != null)
        {
            foreach (RaycastHit raycastHit in raycastHits)
            {
                if (raycastHit.transform.CompareTag("FS_FruitAimI"))
                {
                    raycastHit.transform.GetComponentInParent<FS_Fruit>().
                                                                Is_aimed_flag = false;
                }
            }
        }

        raycastHits = GRC_script.Hits;

        if (raycastHits != null)
        {
            foreach (RaycastHit raycastHit in raycastHits)
            {
                
                if (raycastHit.transform.CompareTag("FS_FruitAimI"))
                {
                    raycastHit.transform.GetComponentInParent<FS_Fruit>().
                                                                Is_aimed_flag = true;
                }
            }
        }


	}
}
