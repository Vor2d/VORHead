using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FS_FruitRenderer : MonoBehaviour {

    [SerializeField] private FS_Fruit FSF_script;
    [SerializeField] private Transform FirstHalf_TRANS;
    [SerializeField] private Transform SecondHalf_TRANS;
    [SerializeField] private float STime = 1.0f;

    private float Stimer;
    private bool cut_triggered;

    // Use this for initialization
    void Start () {
        this.Stimer = STime;
        this.cut_triggered = false;
	}
	
	// Update is called once per frame
	void Update () {
		if(FSF_script.Sliced_flag && !cut_triggered)
        {
            cut();
        }
	}

    private void cut()
    {
        StartCoroutine(seperate());
    }

    IEnumerator seperate()
    {
        while(Stimer > 0.0f)
        {
            Stimer -= Time.deltaTime;
            FirstHalf_TRANS.Translate(Vector3.down*Time.deltaTime);
  ;
            SecondHalf_TRANS.Translate(Vector3.up*Time.deltaTime);
            yield return null;
        }
    }
}
