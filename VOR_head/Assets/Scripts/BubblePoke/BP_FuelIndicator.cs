using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BP_FuelIndicator : MonoBehaviour
{

    [SerializeField] private Transform[] Bars_TRANSs;

    private int curr_level;

    private void Awake()
    {
        this.curr_level = Bars_TRANSs.Length;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void update_bars(float percentage)
    {
        int level = Mathf.CeilToInt(Bars_TRANSs.Length * percentage);
        if(curr_level != level)
        {
            update_bars_trans(level);
            curr_level = level;
        }
    }

    private void update_bars_trans(int level)
    {
        for(int i = 0; i < Bars_TRANSs.Length; i++)
        {
            if(i < level)
            {
                Bars_TRANSs[i].GetComponent<MeshRenderer>().enabled = true;
            }
            else
            {
                Bars_TRANSs[i].GetComponent<MeshRenderer>().enabled = false;
            }
        }
    }
}
