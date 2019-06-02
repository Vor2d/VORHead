using System;
using WAMEC;

/// <summary>
/// Whac a mole VR setting class;
/// </summary>

[Serializable]
public class WAMSetting
{
    public MoleGenerShape moleGenerShape { get; private set; }

    public WAMSetting()
    {
        this.moleGenerShape = MoleGenerShape.circle;
    }

    public WAMSetting(WAMSetting other_setting)
    {
        this.moleGenerShape = other_setting.moleGenerShape;
    }
}
