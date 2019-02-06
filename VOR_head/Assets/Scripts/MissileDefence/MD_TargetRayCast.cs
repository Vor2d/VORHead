using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MD_TargetRayCast : GeneralRayCast
{
    [SerializeField] private MD_GameController MDGC_script;

    public Vector3 TB_hit_position { get; set; }

    private bool reload_gazing;

	// Use this for initialization
	protected override void Start ()
    {
        base.Start();

        this.TB_hit_position = new Vector3();
        this.reload_gazing = false;
	}
	
	// Update is called once per frame
	protected override void Update ()
    {
        base.Update();

        check_multi_hits();
    }

    private void check_multi_hits()
    {
        MDGC_script.Menu_gazing_flag = false;
        MDGC_script.Reload_gazing_flag = false;
        for (int i = 0; i < Hits.Length; i++)
        {
            RaycastHit hit = Hits[i];
            Transform objectHit = hit.transform;
            //Debug.Log("objectHit name " + objectHit.name);
            if (objectHit != null)
            {
                if (objectHit.CompareTag(MD_StrDefiner.TargetBorder_tag))
                {
                    TB_hit_position = hit.point;
                }
                if (MDGC_script.UsingHeadForMenu &&
                            objectHit.CompareTag(GeneralStrDefiner.WorldCanvasCollider_tag))
                {
                    Canvas_hit_position = hit.point;
                    MDGC_script.Menu_gazing_flag = true;
                }
                if(MDGC_script.MDDC_script.UsingReloadSystem &&
                            objectHit.CompareTag(MD_StrDefiner.ReloadCollider_tag))
                {
                    //Debug.Log("reload gazing");
                    MDGC_script.Reload_gazing_flag = true;
                    if(!reload_gazing)
                    {
                        reload_gazing = true;
                        MDGC_script.reload_gazing(objectHit);
                    }
                }
            }
        }
        if(!MDGC_script.Reload_gazing_flag)
        {
            reload_gazing = false;
        }

    }
}