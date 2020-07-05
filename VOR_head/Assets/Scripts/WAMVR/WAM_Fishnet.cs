using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WAMEC;

public class WAM_Fishnet : MonoBehaviour
{
    [SerializeField] private Transform NetIndi_TRANS;
    [SerializeField] private Transform Mesh_TRANS;
    [SerializeField] private float Rotation_deg;
    [SerializeField] private float Rotation_time;

    public Transform NetIn_TRANS { get { return NetIndi_TRANS; } }
    private float net_dist;

    private void Awake()
    {
        this.net_dist = 0.0f;
    }

    // Start is called before the first frame update
    void Start()
    {
        net_dist = Vector3.Distance(NetIndi_TRANS.position,transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void init()
    {
        
    }

    public void start_net(Transform MT,int dir)
    {
        turn_on_mesh();
        transform.position = pos_cal_four_dir(MT.position, dir);
        transform.eulerAngles = rotation_cal(dir);
        StartCoroutine(rotate_cor(MT.GetComponent<WAM_Mole>(), Rotation_deg, Rotation_time));
    }

    private void turn_on_mesh()
    {

    }

    private void turn_off_mesh()
    {

    }

    private Vector3 pos_cal_four_dir(Vector3 Mpos,int dir)
    {
        switch(dir)
        {
            case 0:
                return new Vector3(Mpos.x - net_dist, Mpos.y, Mpos.z);
            case 1:
                return new Vector3(Mpos.x, Mpos.y + net_dist, Mpos.z);
            case 2:
                return new Vector3(Mpos.x + net_dist, Mpos.y, Mpos.z);
            case 3:
                return new Vector3(Mpos.x, Mpos.y - net_dist, Mpos.z);
        }
        return Vector3.zero;
    }

    private Vector3 rotation_cal(int dir)
    {
        float half_deg = Rotation_deg / 2.0f;
        switch (dir)
        {
            case 0:
                return new Vector3(0.0f, 0.0f, -(90 +half_deg));
            case 1:
                return new Vector3(0.0f, 0.0f, (90 + half_deg));
            case 2:
                return new Vector3(0.0f, 0.0f, half_deg);
            case 3:
                return new Vector3(0.0f, 0.0f, -half_deg);
        }
        return Vector3.zero;
    }

    private IEnumerator rotate_cor(WAM_Mole M_script, float deg,float time)
    {
        float speed = deg / time;
        Vector3 rot_dir = new Vector3(0.0f, 0.0f, speed);
        while(time >= 0)
        {
            transform.Rotate(rot_dir * Time.deltaTime);
            time -= Time.deltaTime;
            yield return null;
        }
        turn_off_mesh();
        M_script.finish_mole();
    }

}
