using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WAM_DebugController : MonoBehaviour
{
    [SerializeField] private bool UseDebug;
    [SerializeField] private GeneralFrameRateCal FR_script;
    [SerializeField] private TextMesh TM1;
    [SerializeField] private GameObject O_Prefab;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        frame_rate();
        //frame_rate2();
    }

    private void frame_rate()
    {
        TM1.text = FR_script.Frame_rate.ToString("F2");
    }

    private void frame_rate2()
    {
        Instantiate(O_Prefab);
    }
}
