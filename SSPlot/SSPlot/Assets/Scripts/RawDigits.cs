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
        TM.text = "hdx "+ RD[0].ToString("F3") + " | " +
                    "hdy " + RD[1].ToString("F3") + " | " +
                    "hdz " + RD[2].ToString("F3") + " | " +
                    "\n" +
                    "htx " + RD[3].ToString("F3") + " | " +
                    "hty " + RD[4].ToString("F3") + " | " +
                    "htz " + RD[5].ToString("F3") + " | ";
    }
}
