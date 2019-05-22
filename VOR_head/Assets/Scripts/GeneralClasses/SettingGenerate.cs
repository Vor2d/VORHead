using UnityEngine;
using System.Collections;

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
            StartCoroutine(wait_generate_setting(DC_script));
        }
    }

    private IEnumerator wait_generate_setting(ParentDataController DC_script)
    {
        yield return new WaitForSeconds(0.5f);
        DC_script.generate_setting();
    }
}
