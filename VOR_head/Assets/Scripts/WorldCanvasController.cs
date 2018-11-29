using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class WorldCanvasController : MonoBehaviour {

    [SerializeField] private Camera RayCastCamera;
    [SerializeField] private RightController RC_script;

    private GameObject hit_OBJ;
    private PointerEventData point_data;
    private Vector3 hit_to_screen;
    private bool entered_flag;
    private bool Rcontroller_trigger_flag;

    // Use this for initialization
    void Start () {
        this.point_data = new PointerEventData(EventSystem.current);
        this.hit_to_screen = new Vector3();
        this.hit_OBJ = null;
        this.entered_flag = false;
        this.Rcontroller_trigger_flag = false;
    }
	
	// Update is called once per frame
	void Update () {
        try
        {
            hit_to_screen = RayCastCamera.WorldToScreenPoint(RC_script.hit_point);
        }
        catch { }
        hit_to_screen.z = 0.0f;
        point_data.position = hit_to_screen;

        //Debug.Log("hit_to_screen" + hit_to_screen);

        Vector3[] positions = { transform.position, RC_script.hit_point };
        GetComponent<LineRenderer>().SetPositions(positions);

        right_controller();
    }

    private void FixedUpdate()
    {
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(point_data, results);

        Debug.Log("result " + results.Count);
        foreach(RaycastResult rcr in results)
        {
            Debug.Log("result object " + rcr.gameObject.name);
        }

        if (results.Count > 0)
        {
            if (!GameObject.ReferenceEquals(hit_OBJ, results[results.Count - 1].gameObject))
            {
                hit_OBJ = results[results.Count - 1].gameObject;
                ExecuteEvents.Execute(hit_OBJ, point_data, ExecuteEvents.pointerEnterHandler);
                entered_flag = true;
            }
        }
        else
        {
            if(entered_flag)
            {
                ExecuteEvents.Execute(hit_OBJ, point_data, ExecuteEvents.pointerExitHandler);
                entered_flag = false;
                hit_OBJ = null;
            }
        }

        results.Clear();
    }

    private void right_controller()
    {
        if (Input.GetAxis("Oculus_CrossPlatform_SecondaryIndexTrigger") > 0.5f &&
                                                                !Rcontroller_trigger_flag)
        {
            if (hit_OBJ != null)
            {
                ExecuteEvents.Execute(hit_OBJ, point_data,
                                                    ExecuteEvents.pointerClickHandler);
            }
            Rcontroller_trigger_flag = true;
        }
        else if (Input.GetAxis("Oculus_CrossPlatform_SecondaryIndexTrigger") < 0.1f)
        {
            Rcontroller_trigger_flag = false;
        }
    }
}
