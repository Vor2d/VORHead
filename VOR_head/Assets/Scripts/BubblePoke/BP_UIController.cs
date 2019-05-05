using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BP_UIController : MonoBehaviour
{
    [SerializeField] private BP_RC BPRC;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void restart_button()
    {
        BPRC.GC_script.restart_game();
    }

    public void quit_button()
    {
        BPRC.GC_script.quit_game();
    }
}
