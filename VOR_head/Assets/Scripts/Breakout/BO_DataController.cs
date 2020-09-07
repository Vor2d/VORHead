using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BO_DataController : ParentDataController 
{
    [SerializeField] private Texture2D[] Pattern_textures;

    private BO_Setting Setting;
    public List<BO_BrickPattern> Patterns { get; private set; }

    public static BO_DataController IS;

    protected override void Awake()
    {
        IS = this;

        base.Awake();
        this.Setting = new BO_Setting();
        this.Patterns = new List<BO_BrickPattern>();
    }

    // Use this for initialization
    void Start()
    {
        load_setting();
        get_patterns();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void generate_setting()
    {
        base.generate_setting<BO_Setting>(Setting);
    }

    public void load_setting()
    {
        Setting = base.load_setting<BO_Setting>();
    }

    private void get_patterns()
    {
        BO_BrickPattern temp = null;
        foreach(Texture2D tex in Pattern_textures)
        {
            temp = new BO_BrickPattern();
            temp.generate_pattern_binary(tex);
            Patterns.Add(temp);
        }
    }
}
