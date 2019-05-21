using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AcuityGroup : MonoBehaviour
{
    private const string path = "Sprites/Acuity/Transparant Cs/";
    private readonly Quaternion default_quat = new Quaternion(1.0f, 1.0f, 1.0f, 1.0f);

    public enum AcuityDirections { up,right,down,left,upri,dori,dole,uple };

    [SerializeField] private Canvas canvas;
    [SerializeField] private Camera acu_camera;
    [SerializeField] private Transform Target_TRANS;
    [SerializeField] private Transform Background_TRANS;
    [SerializeField] private Transform AcuitySprite_TRANS;
    [SerializeField] private Transform AcuityIndi_TRANS;
    [SerializeField] private Controller_Input CI_script;

    private bool AI_start_flag;
    private GameController.AcuityMode acuity_mode;

    private void Awake()
    {
        this.AI_start_flag = false;
        this.acuity_mode = default(GameController.AcuityMode);
    }

    // Start is called before the first frame update
    void Start()
    {
        turn_off_AG();
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("default_quat " + default_quat.eulerAngles);

        update_acuity_pos();

        if(AI_start_flag)
        {
            acuity_indicator();
        }
    }

    private void update_acuity_pos()
    {
        //Debug.Log("WorldToScreenPoint " + acu_camera.WorldToScreenPoint(Target_TRANS.position));
        Vector3 pos =
                GeneralMethods.world_to_canvas(Target_TRANS.position, acu_camera, canvas);
        GetComponent<RectTransform>().localPosition = pos;
    }

    public void init_acuity(int acuity_size,GameController.AcuityMode _acuity_mode)
    {
        AcuitySprite_TRANS.GetComponent<SpriteRenderer>().sprite = 
            Resources.Load<Sprite>(path + acuity_size.ToString());
        acuity_mode = _acuity_mode;
    }

    public void turn_off_AG()
    {
        turn_off_AI();
        turn_off_AS();
        //gameObject.SetActive(false);
        turn_off_BG();

    }

    public void turn_on_AG()
    {
        //gameObject.SetActive(true);
        turn_on_BG();
    }

    private void turn_off_BG()
    {
        Background_TRANS.gameObject.SetActive(false);
    }

    private void turn_on_BG()
    {
        Background_TRANS.gameObject.SetActive(true);
    }

    public AcuityDirections turn_on_acuity(bool random_rotate)
    {
        AcuityDirections dir = AcuityDirections.up;
        if (random_rotate)
        {
            dir = rotate();
        }
        //gameObject.SetActive(true);
        turn_on_BG();
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

    private AcuityDirections rotate()
    {
        int random_dir = 0;
        switch (acuity_mode)
        {
            case GameController.AcuityMode.four_dir:
                random_dir = UnityEngine.Random.Range(0, 4);
                break;
            case GameController.AcuityMode.eight_dir:
                random_dir = UnityEngine.Random.Range(0, 8);
                break;
        }
        AcuitySprite_TRANS.rotation = rotate_cal(random_dir);
        return (AcuityDirections)random_dir;
    }

    private Quaternion rotate_cal(int ADir)
    {
        switch(acuity_mode)
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
        //gameObject.SetActive(true);
        turn_on_BG();
        //AcuityIndi_TRANS.gameObject.SetActive(true);
        AI_start_flag = true;
    }

    private void acuity_indicator()
    {
        Quaternion rotate_caled = new Quaternion();
        switch(acuity_mode)
        {
            case GameController.AcuityMode.four_dir:
                rotate_caled = rotate_cal((int)CI_script.Four_dir_input);
                break;
            case GameController.AcuityMode.eight_dir:
                rotate_caled = rotate_cal((int)CI_script.Eight_dir_input);
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
}
