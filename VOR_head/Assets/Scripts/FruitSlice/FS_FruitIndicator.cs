using UnityEngine;

public class FS_FruitIndicator : MonoBehaviour {

    [SerializeField] private FS_Fruit FSF_script;
    [SerializeField] private Transform FruitLineR_TRANS;

    [SerializeField] private Color FocusColor = Color.red;

    public bool Is_aimed_flag { get; set; }
    public bool Last_is_aimed_flag { get; set; }
    private Color start_color;

    // Use this for initialization
    void Start ()
    {
        this.start_color = GetComponent<MeshRenderer>().material.color;
        this.Is_aimed_flag = false;
        this.Last_is_aimed_flag = false;

        set_line_positions();
	}
	
	// Update is called once per frame
	void Update ()
    {
        if(FSF_script.Start_flag)
        {
            check_aim();
        }
	}

    private void check_aim()
    {
        Is_aimed_flag = FSF_script.FSRC.RC_script.check_object(FS_SD.FruitStartI_Tag);
        if(Is_aimed_flag != Last_is_aimed_flag)
        {
            aim_changed(Is_aimed_flag);
        }
        Last_is_aimed_flag = Is_aimed_flag;
    }

    private void aim_changed(bool aimmed)
    {
        if(aimmed)
        {
            set_start_focus_color();
        }
        else
        {
            set_start_unfocus_color();
        }
    }

    private void set_line_positions()
    {
        Vector3[] positions = { transform.position, EndI_TRANS.position };
        FruitLineR_TRANS.GetComponent<LineRenderer>().SetPositions(positions);
    }

    private void set_start_focus_color()
    {
        Color focus_color = FocusColor;
        focus_color.a = start_color.a;
        GetComponent<MeshRenderer>().material.color = focus_color;
    }

    private void set_start_unfocus_color()
    {
        GetComponent<MeshRenderer>().material.color = start_color;
    }

}
