
/// <summary>
/// Fruit Slice persistence data;
/// </summary>
public class FS_DataController : ParentDataController
{
    public FS_Setting GameSetting { get; set; }

    protected override void Awake()
    {
        base.Awake();

        this.GameSetting = new FS_Setting();
        GameSetting = load_setting<FS_Setting>();
    }

}
