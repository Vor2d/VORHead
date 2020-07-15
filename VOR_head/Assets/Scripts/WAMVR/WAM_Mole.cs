using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using UnityEngine;
using WAMEC;

public class WAM_Mole : MonoBehaviour
{
    [SerializeField] private Transform Collider_TRANS;
    [SerializeField] private Transform Mesh_TRANS;
    [SerializeField] private Transform AcuityMesh_TRANS;
    [SerializeField] private Transform ControllerMesh_TRANS;
    [SerializeField] private Transform RedXMesh_TRANS;
    [SerializeField] private GeneralSoundCoroutine ErrorSFX_GSC;
    [SerializeField] private GeneralSoundCoroutine CorrectSFX_GSC;
    [SerializeField] private GeneralSoundCoroutine SpawnSFX_GSC;
    [SerializeField] private Transform ACBG_TRANS;

    [SerializeField] private float AcuityOffSet;
    [SerializeField] private float ControllerMOffSet;
    [SerializeField] private float AfterWhacTime;
    [SerializeField] private bool Use_Sprite_AC;

    private WAM_MoleCenter MC_script;
    private Transform NITRANS_Cache;

    public bool aimming_flag { get; private set; }
    private bool start_flag;
    private WAM_RayCast RCT_cache;
    private Color init_color;
    private float timer;
    private int direction;
    private int last_Cdirection;
    private bool collider_checked_flag;
    private bool whac_ani_flag;
    private bool fish_move_flag;
    private float turning_timer;
    private bool turning_flag;

    private void Awake()
    {
        this.MC_script = null;
        this.RCT_cache = null;
        this.start_flag = false;
        this.aimming_flag = false;
        this.timer = 0.0f;
        this.direction = 0;
        this.last_Cdirection = 0;
        this.collider_checked_flag = false;
        this.NITRANS_Cache = null;
        this.whac_ani_flag = false;
        this.init_color = Color.white;
        this.fish_move_flag = false;
        this.turning_timer = 0.0f;
        this.turning_flag = false;
    }

    private void Start()
    {
        NITRANS_Cache = WAMRC.IS.Fishnet_TRANS.GetComponent<WAM_Fishnet>().NetIn_TRANS;
        //init_color = Mesh_TRANS.GetComponent<MeshRenderer>().material.color;
    }

    // Update is called once per frame
    void Update()
    {
        if (start_flag)
        {
            check_timer();
            check_aim();
            update_TT();
        }
    }

    private void update_TT()
    {
        if (turning_flag) { turning_timer += Time.deltaTime; }
    }

    public void init_mole(WAM_MoleCenter _MC_script, Transform self_mesh = null,Sprite AC_sprite = null)
    {
        MC_script = _MC_script;
        timer = WAMSetting.IS.Mole_des_time;
        RCT_cache = WAMRC.IS.RCT_script;
        last_Cdirection = -1;
        if(self_mesh != null) { Mesh_TRANS = self_mesh; }
        if (Use_Sprite_AC) { AcuityMesh_TRANS.GetComponent<SpriteRenderer>().sprite = AC_sprite; }
    }

    public void start_mole()
    {
        start_flag = true;
        turning_flag = true;
        SpawnSFX_GSC.start_coroutine();
    }

    private void check_timer()
    {
        timer -= Time.deltaTime;
        if(timer < 0)
        {
            timer = float.MaxValue;
            self_destroy();
        }
    }


    private void check_aim()
    {
        if(RCT_cache.check_object(WAMSD.Mole_tag, Collider_TRANS))
        {
            to_aim_state();
        }
        else
        {
            to_unaim_state();
        }
    }

    private void to_aim_state()
    {
        if(!aimming_flag)
        {
            aimming_flag = true;
            if(WAMSetting.IS.Stop_on_bubble)
            {
                WAM_GameController.IS.Check_stop_instance++;
            }
            change_mesh(true);
            stop_TT();
        }
        collider_reached();
    }

    private void stop_TT()
    {
        turning_flag = false;
    }


    private void change_mesh(bool aimed)
    {
        if (WAM_GameController.IS.Use_self_mesh)
        {

        }
        else
        {
            if (aimed) { Mesh_TRANS.GetComponent<MeshRenderer>().material.color = Color.red; }
            else { Mesh_TRANS.GetComponent<MeshRenderer>().material.color = init_color; }
        }
    }

    private void to_unaim_state()
    {
        if(aimming_flag)
        {
            aimming_flag = false;
            if (WAMSetting.IS.Stop_on_bubble)
            {
                WAM_GameController.IS.Check_stop_instance--;
            }
            change_mesh(false);
        }
        
    }

    public void whaced()
    {
        if(start_flag)
        {
            WAM_GameController.IS.success_whac(transform, direction,turning_time: turning_timer);
            //play_correct_sound();
            StartCoroutine(whac_anim(AfterWhacTime));
            start_flag = false;
        }
    }

    public void aimmed_acuity_whac(int dir)
    {
        if (start_flag)
        {
            if (aimming_flag)
            {
                if (dir == direction)
                {
                    whaced();
                }
                else
                {
                    wrong_whac();
                }
            }
        }
    }

    public void unaim_acuity_whac(int dir)
    {
        if (start_flag)
        {
            if (dir == direction)
            {
                whaced();
            }
            else
            {
                wrong_whac();
            }
        }
    }

    private void clean_destroy()
    {
        start_flag = false;
        if (WAMSetting.IS.Stop_on_bubble && aimming_flag)
        {
            WAM_GameController.IS.Check_stop_instance--;
        }
        MC_script.mole_TRANSs.Remove(transform);
        Destroy(gameObject);
    }

    public void generate_acuity(AcuityType A_type,float A_size, float A_time)
    {
        if(start_flag)
        {
            switch (A_type)
            {
                case AcuityType.fourdir:
                    direction = Random.Range(0, 4);
                    break;
                case AcuityType.eightdir:
                    direction = Random.Range(0, 8);
                    break;
            }
            rotate_acuity(A_type, direction);
            AcuityMesh_TRANS.localScale = new Vector3(A_size, A_size, A_size);
            StartCoroutine(flash_acuity(A_time));
        }

    }

    private IEnumerator flash_acuity(float time)
    {
        turn_on_ACmesh();
        yield return new WaitForSeconds(time);
        turn_off_ACmesh();
    }

    private void turn_on_ACmesh()
    {
        if (Use_Sprite_AC) 
        {
            AcuityMesh_TRANS.GetComponent<SpriteRenderer>().enabled = true; 
            ACBG_TRANS.GetComponent<SpriteRenderer>().enabled = true;
        }
        else { AcuityMesh_TRANS.GetComponent<MeshRenderer>().enabled = true; }
    }

    private void turn_off_ACmesh()
    {
        if (Use_Sprite_AC) 
        {
            AcuityMesh_TRANS.GetComponent<SpriteRenderer>().enabled = false;
            ACBG_TRANS.GetComponent<SpriteRenderer>().enabled = false;
        }
        else { AcuityMesh_TRANS.GetComponent<MeshRenderer>().enabled = false; }
    }

    private void rotate_acuity(AcuityType A_type, int dir)
    {
        switch (A_type)
        {
            case AcuityType.fourdir:
                AcuityMesh_TRANS.rotation =
                    Quaternion.Euler(new Vector3(0.0f, 0.0f, dir * -90.0f + AcuityOffSet));
                break;
            case AcuityType.eightdir:
                if (dir < 4)
                {
                    AcuityMesh_TRANS.rotation =
                        Quaternion.Euler(new Vector3(0.0f, 0.0f, dir * -90.0f + AcuityOffSet));
                }
                else
                {
                    AcuityMesh_TRANS.rotation =
                        Quaternion.Euler(new Vector3(0.0f, 0.0f, (dir - 4) * -90.0f + AcuityOffSet - 45.0f));
                }
                break;
        }
    }

    public void choose_acuity(int C_direction)
    {
        if(start_flag)
        {
            if (aimming_flag && (WAMSetting.IS.Controller_mode == ControllerModes.post_judge
                || WAMSetting.IS.Controller_mode == ControllerModes.TD_PJ))
            {
                change_Cmesh(C_direction);
                last_Cdirection = C_direction;
            }
        }
    }
    
    public void change_Cmesh(int C_direction)
    {
        if(start_flag)
        {
            if (!check_Cmesh()) { turn_on_Cmesh(); }

            if (last_Cdirection != C_direction)
            {
                rotate_Cmesh(WAMSetting.IS.Acuity_type, C_direction);
            }
        }
    }

    private bool check_Cmesh()
    {
        if (Use_Sprite_AC) { return ControllerMesh_TRANS.GetComponent<SpriteRenderer>().enabled; }
        else { return ControllerMesh_TRANS.GetComponent<MeshRenderer>().enabled; }
    }

    private void turn_on_Cmesh()
    {
        if (Use_Sprite_AC) { ControllerMesh_TRANS.GetComponent<SpriteRenderer>().enabled = true; }
        else { ControllerMesh_TRANS.GetComponent<MeshRenderer>().enabled = true; }
    }

    private void turn_off_Cmesh()
    {
        if (Use_Sprite_AC) { ControllerMesh_TRANS.GetComponent<SpriteRenderer>().enabled = false; }
        else { ControllerMesh_TRANS.GetComponent<MeshRenderer>().enabled = false; }
    }

    private void rotate_Cmesh(AcuityType A_type,int dir)
    {
        switch (A_type)
        {
            case AcuityType.fourdir:
                ControllerMesh_TRANS.rotation =
                    Quaternion.Euler(new Vector3(0.0f, 0.0f, dir * -90.0f + ControllerMOffSet));
                break;
            case AcuityType.eightdir:
                if (dir < 4)
                {
                    ControllerMesh_TRANS.rotation =
                        Quaternion.Euler(new Vector3(0.0f, 0.0f, dir * -90.0f + ControllerMOffSet));
                }
                else
                {
                    ControllerMesh_TRANS.rotation =
                        Quaternion.Euler(new Vector3(0.0f, 0.0f, (dir - 4) * -90.0f + ControllerMOffSet - 45.0f));
                }
                break;
        }
    }

    private IEnumerator whac_anim(float time = 0.0f)
    {
        //turn_off_mesh();
        //WAMRC.IS.WhacPartical_TRANS.position = transform.position;
        //WAMRC.IS.WhacPartical_TRANS.GetComponent<ParticleSystem>().Play();
        //yield return new WaitForSeconds(time);
        whac_ani_flag = true;
        float last_dist = float.MaxValue;
        while(whac_ani_flag)
        {
            if (!fish_move_flag) { fish_move_flag = fish_move_check(ref last_dist); }
            else { transform.position = NITRANS_Cache.position; }
            yield return null;
        }
        clean_destroy();
    }

    private bool fish_move_check(ref float last_dist)
    {
        float dist = Vector3.Distance(NITRANS_Cache.position, transform.position);
        if (dist > last_dist) { return true; }
        last_dist = dist;
        return false;
    }

    private void self_destroy()
    {
        if(WAMSetting.IS.Controller_mode == ControllerModes.post_judge
            || WAMSetting.IS.Controller_mode == ControllerModes.TD_PJ)
        {
            unaim_acuity_whac(last_Cdirection);
        }
        else
        {
            wrong_whac();
        }
    }

    public void wrong_whac()
    {
        if(start_flag)
        {
            play_error_sound();
            StartCoroutine(wrong_whac_anim(AfterWhacTime));
            start_flag = false;
        }
        
    }

    private IEnumerator wrong_whac_anim(float time)
    {
        turn_off_mesh();
        RedXMesh_TRANS.GetComponent<MeshRenderer>().enabled = true;
        Debug.Log("Wrong whac");
        yield return new WaitForSeconds(time);
        RedXMesh_TRANS.GetComponent<MeshRenderer>().enabled = false;
        clean_destroy();
    }

    private void turn_off_mesh()
    {
        if (WAM_GameController.IS.Use_self_mesh) { Mesh_TRANS.GetComponent<SpriteRenderer>().enabled = false; }
        else { Mesh_TRANS.GetComponent<MeshRenderer>().enabled = false; }
        Collider_TRANS.GetComponent<Collider>().enabled = false;
        turn_off_ACmesh();
        turn_off_Cmesh();
    }

    private void collider_reached()
    {
        if(!collider_checked_flag)
        {
            WAM_GameController.IS.mole_reached();
            collider_checked_flag = true;
        }
    }

    private void play_error_sound()
    {
        ErrorSFX_GSC.start_coroutine();
    }

    private void play_correct_sound()
    {
        CorrectSFX_GSC.start_coroutine();
    }

    public void finish_mole()
    {
        whac_ani_flag = false;
    }
}
