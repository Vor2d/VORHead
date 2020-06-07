using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ParameterSystem;

public class SS_RC : MonoBehaviour
{
    public static SS_RC IS { get; private set; }

    public Transform ErrorText1_TRANS;
    public LoadGroup GameSetting_LG;

    private void Awake()
    {
        IS = this;
    }

}
