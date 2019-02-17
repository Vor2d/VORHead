using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BP_TrailInfo
{
    public List<Vector2> degree_info { get;private set; }

    public BP_TrailInfo()
    {
        this.degree_info = new List<Vector2>();
    }

    public BP_TrailInfo(List<Vector2> deg_info)
    {
        this.degree_info = deg_info;
    }

    public void set_deg_info(List<Vector2> deg_info)
    {
        degree_info = deg_info;
    }

}
