using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller_Input : MonoBehaviour {

    //public enum ControllerType { LeftController,RightController}
    public const string LeftIndexTrigger_str = "Oculus_CrossPlatform_PrimaryIndexTrigger";
    public const string RightIndexTrigger_str = "Oculus_CrossPlatform_SecondaryIndexTrigger";

    
    public OVRInput.Controller Controller_type;

    public bool Forward_flag { get; set; }
    public bool Index_trigger { get; set; }
    public bool Left_flag { get; set; }
    public bool Right_flag { get; set; }
    public System.Action Button_B { get; set; }

    private void Awake()
    {
        this.Button_B = null;
    }

    // Use this for initialization
    void Start () {
        this.Forward_flag = false;
        this.Index_trigger = false;
        this.Left_flag = false;
        this.Right_flag = false;
	}
	
	// Update is called once per frame
	void Update () {

        switch(Controller_type)
        {
            case OVRInput.Controller.LTouch:
                {
                    //if (Input.GetAxis("Oculus_CrossPlatform_PrimaryThumbstickVertical") 
                    //                                                                > 0.5f)
                    //{
                    //    Forward_flag = true;
                    //}
                    //else
                    //{
                    //    Forward_flag = false;
                    //}

                    if (Input.GetAxis(LeftIndexTrigger_str) > 0.5f)
                    {
                        Index_trigger = true;
                    }
                    else
                    {
                        Index_trigger = false;
                    }
                    if (Input.GetAxis("Oculus_CrossPlatform_PrimaryThumbstickHorizontal")
                                                                < -0.5f)
                    {
                        Left_flag = true;
                        Right_flag = false;
                    }
                    else if (Input.GetAxis("Oculus_CrossPlatform_PrimaryThumbstickHorizontal")
                                                                    > -0.5f &&
                            Input.GetAxis("Oculus_CrossPlatform_PrimaryThumbstickHorizontal")
                                                                    < 0.5f)
                    {
                        Left_flag = false;
                        Right_flag = false;
                    }
                    else if (Input.GetAxis("Oculus_CrossPlatform_PrimaryThumbstickHorizontal")
                                            > 0.5f)
                    {
                        Left_flag = false;
                        Right_flag = true;
                    }

                    break;
                }
            case OVRInput.Controller.RTouch:
                {
                    //R vertical thumb stick;
                    if (Input.GetAxis("Oculus_CrossPlatform_SecondaryThumbstickVertical")
                                                                                    > 0.5f)
                    {
                        Forward_flag = true;
                    }
                    else
                    {
                        Forward_flag = false;
                    }
                    //R index trigger;
                    if (Input.GetAxis(RightIndexTrigger_str) > 0.5f)
                    {
                        Index_trigger = true;
                    }
                    else
                    {
                        Index_trigger = false;
                    }
                    //R B button;
                    if(Input.GetKeyDown(KeyCode.JoystickButton1))
                    {
                        Button_B();
                    }


                    break;
                }
        }

    }
}
