using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BO_UIController : MonoBehaviour
{
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
        BO_GameController.IS.quit_game();
    }

    public void restart_button()
    {
        BO_GameController.IS.restart_game();
    }

    public void start_button()
    {
        BO_GameController.IS.pre_start_game();
    }
}
