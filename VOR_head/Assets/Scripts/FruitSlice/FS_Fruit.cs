using UnityEngine;

public class FS_Fruit : MonoBehaviour {

    public FS_FruitSpeedCal2 FSC2_script;
    [SerializeField] private FS_FruitRenderer FR_script;
    [SerializeField] private FS_StopIndicator SI_script;
    [SerializeField] private FS_FruitMesh FM_script;

    public bool Start_flag { get; set; }

    private Vector3 start_pos;
    private Vector3 stop_pos;

    // Use this for initialization
    void Awake () {
        this.Start_flag = false;
        this.stop_pos = new Vector3();
	}

    private void Start()
    {

    }

    // Update is called once per frame
    void Update () {

	}

    public void start_fruit()
    {
        Start_flag = true;
    }

    public void load_trial(Vector2[] poss, Texture2D texture2D)
    {
        FM_script.first_create_mesh(poss, texture2D);
    }

    //public void start_fruit(FS_RC _FSRC)
    //{
    //    Start_flag = true;
    //    FSRC = _FSRC;
    //}

    public void fruit_cutted()
    {
        if(Start_flag)
        {
            stop_pos = get_stop_pos();
            //FR_script.cut();
            SI_script.mark_cut(stop_pos);
            FM_script.cut_mseh(start_pos, stop_pos);
        }
    }

    private Vector3 get_stop_pos()
    {
        Vector3 hit_point = new Vector3();
        if (FS_RC.IS.RC_script.check_object_pos(FS_SD.FruitPlane_tag, out hit_point)) { return hit_point; }
        else { Debug.LogError("get_stop_pos RayCast error"); }
        return new Vector3();
    }

    public void set_start_pos(Vector3 _start_pos)
    {
        start_pos = _start_pos;
    }



}
