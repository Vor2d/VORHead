using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FS_UIController : MonoBehaviour
{
    [SerializeField] private FS_RC FSRC;

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
        FSRC.GC_script.restart();
    }

    public void quit_button()
    {
        FSRC.GC_script.quit_game();
    }
}
