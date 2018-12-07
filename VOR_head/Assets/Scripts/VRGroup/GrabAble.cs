using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabAble : MonoBehaviour {

    private Color init_color;

    virtual protected void init()
    {
        init_color = GetComponent<MeshRenderer>().material.color;
        Debug.Log("coloe " + init_color); 
    }

    virtual public void high_light()
    {
        GetComponent<MeshRenderer>().material.color = Color.red;
    }

    virtual public void de_high_light()
    {
        GetComponent<MeshRenderer>().material.color = init_color;
    }

    
}
