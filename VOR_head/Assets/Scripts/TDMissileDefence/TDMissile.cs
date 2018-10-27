using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TDMissile : MonoBehaviour
{
    //public Transform PlayerTransform;
    public Transform AimITransform;

    public float Speed = 0.5f;

    public bool Aimed_flag { get; set; }

    private bool start_flag;
    private Vector3 target_pos;
    private bool AimI_mesh_flag;

    private TDMD_GameController TDMDGC_script;

    //Need to init objects here since it is a prefab;
    private void Awake()
    {
        this.target_pos = new Vector3();
        this.start_flag = false;
        this.AimI_mesh_flag = false;
        this.TDMDGC_script = 
            GameObject.Find("TDMD_GameController").GetComponent<TDMD_GameController>();
    }

    // Use this for initialization
    void Start()
    {

    }

    private void LateUpdate()
    {
        turn_off_AimI();

        if(Aimed_flag)
        {
            turn_on_AimI();
            Aimed_flag = false;

            if(TDMDGC_script.Fire_flag)
            {
                being_fired();
            }
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (start_flag)
        {
            transform.Translate(target_pos * Time.deltaTime * Speed, Space.World);
        }
    }

    public void set_target(Transform tar_transform)
    {
        target_pos = tar_transform.transform.position - transform.position;
    }

    public void start_move()
    {
        face(target_pos);
        start_flag = true;
    }

    public void face(Vector3 pos)
    {
        transform.up = -pos;
    }

    private void OnTriggerEnter(Collider other)
    {
        GameObject other_GO = other.transform.gameObject;
        if (other_GO.tag == "FrontBorder")
        {
            other_GO.GetComponent<FrontBorder>().get_hit();
            Destroy(gameObject);
        }
    }

    private void turn_on_AimI()
    {
        //Transform AimI_transform = transform.Find("AimIndicator");
        if (!AimI_mesh_flag && AimITransform != null)
        {
            AimITransform.gameObject.GetComponent<AimInticator>().turn_on_mesh();
            AimI_mesh_flag = true;
        }
    }

    private void turn_off_AimI()
    {
        //Transform AimI_transform = transform.Find("AimIndicator");
        if (AimI_mesh_flag && AimITransform != null)
        {
            AimITransform.gameObject.GetComponent<AimInticator>().turn_off_mesh();
            AimI_mesh_flag = false;
        }
    }

    private void being_fired()
    {
        TDMDGC_script.missile_destroyed();
        Destroy(gameObject);
    }

}
