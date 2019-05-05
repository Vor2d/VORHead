using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BO_UIController : MonoBehaviour
{
    [SerializeField] private BO_RC BORC;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void quit_button()
    {
        BORC.GC_script.quit_game();
    }

    public void restart_button()
    {
        BORC.GC_script.restart_game();
    }

    public void start_button()
    {
        BORC.GC_script.pre_start_game();
    }
}
