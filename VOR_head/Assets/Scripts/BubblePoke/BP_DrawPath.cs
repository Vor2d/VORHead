using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BP_DrawPath : MonoBehaviour
{
    [SerializeField] private BP_RC BPRC;

    private List<Vector3> stat_posi_list;

    // Start is called before the first frame update
    void Start()
    {
        this.stat_posi_list = new List<Vector3>();

        get_stations();
        draw_line();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void get_stations()
    {
        foreach (Transform station_TRANS in BPRC.Stations_TRANS)
        {
            stat_posi_list.Add(station_TRANS.position);
        }
    }

    private void draw_line()
    {
        GetComponent<LineRenderer>().positionCount = stat_posi_list.Count;
        GetComponent<LineRenderer>().SetPositions(stat_posi_list.ToArray());
    }
}
