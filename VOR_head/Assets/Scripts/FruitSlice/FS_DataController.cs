using UnityEngine;

/// <summary>
/// Fruit Slice persistence data;
/// </summary>
public class FS_DataController : ParentDataController
{
    public GameObject[] TrialGroup_prefabs;

    public FS_Setting GameSetting { get; set; }
    public FS_Player player { get; set; }

    public static FS_DataController IS { get; set; }

    protected override void Awake()
    {
        IS = this;

        base.Awake();

        this.GameSetting = new FS_Setting();

        player = new FS_Player();
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
