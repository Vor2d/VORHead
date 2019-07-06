using System;
using WAMEC;
using System.Collections.Generic;

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
    public float Mole_frame_dist2;
    public int Mole_frame_num2;
    public List<int> Gener_list;
    public bool Using_acuity;
    public AcuityType Acuity_type;
    public float Acuity_rela_size;
    public AcuityProcess Acuity_process;
    public float Acuity_flash_time;
    public float Head_speed;
    public float Head_stop_time;
    public float Head_stop_speed;
    public float Min_distance;
    public ControllerModes Controller_mode;
    public float Controller_Dtime;
    public bool Check_too_slow;

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
        this.Mole_frame_dist2 = 0.0f;
        this.Mole_frame_num2 = 0;
        this.Gener_list = new List<int>();
        this.Using_acuity = false;
        this.Acuity_type = AcuityType.fourdir;
        this.Acuity_rela_size = 1.0f;
        this.Acuity_process = AcuityProcess.post;
        this.Acuity_flash_time = 0.1f;
        this.Head_speed = 50.0f;
        this.Head_stop_time = 0.1f;
        this.Head_stop_speed = 10.0f;
        this.Min_distance = 0.0f;
        this.Controller_mode = ControllerModes.instant;
        this.Controller_Dtime = 0.0f;
        this.Check_too_slow = false;

        IS = this;
    }

    //public WAMSetting(WAMSetting other_setting)
    //{
    //    this.Mole_gener_shape = other_setting.Mole_gener_shape;
    //    this.Mole_frame_dist = other_setting.Mole_frame_dist;
    //    this.Mole_frame_num = other_setting.Mole_frame_num;
    //    this.Mole_frame_size = other_setting.Mole_frame_size;
    //    this.Mole_gener_type = other_setting.Mole_gener_type;
    //    this.Mole_size = other_setting.Mole_size;
    //    this.Mole_des_time = other_setting.Mole_des_time;
    //    this.Mole_spawn_time = other_setting.Mole_spawn_time;
    //    this.Mole_frame_dist2 = other_setting.Mole_frame_dist2;
    //    this.Mole_frame_num2 = other_setting.Mole_frame_num2;
    //    this.Gener_list = other_setting.Gener_list;
    //    this.Using_acuity = other_setting.Using_acuity;
    //    this.Acuity_type = other_setting.Acuity_type;
    //}

    public static WAMSetting IS { get; private set; }
}
