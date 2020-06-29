using UnityEngine;
using System.Collections.Generic;

public class FS_FruitIndicator : MonoBehaviour {

    public FS_Fruit F_script;
    public Color ActivateColor = Color.green;
    public Color FocusColor = Color.red;
    public Color DeActivatedColor = Color.white;
    [SerializeField] private Transform FruitLineR_TRANS;
    [SerializeField] private GameObject Indicator_Prefab;
    [SerializeField] private Transform[] Position_indi;

    public List<Transform> indicators_TRANSs { get; private set; }
    public int activated_index { get; private set; }

    private void Awake()
    {
        this.indicators_TRANSs = new List<Transform>();
        this.activated_index = 0;
    }

    // Use this for initialization
    void Start ()
    {
        init_indicators();
        set_line_positions();
	}
	
	// Update is called once per frame
	void Update ()
    {


    }

    private void init_indicators()
    {
        Transform temp_TRANS;
        for (int i = 0; i < Position_indi.Length; i++)
        {
            temp_TRANS = 
                Instantiate(Indicator_Prefab, Position_indi[i].position, Quaternion.identity).transform;
            temp_TRANS.SetParent(transform);
            if(i == 0)
            {
                temp_TRANS.GetComponent<FS_Indicator>().init_indicator(this, true);
                F_script.set_start_pos(temp_TRANS.position);
            }
            else
            {
                temp_TRANS.GetComponent<FS_Indicator>().init_indicator(this, false);
            }
            indicators_TRANSs.Add(temp_TRANS);
        }
        activated_index = 0;
    }

    private void set_line_positions()
    {
        Vector3[] positions = new Vector3[Position_indi.Length];
        for(int i = 0; i < Position_indi.Length;i++)
        {
            positions[i] = Position_indi[i].position;
        }
        FruitLineR_TRANS.GetComponent<LineRenderer>().SetPositions(positions);
    }



}
