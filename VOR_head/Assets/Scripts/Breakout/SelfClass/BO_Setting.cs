using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BO_Setting
{
    public Vector3 Brick_gener_center;
    public Vector3 Brick_gener_range;   //Hori range, vert range, depth range;
    public int Auto_gener_number;
    public Vector3 Brick_size;

    public static BO_Setting IS;

    public BO_Setting()
    {
        IS = this;

        this.Brick_gener_center = new Vector3(0.0f, 0.0f, 0.0f);
        this.Brick_gener_range = new Vector3(0.0f, 0.0f, 0.0f);
        this.Auto_gener_number = 5;
        this.Brick_size = new Vector3(2.0f, 1.0f, 1.0f);
    }

}
