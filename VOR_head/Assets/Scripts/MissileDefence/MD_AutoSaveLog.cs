using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MD_AutoSaveLog : MonoBehaviour
{
    [SerializeField] private GeneralLogSystem LogSystemToSave;
    [SerializeField] private float AutoSaveTime;

    private float auto_save_timer;

    // Start is called before the first frame update
    void Start()
    {
        this.auto_save_timer = AutoSaveTime;
    }

    // Update is called once per frame
    void Update()
    {
        if(LogSystemToSave.thread_state_flag)
        {
            auto_save_timer -= Time.unscaledDeltaTime;
            //Debug.Log("auto_save_timer " + auto_save_timer);
            if (auto_save_timer < 0)
            {
                update_file();
                auto_save_timer = AutoSaveTime;
            }
        }
    }

    private void update_file()
    {
        //Debug.Log("update_file");
        LogSystemToSave.write_file();
    }
}
