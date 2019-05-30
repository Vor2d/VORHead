using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralControllerInput : MonoBehaviour
{
    public enum FourDirInput { up, right, down, left, empty };
    public enum EightDirInput { up, right, down, left, upri, dori, dole, uple, empty };

    private const string Hori_axes1 = "Horizontal";
    private const string Vert_axes1 = "Vertical";
    private const string Vert_axes2 = "PS_Vertical";

    public enum ControllerType { Xbox,PS4};

    [SerializeField] ControllerType CurrContollerType;
    [SerializeField] float Sensitivity;

    public bool Forward_flag { get; private set; }
    public bool Back_flag { get; private set; }
    public bool Right_flag { get; private set; }
    public bool Left_flag { get; private set; }
    public System.Action Forward_act { get; set; }
    public System.Action Back_act { get; set; }
    public System.Action Right_act { get; set; }
    public System.Action Left_act { get; set; }
    public System.Action Button5_act { get; set; }

    private void Awake()
    {
        this.Forward_flag = false;
        this.Back_flag = false;
        this.Right_flag = false;
        this.Left_flag = false;
        this.Forward_act = null;
        this.Back_act = null;
        this.Right_act = null;
        this.Left_act = null;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        switch(CurrContollerType)
        {
            case ControllerType.Xbox:
                XboxController();
                break;

            case ControllerType.PS4:
                PS4Controller();
                break;
        }
    }

    private void PS4Controller()
    {
        if (Input.GetAxisRaw(Vert_axes2) < -Sensitivity)
        {
            if (!Forward_flag && Forward_act != null)
            {
                Forward_act();
            }
            Forward_flag = true;
        }
        else
        {
            Forward_flag = false;
        }
        if (Input.GetAxisRaw(Vert_axes2) > Sensitivity)
        {
            if (!Back_flag && Back_act != null)
            {
                Back_act();
            }
            Back_flag = true;
        }
        else
        {
            Back_flag = false;
        }
        if (Input.GetAxis(Hori_axes1) > Sensitivity)
        {
            if (!Right_flag && Right_act != null)
            {
                Right_act();
            }
            Right_flag = true;
        }
        else
        {
            Right_flag = false;
        }
        if (Input.GetAxis(Hori_axes1) < -Sensitivity)
        {
            if (!Left_flag && Left_act != null)
            {
                Left_act();
            }
            Left_flag = true;
        }
        else
        {
            Left_flag = false;
        }

        if (Input.GetKeyDown(KeyCode.JoystickButton5) && Button5_act != null)
        {
            Button5_act();
        }
    }

    private void XboxController()
    {
        if (Input.GetAxis(Vert_axes1) > Sensitivity)
        {
            if (!Forward_flag && Forward_act != null)
            {
                Forward_act();
            }
            Forward_flag = true;
        }
        else
        {
            Forward_flag = false;
        }
        if (Input.GetAxis(Vert_axes1) < -Sensitivity)
        {
            if (!Back_flag && Back_act != null)
            {
                Back_act();
            }
            Back_flag = true;
        }
        else
        {
            Back_flag = false;
        }
        if (Input.GetAxis(Hori_axes1) > Sensitivity)
        {
            if (!Right_flag && Right_act != null)
            {
                Right_act();
            }
            Right_flag = true;
        }
        else
        {
            Right_flag = false;
        }
        if (Input.GetAxis(Hori_axes1) < -Sensitivity)
        {
            if (!Left_flag && Left_act != null)
            {
                Left_act();
            }
            Left_flag = true;
        }
        else
        {
            Left_flag = false;
        }

        if (Input.GetKeyDown(KeyCode.JoystickButton5) && Button5_act != null)
        {
            Button5_act();
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
