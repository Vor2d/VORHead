using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Breakout paddle hit indicator;
public class BO_PadHitInd : MonoBehaviour {

    [SerializeField] private GameObject HitIndicator;
    [SerializeField] BO_PadCollider BOPC_script;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(BOPC_script.contacted_flag)
        {
            Instantiate(HitIndicator, new Vector3(BOPC_script.contact_point.x, 
                                                    BOPC_script.contact_point.y, 
                                                    transform.position.z - 0.3f),
                                    new Quaternion(), transform);
            BOPC_script.contacted_flag = false;
        }
	}


}
