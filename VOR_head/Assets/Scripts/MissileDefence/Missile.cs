using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour {

    [SerializeField] private GameObject Explosion_Prefab;
    [SerializeField] private float SpeedRandomRange = 0.0f;

    private MD_GameController MDGC_script;

    private bool start_flag;
    private Vector3 target_pos;
    private float init_speed;

    //Need to init objects here since it is a prefab;
    private void Awake()
    {
        this.MDGC_script =
            GameObject.Find("MD_GameController").GetComponent<MD_GameController>();

        this.target_pos = new Vector3();
        this.start_flag = false;
        this.init_speed = 0.0f;

    }

    // Use this for initialization
    void Start () {
        //init_speed += Random.Range(-SpeedRandomRange,SpeedRandomRange);

    }
	
	// Update is called once per frame
	void FixedUpdate () {
        if (start_flag)
        {
            transform.Translate(target_pos * GeneralGameController.GameDeltaTime * init_speed, 
                                Space.World);
        }
    }

    public void set_target(Transform tar_transform)
    {
        target_pos = tar_transform.position - transform.position;
    }

    public void start_move()
    {
        face(target_pos);
        start_flag = true;
    }

    public void start_move(float speed)
    {
        set_speed(speed);
        face(target_pos);
        start_flag = true;
    }

    private void set_speed(float speed)
    {
        init_speed = speed;
    }

    public void face(Vector3 pos)
    {
        transform.forward = pos;
    }

    private void OnTriggerEnter(Collider other)
    {
        GameObject other_GO = other.transform.gameObject;
        if (other_GO.tag == "City")
        {
            other_GO.GetComponent<City>().get_hit();
            Instantiate(Explosion_Prefab, transform.position,new Quaternion());
            //MDGC_script.City_hitted();
            Destroy(gameObject);
        }
        else if(other_GO.tag == "Explode")
        {
            MDGC_script.missile_destroyed();
            other_GO.GetComponent<Explode>().missile_hitted();
            Instantiate(Explosion_Prefab, transform.position, new Quaternion());
            Destroy(gameObject);
        }
        else if(other_GO.tag == "MD_GroundBorder")
        {
            Instantiate(Explosion_Prefab, transform.position, new Quaternion());
            Destroy(gameObject);
        }
        //StartCoroutine(MDGC_script.check_missile_number());
        MDGC_script.check_missile_number();

    }
}
