using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataPackage
{
    public Vector3 position;
    public Quaternion rotation;

    public DataPackage(Vector3 position, Quaternion rotation)
    {
        this.position = position;
        this.rotation = rotation;
    }
}

