using UnityEngine;

/// <summary>
/// Self implemented controller input script;
/// </summary>
public class Controller_Input : MonoBehaviour {

    public const string LeftIndexTrigger_str = "Oculus_CrossPlatform_PrimaryIndexTrigger";
    public const string RightIndexTrigger_str = "Oculus_CrossPlatform_SecondaryIndexTrigger";
    public const string RightVertical_str = 
                                        "Oculus_CrossPlatform_SecondaryThumbstickVertical";
    public const string RightHorizontal_str =
                                        "Oculus_CrossPlatform_SecondaryThumbstickHorizontal";

    public OVRInput.Controller Controller_type;
    [SerializeField] private float Sensitivity = 0.5f;

    public bool Forward_flag { get; set; }
    public bool Index_trigger_holding { get; set; }
    public bool Left_flag { get; set; }
    public bool Right_flag { get; set; }
    public bool Back_flag { get; set; }
    public System.Action Button_A { get; set; }
    public System.Action Button_B { get; set; }
    public System.Action IndexTrigger { get; set; }
    public System.Action ForwardAction { get; set; }
    public System.Action LeftAction { get; set; }
    public System.Action RightAction { get; set; }
    public System.Action BackAction { get; set; }

    private bool index_triggered_flag;

    private void Awake()
    {
        this.Button_A = null;
        this.Button_B = null;
        this.IndexTrigger = null;
        this.ForwardAction = null;
        this.LeftAction = null;
        this.RightAction = null;
        this.BackAction = null;
    }

    // Use this for initialization
    void Start () {
        this.Forward_flag = false;
        this.Back_flag = false;
        this.Right_flag = false;
        this.Left_flag = false;
        this.Index_trigger_holding = false;
        this.index_triggered_flag = false;

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
                    //R vertical thumb stick;
                    if (Input.GetAxis(RightVertical_str)> Sensitivity)
                    {
                        if(!Forward_flag && ForwardAction != null)
                        {
                            ForwardAction();
                        }
                        Forward_flag = true;
                    }
                    else
                    {
                        Forward_flag = false;
                    }
                    if(Input.GetAxis(RightVertical_str) < -Sensitivity)
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
                    if (Input.GetAxis(RightHorizontal_str) > Sensitivity)
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
                    if (Input.GetAxis(RightHorizontal_str) < -Sensitivity)
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

                    //R index trigger;
                    if (Input.GetAxis(RightIndexTrigger_str) > 0.5f)
                    {
                        Index_trigger_holding = true;
                        if(!index_triggered_flag && IndexTrigger != null)
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
                    if(Input.GetKeyDown(KeyCode.JoystickButton1) && Button_B != null)
                    {
                        Button_B();
                    }
                    if (Input.GetKeyDown(KeyCode.JoystickButton0) && Button_A != null)
                    {
                        Button_A();
                    }


                    break;
                }
        }

    }
}