using Boo.Lang;
using System;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.XR;

/// <summary>
/// Self implemented controller input script;
/// </summary>
public class Controller_Input : MonoBehaviour {

    public enum FourDirInput { up,right,down,left,empty};
    public enum EightDirInput { up,right,down,left,upri,dori,dole,uple,empty};

    public const string LeftIndexTrigger_str = "Oculus_CrossPlatform_PrimaryIndexTrigger";
    public const string RightIndexTrigger_str = "Oculus_CrossPlatform_SecondaryIndexTrigger";
    public const string leftVertical_str =
                                    "Oculus_CrossPlatform_PrimaryThumbstickVertical";
    public const string LeftHorizontal_str =
                                        "Oculus_CrossPlatform_PrimaryThumbstickHorizontal";
    public const string RightVertical_str = 
                                        "Oculus_CrossPlatform_SecondaryThumbstickVertical";
    public const string RightHorizontal_str =
                                        "Oculus_CrossPlatform_SecondaryThumbstickHorizontal";

    public OVRInput.Controller Controller_type;
    [SerializeField] private float JoystickSensitivity = 0.5f;
    [SerializeField] private float TriggerSensitivity = 0.5f;
    [SerializeField] private bool UsingXR;
    [SerializeField] private XRNode Controller_node;

    public bool Forward_flag { get; set; }
    public bool Index_trigger_holding { get; set; }
    public bool Left_flag { get; set; }
    public bool Right_flag { get; set; }
    public bool Back_flag { get; set; }
    public bool Button_X_hold { get; set; }
    public bool Button_Y_hold { get; set; }
    public System.Action Button_X { get; set; }
    public System.Action Button_Y { get; set; }
    public System.Action IndexTrigger { get; set; }
    public System.Action ForwardAction { get; set; }
    public System.Action LeftAction { get; set; }
    public System.Action RightAction { get; set; }
    public System.Action BackAction { get; set; }

    private bool index_triggered_flag;
    private bool xbutton_flag;
    private bool ybutton_flag;
    private InputDevice device;

    private void Awake()
    {
        this.Forward_flag = false;
        this.Back_flag = false;
        this.Right_flag = false;
        this.Left_flag = false;
        this.Index_trigger_holding = false;
        this.index_triggered_flag = false;
        this.Button_X_hold = false;
        this.Button_Y_hold = false;
        this.Button_X = null;
        this.Button_Y = null;
        this.IndexTrigger = null;
        this.ForwardAction = null;
        this.LeftAction = null;
        this.RightAction = null;
        this.BackAction = null;
        this.xbutton_flag = false;
        this.ybutton_flag = false;
        this.device = new InputDevice();
    }

    // Use this for initialization
    void Start () 
    {
        if (UsingXR) { get_XR_device(); }
    }

    private void get_XR_device()
    {
        XRDeviceManager.InitXRDevice();
        device = XRDeviceManager.get_device(Controller_node);
    }
	
	// Update is called once per frame
	void Update () {
        if (UsingXR) { process_XR_controller(); }
        else { process_OVR_controller(); }


    }

    private void process_XR_controller()
    {
        float trigger_val = 0.0f;
        float JS_horri_val = 0.0f;
        float JS_vert_val = 0.0f;
        bool buttonx = false;
        bool buttony = false;
        trigger_val = XRDeviceManager.get_trigger(device);
        JS_horri_val = XRDeviceManager.get_JS_horri(device);
        JS_vert_val = XRDeviceManager.get_JS_vert(device);
        buttonx = XRDeviceManager.get_xbutton(device);
        buttony = XRDeviceManager.get_ybutton(device);

        process_trigger(trigger_val);
        process_JS(new Vector2(JS_horri_val, JS_vert_val));
        process_buttons((buttonx, buttony));
    }

    private void process_OVR_controller()
    {
        float trigger_val = 0.0f;
        float JS_horri_val = 0.0f;
        float JS_vert_val = 0.0f;
        bool buttonx = false;
        bool buttony = false;
        switch (Controller_type)
        {
            case OVRInput.Controller.LTouch:
                {
                    trigger_val = Input.GetAxis(LeftIndexTrigger_str);
                    JS_horri_val = Input.GetAxis(LeftHorizontal_str);
                    JS_vert_val = Input.GetAxis(leftVertical_str);
                    buttonx = Input.GetKeyDown(KeyCode.JoystickButton2);
                    buttony = Input.GetKeyDown(KeyCode.JoystickButton3);
                    break;
                }
            case OVRInput.Controller.RTouch:
                {
                    trigger_val = Input.GetAxis(RightIndexTrigger_str);
                    JS_horri_val = Input.GetAxis(RightHorizontal_str);
                    JS_vert_val = Input.GetAxis(RightVertical_str);
                    buttonx = Input.GetKeyDown(KeyCode.JoystickButton0);
                    buttony = Input.GetKeyDown(KeyCode.JoystickButton1);
                    break;
                }
        }
        process_trigger(trigger_val);
        process_JS(new Vector2(JS_horri_val, JS_vert_val));
        process_buttons((buttonx, buttony));
    }

    private void process_trigger(float val)
    {
        if (val > TriggerSensitivity)
        {
            Index_trigger_holding = true;
            if (!index_triggered_flag && IndexTrigger != null)
            {
                IndexTrigger();
                index_triggered_flag = true;
            }
        }
        else
        {
            index_triggered_flag = false;
            Index_trigger_holding = false;
        }
    }

    /// <summary>
    /// Jotstick;
    /// </summary>
    /// <param name="axis2d">Horizontal then vertical</param>
    private void process_JS(Vector2 axis2d)
    {
        process_JS_horri(axis2d.x);
        process_JS_vert(axis2d.y);
    }

    private void process_JS_horri(float val)
    {
        if (val > JoystickSensitivity)
        {
            if (!Right_flag && RightAction != null)
            {
                RightAction();
            }
            Right_flag = true;
        }
        else
        {
            Right_flag = false;
        }
        if (val < -JoystickSensitivity)
        {
            if (!Left_flag && LeftAction != null)
            {
                LeftAction();
            }
            Left_flag = true;
        }
        else
        {
            Left_flag = false;
        }
    }

    private void process_JS_vert(float val)
    {
        if (val > JoystickSensitivity)
        {
            if (!Forward_flag && ForwardAction != null)
            {
                ForwardAction();
            }
            Forward_flag = true;
        }
        else
        {
            Forward_flag = false;
        }
        if (val < -JoystickSensitivity)
        {
            if (!Back_flag && BackAction != null)
            {
                BackAction();
            }
            Back_flag = true;
        }
        else
        {
            Back_flag = false;
        }
    }

    /// <summary>
    /// Process buttons;
    /// </summary>
    /// <param name="but_bools">x button then y button</param>
    private void process_buttons((bool,bool) but_bools)
    {
        process_xbutton(but_bools.Item1);
        process_ybutton(but_bools.Item2);
    }

    private void process_xbutton(bool but_bool)
    {
        Button_X_hold = but_bool;
        if (but_bool && !xbutton_flag && Button_X != null)
        {
            Button_X();
        }
        xbutton_flag = but_bool;
    }

    private void process_ybutton(bool but_bool)
    {
        Button_Y_hold = but_bool;
        if (but_bool && !ybutton_flag && Button_Y != null)
        {
            Button_Y();
        }
        ybutton_flag = but_bool;
    }

    [Obsolete("Use process_OVR_controller")]
    private void process_OVR_controller1()
    {
        switch (Controller_type)
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
                        Index_trigger_holding = true;
                    }
                    else
                    {
                        Index_trigger_holding = false;
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
                    rightC_dir();

                    //R index trigger;
                    if (Input.GetAxis(RightIndexTrigger_str) > 0.5f)
                    {
                        Index_trigger_holding = true;
                        if (!index_triggered_flag && IndexTrigger != null)
                        {
                            IndexTrigger();
                            index_triggered_flag = true;
                        }
                    }
                    else
                    {
                        index_triggered_flag = false;
                        Index_trigger_holding = false;
                    }
                    //R B button;
                    if (Input.GetKeyDown(KeyCode.JoystickButton1) && Button_Y != null)
                    {
                        Button_Y();
                    }
                    if (Input.GetKeyDown(KeyCode.JoystickButton0) && Button_X != null)
                    {
                        Button_X();
                    }


                    break;
                }
        }
    }

    [Obsolete("Use process_JS()")]
    private void rightC_dir()
    {
        //R vertical thumb stick;
        if (Input.GetAxis(RightVertical_str) > JoystickSensitivity)
        {
            if (!Forward_flag && ForwardAction != null)
            {
                ForwardAction();
            }
            Forward_flag = true;
        }
        else
        {
            Forward_flag = false;
        }
        if (Input.GetAxis(RightVertical_str) < -JoystickSensitivity)
        {
            if (!Back_flag && BackAction != null)
            {
                BackAction();
            }
            Back_flag = true;
        }
        else
        {
            Back_flag = false;
        }
        if (Input.GetAxis(RightHorizontal_str) > JoystickSensitivity)
        {
            if (!Right_flag && RightAction != null)
            {
                RightAction();
            }
            Right_flag = true;
        }
        else
        {
            Right_flag = false;
        }
        if (Input.GetAxis(RightHorizontal_str) < -JoystickSensitivity)
        {
            if (!Left_flag && LeftAction != null)
            {
                LeftAction();
            }
            Left_flag = true;
        }
        else
        {
            Left_flag = false;
        }
    }

    public FourDirInput Four_dir_input
    {
        get
        {
            if (Forward_flag) { return FourDirInput.up; }
            if (Right_flag) { return FourDirInput.right; }
            if (Back_flag) { return FourDirInput.down; }
            if (Left_flag) { return FourDirInput.left; }
            return FourDirInput.empty;
        }
    }
    public EightDirInput Eight_dir_input
    {
        get
        {
            if (Forward_flag && Right_flag) { return EightDirInput.upri; }
            if (Back_flag && Right_flag) { return EightDirInput.dori; }
            if (Back_flag && Left_flag) { return EightDirInput.dole; }
            if (Forward_flag && Left_flag) { return EightDirInput.uple; }
            if (Forward_flag) { return EightDirInput.up; }
            if (Right_flag) { return EightDirInput.right; }
            if (Back_flag) { return EightDirInput.down; }
            if (Left_flag) { return EightDirInput.left; }
            return EightDirInput.empty;
        }
    }
}