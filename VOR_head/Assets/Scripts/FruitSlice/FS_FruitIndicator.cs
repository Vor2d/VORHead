using UnityEngine;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public class FS_FruitIndicator : MonoBehaviour {

    public FS_Fruit F_script;
    public Color ActivateColor = Color.green;
    public Color FocusColor = Color.red;
    public Color DeActivatedColor = Color.white;
    [SerializeField] private Transform FruitLineR_TRANS;
    //[SerializeField] private GameObject Indicator_Prefab;
    [SerializeField] private Transform[] Position_indi; //Hidden objects;

    public List<Transform> indicators_TRANSs { get; private set; }  //Spawned indicators;
    public int activated_index { get; private set; }
    public int activated_trial { get { return activated_index / 2; } }
    public int total_trial { get { return Position_indi.Length / 2; } }

    private void Awake()
    {
        this.indicators_TRANSs = new List<Transform>();
        this.activated_index = 0;
    }

    // Use this for initialization
    void Start ()
    {
        //init_indicators();
        //set_line_positions();
	}
	
	// Update is called once per frame
	void Update ()
    {


    }

    /// <summary>
    /// Init indicators;
    /// </summary>
    /// <returns>How many trials are there;</returns>
    public int init_indicators()
    {
        Transform temp_TRANS = null;
        for (int i = 0; i < Position_indi.Length; i++)
        {
            temp_TRANS = 
                Instantiate(FS_RC.IS.Fruit_Indi_Prefab, Position_indi[i].position, Quaternion.identity).
                transform;
            float size = FS_Setting.IS.IndicatorSize;
            temp_TRANS.localScale = new Vector3(size, size, size);
            temp_TRANS.SetParent(transform);
            temp_TRANS.GetComponent<FS_Indicator>().init_indicator(this, false);
            indicators_TRANSs.Add(temp_TRANS);
        }
        activated_index = -2;
        return Position_indi.Length / 2;
    }

    public void deactive_curr_pair()
    {
        if (activated_index < 0) { return; }
        indicators_TRANSs[activated_index].GetComponent<FS_Indicator>().set_act_state(false);
    }

    public void activate_next_pair()
    {
        activated_index += 2;
        indicators_TRANSs[activated_index].GetComponent<FS_Indicator>().set_act_state(true);
        set_one_line_positions(activated_index);
    }

    private void set_one_line_positions(int index)
    {
        Vector3[] positions = new Vector3[2];
        positions[0] = Position_indi[index].position;
        positions[1] = Position_indi[index + 1].position;
        FruitLineR_TRANS.GetComponent<LineRenderer>().SetPositions(positions);
        FruitLineR_TRANS.GetComponent<LineRenderer>().enabled = true;
    }

    private void set_line_positions()
    {
        Vector3[] positions = new Vector3[Position_indi.Length];
        for (int i = 0; i < Position_indi.Length; i++)
        {
            positions[i] = Position_indi[i].position;
        }
        FruitLineR_TRANS.GetComponent<LineRenderer>().SetPositions(positions);
    }

    public Vector3 get_curr_sta_pos()
    {
        return indicators_TRANSs[activated_index].position;
    }

    public Vector3 get_curr_stop_pos()
    {
        return indicators_TRANSs[activated_index+1].position;
    }

    public void prepare_TB_to_curr()
    {
        deactive_curr_pair();
        activated_index -= 2;
        activated_index = activated_index < -2 ? -2 : activated_index;
    }

    public void prepare_TB_to_last()
    {
        deactive_curr_pair();
        activated_index -= 4;
        activated_index = activated_index < -2 ? -2 : activated_index;
    }

}
