using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BP_Path))]
public class BP_DrawPath : MonoBehaviour
{
    private List<Vector3> stat_posi_list;

    // Start is called before the first frame update
    void Start()
    {
        this.stat_posi_list = new List<Vector3>();

        if (GetComponent<BP_Path>().BPRC.GC_script.DrawPathFlag)
        {
            get_stations();
            draw_line();
        }
    }

    private void get_stations()
    {
        foreach (Transform station_TRANS in GetComponent<BP_Path>().Stations_TRANS)
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
