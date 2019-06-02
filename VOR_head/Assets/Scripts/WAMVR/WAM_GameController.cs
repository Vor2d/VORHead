using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WAMEC;

public class WAM_GameController : MonoBehaviour
{
    [SerializeField] private WAMRC RC;

    // Start is called before the first frame update
    void Start()
    {
        generate_mole_center();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void generate_mole_center()
    {
        GameObject mole_center_OBJ = 
                Instantiate(RC.MoleCenter_Prefab, RC.MoleCenterInidcator.position, Quaternion.identity);
        mole_center_OBJ.GetComponent<WAM_MoleCenter>().init_mole_center(RC, MoleGenerShape.circle, 3.0f, 6);
        mole_center_OBJ.GetComponent<WAM_MoleCenter>().generate_mole_frame();
    }
}
