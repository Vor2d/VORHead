using Boo.Lang;
using UnityEngine;

public class WAMRC : MonoBehaviour
{
    //Transform;
    public Transform MoleCenterInidcator_TRANS;
    public Transform WhacPartical_TRANS;
    public Transform Text1_TRANS;
    public Transform ScoreText_TRANS;
    public Transform BG_Grid_TRANS;
    public Transform Splash_TRANS;
    public Transform Fishnet_TRANS;
    public Transform TimerText_TRANS;
    public Transform HeadIndi_TRANS;
    public Transform BonusText_TRANS;
    //Object;
    //public WAM_GameController GC_script;
    [HideInInspector]
    //public WAM_DataController DC_script;
    public WAM_RayCast RCT_script;
    public Controller_Input CI_script;
    //Prefab;
    public GameObject MoleCenter_Prefab;
    public GameObject MoleFrame_Prefab;
    public GameObject Mole_Prefab;
    public GameObject[] Fish_Prefabs;
    //public GameObject Hole_Prefab;
    //Pool;
    public Transform MoleCenter_TRANS { get; set; }

    public static WAMRC IS { get; private set; }

    private void Awake()
    {
        IS = this;
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
