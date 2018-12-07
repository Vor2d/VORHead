using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FS_FruitIndicator : MonoBehaviour {

    [SerializeField] private FS_Fruit FSF_script;
    [SerializeField] private Transform StartI_TRANS;
    [SerializeField] private Transform EndI_TRANS;
    [SerializeField] private Transform FruitLineR_TRANS;

    private bool start_color_triggerred;
    private Color start_color;

    // Use this for initialization
    void Start () {

        this.start_color = StartI_TRANS.GetComponent<MeshRenderer>().material.color;
        this.start_color_triggerred = false;

        set_line_positions();

	}
	
	// Update is called once per frame
	void Update () {
        
        if(FSF_script.Aim_changed)
        {
            if(FSF_script.Is_aimed_flag)
            {
                set_start_focus_color();
            }
            else
            {
                set_start_unfocus_color();
            }
        }

	}

    private void set_line_positions()
    {
        Vector3[] positions = { StartI_TRANS.position, EndI_TRANS.position };
        FruitLineR_TRANS.GetComponent<LineRenderer>().SetPositions(positions);
    }

    private void set_start_focus_color()
    {
        Color trans_red = Color.red;
        trans_red.a = start_color.a;
        StartI_TRANS.GetComponent<MeshRenderer>().material.color = trans_red;
    }

    private void set_start_unfocus_color()
    {
        StartI_TRANS.GetComponent<MeshRenderer>().material.color = start_color;
    }

}
