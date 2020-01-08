using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RawDigits : MonoBehaviour
{
    [SerializeField] private TextMesh TM;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        float[] RD = CoilData.IS.Raw_data;
        TM.text = RD[0].ToString("F3") + " | " +
                    RD[1].ToString("F3") + " | " +
                    RD[2].ToString("F3") + " | " +
                    "\n" +
                    RD[3].ToString("F3") + " | " +
                    RD[4].ToString("F3") + " | " +
                    RD[5].ToString("F3") + " | ";
    }
}
