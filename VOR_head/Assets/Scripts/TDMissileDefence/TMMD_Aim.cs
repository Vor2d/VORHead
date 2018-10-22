using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TMMD_Aim : MonoBehaviour {

    public float ray_cast_distance = 100.0f;

    // Use this for initialization
    void Start () {
		
	}

    // Update is called once per frame
    void Update()
    {

        //       multi_raycast_hit();
        //       toggle_hit();

    }

 //   private void multi_raycast_hit()
 //   {
 //       Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

 //       hits = Physics.RaycastAll(ray, ray_cast_distance);
 //   }

 //   private void toggle_hit()
 //   {
 //       foreach (RaycastHit hit in hits)
 //       {
 //           GameObject hit_OBJ = hit.transform.gameObject;
 //           if (hit_OBJ.tag == "TDMissile")
 //           {
 //               hit_OBJ.GetComponent<TDMissile>().Aimed_flag = true;
 //           }

 //       }


 //   }
}
