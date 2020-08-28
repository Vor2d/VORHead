using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Breakout paddle hit indicator;
public class BO_PadHitInd : MonoBehaviour {

    [SerializeField] private GameObject HitIndicator;
    [SerializeField] BO_PadCollider PC_script;
    [SerializeField] BO_PadCollider PTrigger_script;
    [SerializeField] float Offset;

    private BO_PadCollider activated_PC;

    private void Awake()
    {
        this.activated_PC = null;
    }

    // Use this for initialization
    void Start () 
    {
        register_spawn();

    }
	
	// Update is called once per frame
	void Update () {

	}

    private void OnDestroy()
    {
        deregister_spawn();
    }

    private void register_spawn()
    {
        if (PC_script.gameObject.activeSelf) 
        {
            PC_script.OnCEnter_CB += spawn_indicator;
            activated_PC = PC_script;
        }
        if(PTrigger_script.gameObject.activeSelf)
        {
            PTrigger_script.OnCEnter_CB += spawn_indicator;
            activated_PC = PTrigger_script;
        }
        
    }

    private void deregister_spawn()
    {
        PC_script.OnCEnter_CB -= spawn_indicator;
        PTrigger_script.OnCEnter_CB -= spawn_indicator;
    }

    private void spawn_indicator()
    {
        Instantiate(HitIndicator, new Vector3(activated_PC.Contact_point.x, activated_PC.Contact_point.y,
            transform.position.z - Offset), new Quaternion(), transform);
    }


}
