using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WAMEC;

public class WAM_MoleCenter : MonoBehaviour
{
    private WAMRC RC;

    private MoleGenerShape gener_shape;
    private int mole_frame_num;
    private float distant;

    private void Awake()
    {
        this.RC = null;
        this.gener_shape = MoleGenerShape.circle;
        this.mole_frame_num = 1;
        this.distant = 1.0f;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void init_mole_center(WAMRC _RC,MoleGenerShape _gener_shape, float _distant,int _mole_frame_num)
    {
        RC = _RC;
        gener_shape = _gener_shape;
        mole_frame_num = _mole_frame_num;
        distant = _distant;
    }

    public void generate_mole_frame()
    {
        switch(gener_shape)
        {
            case MoleGenerShape.circle:
                generate_circle();
                break;
        }
    }

    private void generate_circle()
    {
        float degree = 0.0f;
        float degree_incr = 360.0f / mole_frame_num;
        float x = 0.0f;
        float y = 0.0f;
        while(degree < 360.0f)
        {
            x = distant * Mathf.Cos(Mathf.PI * degree / 180.0f);
            y = distant * Mathf.Sin(Mathf.PI * degree / 180.0f);
            spawn_mole_frame(new Vector3(x, y, transform.position.z));
            degree += degree_incr;
        }
    }

    private void spawn_mole_frame(Vector3 pos)
    {
        Transform frame_TRANS = Instantiate(RC.MoleFrame_Prefab, pos, Quaternion.identity).transform;
    }
}
