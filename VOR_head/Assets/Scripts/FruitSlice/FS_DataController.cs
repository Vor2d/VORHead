using UnityEngine;

/// <summary>
/// Fruit Slice persistence data;
/// </summary>
public class FS_DataController : ParentDataController
{
    public FS_Setting GameSetting { get; set; }
    public GameObject[] TrialGroup_prefabs;

    public static FS_DataController IS { get; set; }

    protected override void Awake()
    {
        IS = this;

        base.Awake();

        this.GameSetting = new FS_Setting();
    }

    private void Start()
    {
        GameSetting = load_setting<FS_Setting>();
        Debug.Log("GameSetting loaded " + GameSetting.GetType());
    }

    public override void generate_setting()
    {
        generate_setting<FS_Setting>(GameSetting);
    }

}
