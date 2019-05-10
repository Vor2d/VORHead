using UnityEngine;

public class FS_Fruit : MonoBehaviour {

    public FS_RC FSRC;
    public FS_FruitSpeedCal2 FSC2_script;
    [SerializeField] private FS_FruitRenderer FR_script;
    [SerializeField] private FS_StopIndicator SI_script;

    public bool Start_flag { get; set; }

    // Use this for initialization
    void Awake () {
        this.Start_flag = false;
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

    public void start_fruit(FS_RC _FSRC)
    {
        Start_flag = true;
        FSRC = _FSRC;
    }

    public void fruit_cutted()
    {
        if(Start_flag)
        {
            FR_script.cut();
            SI_script.mark_cut();
        }
    }


}
