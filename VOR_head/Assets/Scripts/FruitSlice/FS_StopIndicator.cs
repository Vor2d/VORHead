using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FS_StopIndicator : MonoBehaviour {

    [SerializeField] private FS_Fruit FSF_script;
    [SerializeField] private GameObject StopIndicator_Prefab;
    [SerializeField] private FS_FruitIndicator FSFI_script;

    // Use this for initialization
    void Start () {

    }
	
	// Update is called once per frame
	void Update () {

	}

    private void mark_cut()
    {
        //if(!FI_null)
        //{
        //    Vector3 hit_point = FSF_script.FSCRH_script.check_ray_to_plane();
        //    GameObject stop_IOBJ = 
        //            Instantiate(StopIndicator_Prefab, hit_point, new Quaternion());
        //    Vector3[] positions = { FSFI_script.StartI_TRANS.position, hit_point };
        //    stop_IOBJ.GetComponent<LineRenderer>().SetPositions(positions);
        //}

    }
}
