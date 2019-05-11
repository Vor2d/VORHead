using UnityEngine;

public class GeneralHeadSimulatorRNP : MonoBehaviour {

    [SerializeField] protected string DC_name;
    [SerializeField] protected Transform Camera_TRANS;

    protected ParentDataController PDC_script;

    // Use this for initialization
    protected virtual void Start()
    {
        try
        {
            this.PDC_script =
                GameObject.Find(DC_name).GetComponent<ParentDataController>();
        }
        catch { Debug.Log("Can not find object! " + DC_name); }

    }

    // Update is called once per frame
    protected virtual void Update()
    {
        transform.position = Camera_TRANS.position;
        if (PDC_script != null && PDC_script.using_VR)
        {
            transform.rotation = GeneralMethods.getVRrotation();
        }
    }

}
