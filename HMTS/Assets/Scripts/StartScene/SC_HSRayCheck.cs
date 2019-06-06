using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_HSRayCheck : MonoBehaviour
{
    [SerializeField] private GeneralRayCast GRC_script;
    [SerializeField] private SC_GameController SCGC_script;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //if(SCGC_script.UsingHeadForMenu && GRC_script.Hits != null)
        //{
        //    foreach(RaycastHit hit in GRC_script.Hits)
        //    {
        //        if(hit.transform.CompareTag(GeneralStrDefiner.WorldCanvasCollider_tag))
        //        {
        //            GRC_script.Canvas_hit_position = hit.point;
        //        }
        //    }
        //}
    }
}
