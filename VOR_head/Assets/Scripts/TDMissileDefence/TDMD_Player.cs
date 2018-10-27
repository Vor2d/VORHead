using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TDMD_Player : MonoBehaviour {

    public int InitLife = 10;
    public TDMD_GameController TDMDGC_script;

    public int life { get; set; }

	// Use this for initialization
	void Start () {
        this.life = InitLife;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void get_hit()
    {
        Debug.Log("get hitted");
        life--;
        TDMDGC_script.Text_changed_flag = true;
    }
}
