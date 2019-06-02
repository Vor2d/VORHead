using UnityEngine;

public class WAMRC : MonoBehaviour
{
    //Transform;
    public Transform MoleCenterInidcator_TRANS;
    //Object;
    public WAM_GameController GC_script;
    [HideInInspector]
    public WAM_DataController DC_script;
    public WAM_RayCast RCT_script;
    public Controller_Input CI_script;
    //Prefab;
    public GameObject MoleCenter_Prefab;
    public GameObject MoleFrame_Prefab;
    public GameObject Mole_Prefab;
    //Pool;
    public Transform MoleCenter_TRANS { get; set; }

    private void Awake()
    {
        GeneralMethods.check_ref<WAM_DataController>(ref DC_script,WAMSD.DC_name);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
