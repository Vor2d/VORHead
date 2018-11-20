using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BO_InitController : MonoBehaviour {

    [SerializeField] private GameObject BO_prefab;

    // Use this for initialization
    void Start()
    {
        if (GameObject.Find("BO_DataController") == null)
        {
            GameObject temp_BO_OBJ = Instantiate(BO_prefab, new Vector3(), new Quaternion());
            temp_BO_OBJ.name = "BO_DataController";
        }

        SceneManager.LoadScene("Breakout");
    }

    // Update is called once per frame
    void Update()
    {

    }
}
