using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BP_Path))]
public class BP_DrawPath : MonoBehaviour
{
    [SerializeField] private float Alpha;

    private List<Vector3> stat_posi_list;

    // Start is called before the first frame update
    void Start()
    {
        this.stat_posi_list = new List<Vector3>();
    }

    public void draw_path()
    {
        get_stations();
        draw_line();
    }

    public void draw_path(Color _theme_color)
    {
        get_stations();
        draw_line(_theme_color);
    }

    private void get_stations()
    {
        foreach (Transform station_TRANS in GetComponent<BP_Path>().Stations_TRANSs)
        {
            stat_posi_list.Add(station_TRANS.position);
        }
    }

    private void draw_line()
    {
        GetComponent<LineRenderer>().positionCount = stat_posi_list.Count;
        GetComponent<LineRenderer>().SetPositions(stat_posi_list.ToArray());
    }

    private void draw_line(Color _theme_color)
    {
        GetComponent<LineRenderer>().positionCount = stat_posi_list.Count;
        GetComponent<LineRenderer>().SetPositions(stat_posi_list.ToArray());
        GetComponent<LineRenderer>().materials[0].color =
            new Color(_theme_color.r, _theme_color.g, _theme_color.b, Alpha / 255);
    }


}
