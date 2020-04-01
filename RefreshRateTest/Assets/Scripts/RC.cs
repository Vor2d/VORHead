using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RC : MonoBehaviour
{
    public static RC IS;

    public AcuityGroup AG_script;
    public TargetGroup TG_script;
    [SerializeField] private string acuity_path;

    [HideInInspector]
    public Sprite[] AcuitySprites;

    private void Awake()
    {
        IS = this;

        load_acuity_sprites();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void load_acuity_sprites()
    {
        Debug.Log("Loading acuity sprites " + acuity_path);
        try
        {
            AcuitySprites = Resources.LoadAll<Sprite>(acuity_path);
            Debug.Log("Acuity sprites loaded!");
        }
        catch { Debug.Log("Acuity sprite failed"); }

    }
}
