using System;
using WAMEC;

/// <summary>
/// Whac a mole setting class;
/// </summary>

[Serializable]
public class WAMSetting
{
    public MoleGenerShape Mole_gener_shape;
    public float Mole_frame_dist;
    public int Mole_frame_num;
    public float Mole_frame_size;
    public MoleGenerType Mole_gener_type;
    public float Mole_size;
    public float Mole_des_time;
    public float Mole_spawn_time;



 

    public WAMSetting()
    {
        this.Mole_gener_shape = MoleGenerShape.circle;
        this.Mole_frame_dist = 3.0f;
        this.Mole_frame_num = 6;
        this.Mole_frame_size = 2.0f;
        this.Mole_gener_type = MoleGenerType.random;
        this.Mole_size = 1.0f;
        this.Mole_des_time = 2.0f;
        this.Mole_spawn_time = 3.0f;

        Instance = this;
    }

    public WAMSetting(WAMSetting other_setting)
    {
        this.Mole_gener_shape = other_setting.Mole_gener_shape;
        this.Mole_frame_dist = other_setting.Mole_frame_dist;
        this.Mole_frame_num = other_setting.Mole_frame_num;
        this.Mole_frame_size = other_setting.Mole_frame_size;
        this.Mole_gener_type = other_setting.Mole_gener_type;
        this.Mole_size = other_setting.Mole_size;
        this.Mole_des_time = other_setting.Mole_des_time;
        this.Mole_spawn_time = other_setting.Mole_spawn_time;
    }

    public static WAMSetting Instance { get; private set; }
}
