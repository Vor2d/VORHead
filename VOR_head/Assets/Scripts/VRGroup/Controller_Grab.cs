using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller_Grab : MonoBehaviour {

    [SerializeField] GrabTrigger GT_script;
    [SerializeField] Controller_Input CI_script;
    //[SerializeField] Transform FixJoint;

    private bool empty_grabed;
    private bool grab_triggered;
    private Transform last_other;

    // Use this for initialization
    void Start () {
        this.empty_grabed = false;
        this.grab_triggered = false;
	}
	
	// Update is called once per frame
	void Update () {
		//if(Input.GetAxis("Oculus_CrossPlatform_SecondaryHandTrigger") > 0.5f)
  //      {
  //          grab_button_down = true;
  //      }
  //      else
  //      {
  //          grab_button_down = false;
  //          empty_grabed = false;
  //          release();
  //      }

        if(CI_script.Index_trigger && GT_script.other_TRANS == null)
        {
            empty_grabed = true;
        }
        else if(CI_script.Index_trigger && !empty_grabed && !grab_triggered)
        {
            grab_triggered = true;
            grab();
        }
        else if(!CI_script.Index_trigger)
        {
            empty_grabed = false;
            release();
        }

        if(GT_script.other_TRANS != null)
        {
            last_other = GT_script.other_TRANS;
        }
    }

    private void grab()
    {
        GetComponent<FixedJoint>().connectedBody =
                GT_script.other_TRANS.GetComponent<Rigidbody>();
        Debug.Log("grab " + GT_script.other_TRANS.name);
        GT_script.disable_trigger();
    }

    private void release()
    {
        if(grab_triggered)
        {
            GetComponent<FixedJoint>().connectedBody = null;
            last_other.GetComponent<Rigidbody>().velocity =
                        OVRInput.GetLocalControllerVelocity(CI_script.Controller_type);
            grab_triggered = false;
            GT_script.enable_trigger();
        }
    }
    

}
