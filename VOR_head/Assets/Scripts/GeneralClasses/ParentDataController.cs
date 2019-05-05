using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParentDataController : MonoBehaviour {

    public bool using_VR { get; set; }
    public bool using_coil { get; set; }

    public MySceneManager MSM_script { get; set; }

    virtual public void init_DC()
    {
        GameObject temp_SM_OBJ = GameObject.Find(GeneralStrDefiner.SceneManagerGO_name);
        if(temp_SM_OBJ != null)
        {
            MSM_script = temp_SM_OBJ.GetComponent<MySceneManager>();
            this.using_VR = MSM_script.using_VR;
            this.using_coil = MSM_script.using_coil;
        }
    }
}
