using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MD_TargetRayCast : GeneralRayCast
{

    public Vector3 TB_hit_position { get; set; }

    [SerializeField] private MD_GameController MDGC_script;

	// Use this for initialization
	protected override void Start ()
    {
        base.Start();

        this.TB_hit_position = new Vector3();
	}
	
	// Update is called once per frame
	protected override void FixedUpdate ()
    {
        base.FixedUpdate();

        check_multi_hits();
    }

    private void check_multi_hits()
    {
        MDGC_script.Menu_gazing_flag = false;
        for (int i = 0; i < Hits.Length; i++)
        {
            RaycastHit hit = Hits[i];
            Transform objectHit = hit.transform;
            if (objectHit != null)
            {
                if (objectHit.CompareTag(MD_StrDefiner.TargetBorder_tag))
                {
                    TB_hit_position = hit.point;
                }
                else if (MDGC_script.UsingHeadForMenu &&
                            objectHit.CompareTag(GeneralStrDefiner.WorldCanvasCollider_tag))
                {
                    Canvas_hit_position = hit.point;
                    MDGC_script.Menu_gazing_flag = true;
                }
            }
        }
    }
}