using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;

/// <summary>
/// Must be used if using VR world canvs; Use the prefab default settings and link up objects to make things
/// happen;
/// </summary>
public class WorldCanvasController : MonoBehaviour {

    public enum ControllerButton { IndexTrigger};

    [SerializeField] private Camera RayCastCamera;
    [SerializeField] private GeneralRayCast GRC_script;
    [SerializeField] private Controller_Input CI_script;

    [SerializeField] private ControllerButton ClickButton = ControllerButton.IndexTrigger;

    private GameObject hit_OBJ;
    private PointerEventData point_data;
    private Vector3 hit_to_screen;
    private bool entered_flag;

    private void Awake()
    {
        
    }

    // Use this for initialization
    void Start () {
        register_click_button(execute_event);

        this.point_data = new PointerEventData(EventSystem.current);
        this.hit_to_screen = new Vector3();
        this.hit_OBJ = null;
        this.entered_flag = false;
    }
	
	// Update is called once per frame
	void Update () {
        try
        {
            hit_to_screen = 
                    RayCastCamera.WorldToScreenPoint(GRC_script.Canvas_hit_position);
        }
        catch { }
        hit_to_screen.z = 0.0f;
        point_data.position = hit_to_screen;

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(point_data, results);

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
            if (entered_flag)
            {
                ExecuteEvents.Execute(hit_OBJ, point_data, ExecuteEvents.pointerExitHandler);
                entered_flag = false;
                hit_OBJ = null;
            }
        }

        results.Clear();
    }

    private void register_click_button(Action event_fun)
    {
        switch(ClickButton)
        {
            case ControllerButton.IndexTrigger:
                CI_script.IndexTrigger += event_fun;
                break;
        }
    }

    private void deregister_click_button(Action event_fun)
    {
        switch (ClickButton)
        {
            case ControllerButton.IndexTrigger:
                CI_script.IndexTrigger -= event_fun;
                break;
        }
    }

    private void execute_event()
    {
        if (hit_OBJ != null)
        {
            ExecuteEvents.Execute(hit_OBJ, point_data,
                                                ExecuteEvents.pointerClickHandler);
        }
    }

    private void OnDestroy()
    {
        deregister_click_button(execute_event);
    }
}
