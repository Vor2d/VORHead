using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyGroup : BodyParent
{
    private void Start()
    {
        if (AutoResetHeight) { adjust_height(); }
    }
}
