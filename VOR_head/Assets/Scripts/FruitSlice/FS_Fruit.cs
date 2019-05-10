using UnityEngine;
using System;

public class FS_Fruit : MonoBehaviour {

    public bool Sliced_flag { get; set; }
    public bool Start_flag { get; set; }
    public bool Aim_changed { get; set; }

    [SerializeField] public FS_RC FSRC;

    private bool inner_sliced_flag;

    // Use this for initialization
    void Awake () {
        this.Start_flag = false;
	}

    private void Start()
    {
        this.Sliced_flag = false;
        this.inner_sliced_flag = false;

        start_fruit();
    }

    // Update is called once per frame
    void Update () {
		
        if(Start_flag)
        {

            if (inner_sliced_flag)
            {
                Sliced_flag = true;
                inner_sliced_flag = false;
            }
            else
            {
                Sliced_flag = false;
            }
        }

	}

    public void start_fruit()
    {
        Start_flag = true;
    }

    public void start_fruit(FS_RC _FSRC)
    {
        Start_flag = true;
        FSRC = _FSRC;
    }

    [Obsolete("Use fruit_cutted")]
    private void fruit_sliced()
    {
        Sliced_flag = true;
        FSRC.GC_script.fruit_destroyed();
        Destroy(gameObject);
    }

    public void fruit_cutted()
    {
        inner_sliced_flag = true;
    }

}
