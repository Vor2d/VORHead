using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParentDataController : MonoBehaviour {

    public bool using_VR { get; set; }
    public bool using_coil { get; set; }

    virtual public void init_DC()
    {
        GameObject temp_SM_OBJ = GameObject.Find(GeneralStrDefiner.SceneManagerGO_name);
        if(temp_SM_OBJ != null)
        {
            this.using_VR = temp_SM_OBJ.GetComponent<MySceneManager>().using_VR;
            this.using_coil = temp_SM_OBJ.GetComponent<MySceneManager>().using_coil;
        }
    }
}
