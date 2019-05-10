using UnityEngine;

public class SettingGenerate : MonoBehaviour
{
    [SerializeField] private GameObject[] DC_Prefabs;
    [SerializeField] KeyCode Key;

    private void Update()
    {
        if (Input.GetKeyDown(Key))
        {
            file_generation();
        }
    }

    private void file_generation()
    {
        foreach(GameObject DC_Prefab in DC_Prefabs)
        {
            ParentDataController DC_script = 
                                Instantiate(DC_Prefab, Vector3.zero, Quaternion.identity).
                                GetComponent<ParentDataController>();
            DC_script.generate_setting();
        }
    }
}
