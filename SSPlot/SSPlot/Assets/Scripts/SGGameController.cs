using UnityEngine;
using System.IO;
using System;

public class SGGameController : MonoBehaviour
{
    [SerializeField] private string setting_path;
    [SerializeField] private string setting_file_name;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.S))
        {
            generate_setting();
        }
    }

    private void generate_setting()
    {
        try
        {
            if (!Directory.Exists(setting_path))
            {
                Directory.CreateDirectory(setting_path);
            }
            SSPlotSetting setting_class = new SSPlotSetting();
            string from_class = JsonUtility.ToJson(setting_class);
            Debug.Log("Writing file " + setting_file_name + "!!!");
            File.WriteAllText(setting_path + setting_file_name, from_class);
        }
        catch (Exception e) { Debug.Log("Generating settings error " + e); }
    }
}
