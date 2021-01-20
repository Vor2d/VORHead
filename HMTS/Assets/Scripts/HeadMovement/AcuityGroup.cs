using UnityEngine;
using System.Linq;

public class AcuityGroup : MonoBehaviour
{
    
    protected readonly Quaternion default_quat = new Quaternion(1.0f, 1.0f, 1.0f, 1.0f);

    public enum AcuityDirections { up,right,down,left,upri,dori,dole,uple };

    [SerializeField] protected Canvas canvas;
    [SerializeField] protected Camera acu_camera;
    [SerializeField] protected Transform Target_TRANS;
    [SerializeField] protected Transform Background_TRANS;
    [SerializeField] protected Transform AcuitySprite_TRANS;
    [SerializeField] protected Transform AcuityIndi_TRANS;
    [SerializeField] protected Controller_Input CI_script;
    [SerializeField] protected GeneralControllerInput GCI_script;

    protected bool AI_start_flag;
    protected GameController.AcuityMode acuity_mode;
    protected DataController DC_script;
    protected Sprite right_sprite;
    protected Sprite upri_sprite;

    protected void Awake()
    {
        this.AI_start_flag = false;
        this.acuity_mode = default(GameController.AcuityMode);
        this.DC_script = null;
        this.right_sprite = null;
        this.upri_sprite = null;
    }

    // Start is called before the first frame update
    protected void Start()
    {
        turn_off_AG();
    }

    // Update is called once per frame
    protected void Update()
    {
        update_acuity_pos();

        if(AI_start_flag)
        {
            if(DC_script.MSM_script.using_VR)
            {
                acuity_indicator_VR();
            }
            if(DC_script.MSM_script.using_coil)
            {
                acuity_indicator();
            }
        }
    }

    protected void update_acuity_pos()
    {
        Vector3 pos =
                GeneralMethods.world_to_canvas(Target_TRANS.position, acu_camera, canvas);
        pos = Vector3Int.RoundToInt(pos);
        GetComponent<RectTransform>().localPosition = pos;
    }

    public void init_acuity(int acuity_size,GameController.AcuityMode _acuity_mode,
                            DataController _DC_script)
    {
        DC_script = _DC_script;
        acuity_mode = _acuity_mode;

        change_acuity_size(acuity_size);
    }

    public void change_acuity_size(int size)
    {
        right_sprite = DC_script.Acuity_sprites.Single(s => s.name == size.ToString());
        upri_sprite = DC_script.Acuity_sprites.Single(s => s.name == (size+100).ToString());
    }

    public void turn_off_AG()
    {
        turn_off_AI();
        turn_off_AS();
        turn_off_BG();
    }

    public void turn_on_AG()
    {
        turn_on_BG();
    }

    protected void turn_off_BG()
    {
        Background_TRANS.gameObject.SetActive(false);
    }

    protected void turn_on_BG()
    {
        Background_TRANS.gameObject.SetActive(true);
    }

    public virtual AcuityDirections turn_on_acuity(bool random_rotate,
                        AcuityDirections def_dir = AcuityDirections.up)
    {
        AcuityDirections dir = AcuityDirections.up;
        if (random_rotate)
        {
            dir = rotate(true);
        }
        else
        {
            dir = rotate(false, def_dir: def_dir);
        }
        turn_on_BG();
        try
        {
            NetReciever.IS.WriteToArduino("Hi");
        }
        catch (System.Exception e) { Debug.Log(e); }
        AcuitySprite_TRANS.gameObject.SetActive(true);
        return dir;
    }

    public void turn_off_AS()
    {
        AcuitySprite_TRANS.gameObject.SetActive(false);
    }

    public void turn_off_AI()
    {
        AI_start_flag = false;
        AcuityIndi_TRANS.gameObject.SetActive(false);
    }

    protected virtual AcuityDirections rotate(bool random, AcuityDirections def_dir = AcuityDirections.up)
    {
        int random_dir = 0;
        if (random)
        {
            switch (acuity_mode)
            {
                case GameController.AcuityMode.four_dir:
                    random_dir = UnityEngine.Random.Range(0, 4);
                    break;
                case GameController.AcuityMode.eight_dir:
                    random_dir = UnityEngine.Random.Range(0, 8);
                    break;
            }
        }
        else { random_dir = (int)def_dir; }
        AcuitySprite_TRANS.localEulerAngles = Vector3Int.RoundToInt(rotate_cal_euler(random_dir));
        return (AcuityDirections)random_dir;
    }

    protected Quaternion rotate_cal(int ADir)
    {
        switch(acuity_mode)
        {
            case GameController.AcuityMode.four_dir:
                if (ADir < 4)
                {
                    AcuitySprite_TRANS.GetComponent<SpriteRenderer>().sprite = right_sprite;
                    return Quaternion.Euler(new Vector3(0.0f, 0.0f, -(ADir * 90.0f) + 90.0f));
                }
                else
                {
                    return default_quat;
                }
            case GameController.AcuityMode.eight_dir:
                if (ADir < 4)
                {
                    AcuitySprite_TRANS.GetComponent<SpriteRenderer>().sprite = right_sprite;
                    return Quaternion.Euler(new Vector3(0.0f, 0.0f, -(ADir * 90.0f) + 90.0f));
                }
                else if (ADir < 8)
                {
                    Debug.Log("dir " + ADir);
                    AcuitySprite_TRANS.GetComponent<SpriteRenderer>().sprite = upri_sprite;
                    return Quaternion.Euler(new Vector3(0.0f, 0.0f, -((ADir - 4) * 90.0f)));
                }
                else
                {
                    return default_quat;
                }
        }

        return Quaternion.identity;
    }

    protected Vector3 rotate_cal_euler(int ADir)
    {
        switch (acuity_mode)
        {
            case GameController.AcuityMode.four_dir:
                if (ADir < 4)
                {
                    AcuitySprite_TRANS.GetComponent<SpriteRenderer>().sprite = right_sprite;
                    return new Vector3(0.0f, 0.0f, -(ADir * 90.0f) + 90.0f);
                }
                else
                {
                    return default_quat.eulerAngles;
                }
            case GameController.AcuityMode.eight_dir:
                if (ADir < 4)
                {
                    AcuitySprite_TRANS.GetComponent<SpriteRenderer>().sprite = right_sprite;
                    return new Vector3(0.0f, 0.0f, -(ADir * 90.0f) + 90.0f);
                }
                else if (ADir < 8)
                {
                    Debug.Log("dir " + ADir);
                    AcuitySprite_TRANS.GetComponent<SpriteRenderer>().sprite = upri_sprite;
                    return new Vector3(0.0f, 0.0f, -((ADir - 4) * 90.0f));
                }
                else
                {
                    return default_quat.eulerAngles;
                }
        }

        return Vector3.zero;
    }

    protected Quaternion indicator_rotate_cal(int ADir)
    {
        switch (acuity_mode)
        {
            case GameController.AcuityMode.four_dir:
                if (ADir < 4)
                {
                    return Quaternion.Euler(new Vector3(0.0f, 0.0f, -(ADir * 90.0f) + 90.0f));
                }
                else
                {
                    return default_quat;
                }
            case GameController.AcuityMode.eight_dir:
                if (ADir < 4)
                {
                    return Quaternion.Euler(new Vector3(0.0f, 0.0f, -(ADir * 90.0f) + 90.0f));
                }
                else if (ADir < 8)
                {
                    return Quaternion.Euler(new Vector3(0.0f, 0.0f, -((ADir - 4) * 90.0f) + 45.0f));
                }
                else
                {
                    return default_quat;
                }
        }

        return Quaternion.identity;
    }

    public void start_AI()
    {
        turn_on_BG();
        AI_start_flag = true;
    }

    protected void acuity_indicator_VR()
    {
        Quaternion rotate_caled = new Quaternion();
        switch(acuity_mode)
        {
            case GameController.AcuityMode.four_dir:
                rotate_caled = indicator_rotate_cal((int)CI_script.Four_dir_input);
                break;
            case GameController.AcuityMode.eight_dir:
                rotate_caled = indicator_rotate_cal((int)CI_script.Eight_dir_input);
                break;
        }
        if(rotate_caled.eulerAngles == default_quat.eulerAngles)
        {
            AcuityIndi_TRANS.gameObject.SetActive(false);
        }
        else
        {
            AcuityIndi_TRANS.gameObject.SetActive(true);
            AcuityIndi_TRANS.rotation = rotate_caled;
        }
        
    }

    protected void acuity_indicator()
    {
        Quaternion rotate_caled = new Quaternion();
        switch (acuity_mode)
        {
            case GameController.AcuityMode.four_dir:
                rotate_caled = indicator_rotate_cal((int)GCI_script.Four_dir_input);
                break;
            case GameController.AcuityMode.eight_dir:
                rotate_caled = indicator_rotate_cal((int)GCI_script.Eight_dir_input);
                break;
        }
        if (rotate_caled.eulerAngles == default_quat.eulerAngles)
        {
            AcuityIndi_TRANS.gameObject.SetActive(false);
        }
        else
        {
            AcuityIndi_TRANS.gameObject.SetActive(true);
            AcuityIndi_TRANS.localRotation = rotate_caled;
        }

    }
}
