using UnityEngine;

public class FS_CheckRayHit : MonoBehaviour {

    [SerializeField] private GeneralRayCast GRC_script;

    //private RaycastHit[] raycastHits;

	// Use this for initialization
	void Start () {		
	}
	
	// Update is called once per frame
	void Update () {
	}

    public bool check_ray_to_start()
    {
        if (GRC_script.Hits != null)
        {
            foreach (RaycastHit raycastHit in GRC_script.Hits)
            {
                //Debug.Log("raycastHit " + raycastHit.transform.name);
                if (raycastHit.transform.CompareTag(FS_SD.FruitStartI_Tag))
                {
                    return true;
                }
            }
        }
        return false;
    }

    public Vector3 check_ray_to_plane()
    {
        if(GRC_script.Hits != null)
        {
            foreach (RaycastHit raycastHit in GRC_script.Hits)
            {
                if (raycastHit.transform.CompareTag(FS_SD.FruitPlane_Tag))
                {
                    return raycastHit.point;
                }
            }
        }
        return Vector3.zero;
    }
}
