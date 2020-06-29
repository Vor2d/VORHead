using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FS_StopIndicator : MonoBehaviour {

    [SerializeField] private FS_Fruit F_script;
    [SerializeField] private GameObject StopIndicator_Prefab;
    [SerializeField] private FS_FruitIndicator FSFI_script;

    private FS_RayCast RC_cache;

    private void Awake()
    {
        this.RC_cache = null;
    }

    // Use this for initialization
    void Start ()
    {
        RC_cache = FS_RC.IS.RC_script;
    }
	
	// Update is called once per frame
	void Update () {

	}

    public void mark_cut()
    {
        Vector3 hit_point = new Vector3();
        if(RC_cache.check_object_pos(FS_SD.FruitPlane_tag,out hit_point))
        {
            spawn_stop_indi(hit_point, 
                    FSFI_script.indicators_TRANSs[FSFI_script.activated_index].position);
        }
    }

    public void mark_cut(Vector3 stop_pos)
    {
        spawn_stop_indi(stop_pos, FSFI_script.indicators_TRANSs[FSFI_script.activated_index].position);
    }

    private void spawn_stop_indi(Vector3 pos, Vector3 start_pos)
    {
        GameObject stop_IOBJ = Instantiate(StopIndicator_Prefab, pos, new Quaternion());
        Vector3[] positions = { start_pos, pos };
        stop_IOBJ.GetComponent<LineRenderer>().SetPositions(positions);
    }
}
