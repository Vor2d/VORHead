﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;

using HMTS_enum;

/// <summary>
/// A static class that contains a lot of common methods that will be reused;
/// </summary>
public static class GeneralMethods {

    private static char[] file_spliter = new char[] { ' ', '\t' };
    private const char line_separator = '_';
    private enum Direction { left, right };

    #region HMTS
    //y is horizontal, rotate by y axis; x is vertical, rotate by x axis;
    //Convert the real world angles to virtual world angles;
    [Obsolete("Not using flat screen")]
    public static float RealToVirtualy(float player_screen_cm,float screen_width_cm,
                                                                                float degree)
    {
        if ((degree+90.0f)%180.0f == 0)
        {
            return 90.0f;
        }
        float abs_degree = Mathf.Abs(degree);
        float dest_on_screen_cm = Mathf.Tan(abs_degree * Mathf.PI / 180.0f)*player_screen_cm;   //DOSC
        float prop_DOSC_to_halfwcm = dest_on_screen_cm / (screen_width_cm / 2.0f);
        float dest_on_screen_pixel = ((float)Screen.width / 2.0f) * prop_DOSC_to_halfwcm
                                        + ((float)Screen.width / 2.0f);
        Vector3 virtual_position_base10 =
            Camera.main.ScreenToWorldPoint(new Vector3(dest_on_screen_pixel, 0.0f, 10.0f));
        float prop_virx_to_base10 = virtual_position_base10.x / 10.0f;
        float virtual_degreey = Mathf.Atan(prop_virx_to_base10)/Mathf.PI*180.0f;

        if(degree < 0.0f)
        {
            virtual_degreey = -virtual_degreey;
        }

        return virtual_degreey;
    }

    public static float RealToVirtualy_curved(float player_screen_cm, float screen_width_cm,
                                                float degree, float screen_reso_h,
                                                float scale = 1.0f)
    {
        degree *= scale;
        float abs_degree = Mathf.Abs(degree);
        float dest_on_screen_cm = (abs_degree / 360.0f) * 2 * Mathf.PI * player_screen_cm;   //DOSC;
        //Debug.Log("dest_on_screen_cm " + dest_on_screen_cm);
        float prop_DOSC_to_halfwcm = dest_on_screen_cm / (screen_width_cm / 2.0f);
        float dest_on_screen_pixel = ((float)screen_reso_h / 2.0f) * prop_DOSC_to_halfwcm
                                        + ((float)screen_reso_h / 2.0f);
        Vector3 virtual_position_base10 =
            Camera.main.ScreenToWorldPoint(new Vector3(dest_on_screen_pixel, 0.0f, 10.0f));
        float prop_virx_to_base10 = virtual_position_base10.x / 10.0f;
        float virtual_degreey = Mathf.Atan(prop_virx_to_base10) / Mathf.PI * 180.0f;

        if (degree < 0.0f)
        {
            virtual_degreey = -virtual_degreey;
        }

        return virtual_degreey;
    }

    private static float RealToVirtualx(float player_screen_cm, float screen_height_cm,
                                        float degree, float screen_reso_v)
    {
        if ((degree + 90.0f) % 180.0f == 0)
        {
            return 90.0f;
        }
        float abs_degree = Mathf.Abs(degree);
        float dest_on_screen_cm = Mathf.Tan(abs_degree * Mathf.PI / 180.0f) * player_screen_cm;   //DOSC
        float prop_DOSC_to_halfhcm = dest_on_screen_cm / (screen_height_cm / 2.0f);
        float dest_on_screen_pixel = ((float)screen_reso_v / 2.0f) * prop_DOSC_to_halfhcm
                                        + ((float)screen_reso_v / 2.0f);
        Vector3 virtual_position_base10 =
            Camera.main.ScreenToWorldPoint(new Vector3(0.0f, dest_on_screen_pixel, 10.0f));
        float prop_viry_to_base10 = virtual_position_base10.y / 10.0f;
        float virtual_degreey = Mathf.Atan(prop_viry_to_base10) / Mathf.PI * 180;

        if (degree < 0.0f)
        {
            virtual_degreey = -virtual_degreey;
        }


        return virtual_degreey;
    }

    //rot_degreex is vertical, rotate by x axis; Vertical first!!!
    [Obsolete("Not using flat screen")]
    public static Vector3 RealToVirtual(float player_screen_cm, float screen_width_cm, 
                                        float rot_degreex, float rot_degreey,
                                        float screen_reso_v,float screen_reso_h)
    {
        float screen_htow_scale = (float)screen_reso_v / (float)screen_reso_h;
        float screen_height_cm = screen_htow_scale * screen_width_cm;
        float virtual_rotex = 0.0f;
        float virtual_rotey = 0.0f;

        virtual_rotex = RealToVirtualx(player_screen_cm, screen_height_cm, rot_degreex,screen_reso_v);
        virtual_rotey = RealToVirtualy(player_screen_cm, screen_width_cm, rot_degreey);

        return new Vector3(virtual_rotex, virtual_rotey, 0.0f);
    }

    public static Vector3 RealToVirtual_curved(float player_screen_cm, float screen_width_cm,
                                                float rot_degreex, float rot_degreey,float screen_reso_v,
                                                float screen_reso_h,float scale = 1.0f)
    {
        rot_degreey *= scale;
        float screen_htow_scale = (float)screen_reso_v / (float)screen_reso_h;
        float screen_height_cm = screen_htow_scale * screen_width_cm;
        float virtual_rotex = 0.0f;
        float virtual_rotey = 0.0f;

        virtual_rotex = RealToVirtualx(player_screen_cm, screen_height_cm, rot_degreex,screen_reso_v);
        virtual_rotey = RealToVirtualy_curved(player_screen_cm, screen_width_cm, rot_degreey,screen_reso_h);

        return new Vector3(virtual_rotex, virtual_rotey, 0.0f);
    }

    //Load trials data with TrialInfo class
    public static List<Section> load_game_data_general(string path)
    {
        List<Section> sections = new List<Section>();

        bool section_state_GM = true;
        int in_line_counter = 0;
        Dictionary<string, string> temp_para_dict = new Dictionary<string, string>();
        GameMode temp_gameMode = new GameMode();
        TrialInfo temp_trialInfo = new TrialInfo();

        Debug.Log("Loading game data file " + path);
        try
        {
            StreamReader reader = new StreamReader(path);
            while (!reader.EndOfStream)
            {
                in_line_counter++;
                if (section_state_GM)
                {
                    string[] splitstr = reader.ReadLine().Split(file_spliter);
                    if (splitstr[0][0] == line_separator)
                    {
                        section_state_GM = false;
                        in_line_counter = 0;
                        temp_gameMode.set_preset_para(temp_para_dict);
                        continue;
                    }
                    else
                    {
                        if (in_line_counter == 1)
                        {
                            switch (splitstr[1])
                            {
                                case "GazeTest":
                                    {
                                        temp_gameMode.
                                    set_preset_mode(GameModeEnum.GazeTest);
                                        break;
                                    }
                                case "EyeTest":
                                    {
                                        temp_gameMode.
                                    set_preset_mode(GameModeEnum.EyeTest);
                                        break;
                                    }
                                case "Feedback_Learning":
                                    {
                                        temp_gameMode.
                                    set_preset_mode(GameModeEnum.Feedback_Learning);
                                        break;
                                    }
                                case "Jump_Learning":
                                    {
                                        temp_gameMode.
                                    set_preset_mode(GameModeEnum.Jump_Learning);
                                        break;
                                    }
                                case "HC_FB_Learning":
                                    {
                                        temp_gameMode.
                                    set_preset_mode(GameModeEnum.HC_FB_Learning);
                                        break;
                                    }
                                case "Training":
                                    {
                                        temp_gameMode.
                                    set_preset_mode(GameModeEnum.Training);
                                        break;
                                    }
                                case "NoReddot":
                                    {
                                        temp_gameMode.
                                    set_preset_mode(GameModeEnum.NoReddot);
                                        break;
                                    }
                                case "StaticAcuity":
                                    {
                                        temp_gameMode.
                                    set_preset_mode(GameModeEnum.StaticAcuity);
                                        break;
                                    }
                                case "DynamicAcuity":
                                    {
                                        temp_gameMode.
                                    set_preset_mode(GameModeEnum.DynamicAcuity);
                                        break;
                                    }
                                case "PostDynamicAcuity":
                                    {
                                        temp_gameMode.
                                    set_preset_mode(GameModeEnum.PostDynamicAcuity);
                                        break;
                                    }
                                default:
                                    {
                                        temp_gameMode.
                                    set_preset_mode(GameModeEnum.Default);
                                        break;
                                    }
                            }
                        }
                        else
                        {
                            temp_para_dict.Add(splitstr[0], splitstr[1]);
                        }
                    }
                }
                else
                {
                    string[] splitstr = reader.ReadLine().Split(file_spliter);
                    if (splitstr[0][0] == line_separator)
                    {
                        Section temp_section = new Section(temp_gameMode, temp_trialInfo);
                        sections.Add(temp_section);

                        section_state_GM = true;
                        in_line_counter = 0;
                        temp_gameMode = new GameMode();
                        temp_para_dict = new Dictionary<string, string>();
                        temp_section = new Section();
                        temp_trialInfo = new TrialInfo();
                        continue;
                    }
                    else
                    {
                        if (in_line_counter == 1)
                        {
                            try { temp_trialInfo.Loop_number = Int32.Parse(splitstr[0]); }
                            catch (Exception e) { Debug.Log(e); }
                        }
                        else
                        {
                            temp_trialInfo.Turn_data.
                                    Add(new Vector2(float.Parse(splitstr[0]), 0.0f));
                            temp_trialInfo.Jump_data.
                                    Add(new Vector2(float.Parse(splitstr[1]), 0.0f));
                        }
                    }
                }
            }
            reader.Close();
        }
        catch (Exception e)
        {
            Debug.Log("Reading file error! " + e);
        }
        Debug.Log("Loading game data complete! ");

        return sections;
    }

    public static GameSetting read_game_setting_general(string path)
    {
        Dictionary<string, string> setting_dict = new Dictionary<string, string>();
        GameSetting system_setting = new GameSetting();

        Debug.Log("Loading game settings.");
        try
        {
            StreamReader reader = new StreamReader(path);
            while (!reader.EndOfStream)
            {
                string[] splitstr = reader.ReadLine().Split(new char[] { ' ', '\t' });
                setting_dict.Add(splitstr[0], splitstr[1]);
            }
            reader.Close();
        }
        catch (Exception e)
        {
            Debug.Log("Reading file error! " + e);
        }
        Debug.Log("Loading game settings complete.");


        system_setting.set_preset_setting(setting_dict);

        return system_setting;
    }

    //Monitor Change Position General Method;
    private static float DestinationCal(float init_dist, float ang_deg, int direc)
    {
        if ((ang_deg + 90) % 180 == 0)
        {
            Debug.Log("Angle can not be 0 or 180");
            return float.MaxValue;
        }

        float ang_rand = ang_deg * Mathf.PI / 180.0f;

        float destnation = Mathf.Tan(ang_rand) * init_dist;

        if (direc == (int)(Direction.left))
        {
            destnation = -destnation;
        }

        return destnation;
    }

    private static float DestinationCal(float init_dist, float ang_deg)
    {
        if ((ang_deg + 90) % 180 == 0)
        {
            throw new Exception("Angle can not be 90 or 0");
        }

        float ang_rand = ang_deg * Mathf.PI / 180.0f;

        float destnation = Mathf.Tan(ang_rand) * init_dist;

        return destnation;
    }

    public static Vector3 PositionCal(float init_dist, float ang_degX, float ang_degY,
                                                                    int direcX, int direcY)
    {
        return new Vector3(DestinationCal(init_dist, ang_degX, direcX),
                            DestinationCal(init_dist, ang_degY, direcY),
                            init_dist);
    }

    public static Vector3 PositionCalSphere(float init_dist, float ang_degX,
        float ang_degY, int direcX, int direcY)
    {
        float x_raid = ang_degX * (direcX == 0 ? -1 : 1) * Mathf.Deg2Rad;
        float y_raid = ang_degY * (direcY == 0 ? -1 : 1) * Mathf.Deg2Rad;
        float x = init_dist * Mathf.Sin(x_raid) * Mathf.Cos(y_raid);
        float y = init_dist * Mathf.Sin(x_raid) * Mathf.Sin(y_raid);
        float z = init_dist * Mathf.Cos(x_raid);
        return new Vector3(x, y, z);
    }

    public static Vector3 PositionCal(float init_dist, float ang_degX, float ang_degY)
    {
        return new Vector3(DestinationCal(init_dist, ang_degX),
                            -DestinationCal(init_dist, ang_degY),   //Invert sign because rotation and position have different positive direction;
                            init_dist);
    }

    public static Vector3 PositionCal(float init_dist, Vector3 degrees)
    {
        Vector3 n_degrees = normalize_degree(degrees);
        return new Vector3(DestinationCal(init_dist, n_degrees.y),
                            -DestinationCal(init_dist, n_degrees.x),
                            init_dist);
    }

    //Loading trials for both vertical and horizontal. Vertical first, horizontal second.
    public static List<Vector2> read_trial_file_VNH(string path)
    {

        List<Vector2> trial_data = new List<Vector2>();
        Debug.Log("Loading trials from file " + path);
        try
        {
            StreamReader reader = new StreamReader(path);
            while (!reader.EndOfStream)
            {
                string[] splitstr = reader.ReadLine().Split(file_spliter);
                trial_data.Add(new Vector2(float.Parse(splitstr[0]), float.Parse(splitstr[1])));
            }
        }
        catch (Exception e)
        {
            Debug.Log("Loading Failed! " + e);
        }
        Debug.Log("Loading Successful!");

        return trial_data;

    }

    public static int change_by_percent(ref int index,int number, ref int right_num, float up_cent,float down_cent)
    {
        index++;
        int result = -10;
        if (index > number)
        {
            if (right_num > (number * up_cent))
            {
                result = 1;
            }
            else if ((number - right_num) > (number - (number * down_cent)))
            {
                result = -1;
            }
            index = 1;
            right_num = 0;
        }
        else
        {
            result = 0;
        }

        return result;
    }

    #endregion

    /// <summary>
    /// Put degree to -180 to 180;
    /// </summary>
    public static float normalize_degree(float degree)
    {
        return (degree >= 180) ? -(360 - degree) : degree;
    }

    public static Vector3 normalize_degree(Vector3 degree)
    {
        return new Vector3(normalize_degree(degree.x),
                            normalize_degree(degree.y),
                            normalize_degree(degree.z));
    }

    //Method to get headset speed;
    public static Vector3 getVRspeed()
    {
        Vector3 angularVelocityRead =
                (OVRPlugin.GetNodeAngularVelocity(OVRPlugin.Node.Head, OVRPlugin.Step.Render).
                FromFlippedZVector3f()) * Mathf.Rad2Deg;

        return angularVelocityRead;
    }

    //Method to get headset rotation;
    public static Quaternion getVRrotation()
    {
        //Quaternion VRrotation = InputTracking.GetLocalRotation(XRNode.CenterEye);
        Quaternion quatf_orientation =
                        OVRPlugin.GetNodePose(OVRPlugin.Node.EyeCenter, OVRPlugin.Step.Render).
                        ToOVRPose().orientation;
        Quaternion VRrotation = new Quaternion(quatf_orientation.x, quatf_orientation.y, 
                                                quatf_orientation.z, quatf_orientation.w);

        return VRrotation;
    }

    public static float get_median(List<float> data_list)
    {
        data_list.Sort();

        if (data_list.Count % 2 != 0)
        {
            return data_list[data_list.Count / 2];
        }
        else
        {
            return (data_list[data_list.Count / 2] + data_list[data_list.Count / 2 - 1])
                            / 2.0f;
        }
    }

    public static void fit_linear(float x1, float x2, float y1, float y2, 
                                    out float k, out float b)
    {
        k = 0;
        b = 0;
        float delta_y = y1 - y2;
        float delta_x = x1 - x2;
        k = delta_y / delta_x;
        b = y1 - (k * x1);
    }

    public static void recenter_VR()
    {
        UnityEngine.XR.InputTracking.Recenter();
    }

    //Have to stop from outside;
    public static IEnumerator blink_object(GameObject target_OBJ, float inter_time, 
                                            Color target_color)
    {
        Material target_material_REF = target_OBJ.GetComponent<MeshRenderer>().material;
        Color init_color = target_material_REF.color;
        bool orin_color_flag = true;
        while (true)
        {
            if(orin_color_flag)
            {
                target_material_REF.color = target_color;
            }
            else
            {
                target_material_REF.color = init_color;
            }
            orin_color_flag = !orin_color_flag;
            yield return new WaitForSeconds(inter_time);
        }
    }

    public static void start_timer(ref float timer, ref bool timer_flag, float init_time)
    {
        timer_flag = true;
        timer = init_time;
    }

    public static bool check_timer(float timer,ref bool timer_flag)
    {
        if (timer < 0.0f)
        {
            timer = float.MaxValue;
            timer_flag = false;
            return true;
        }
        return false;
    }

    public static void check_ref<T>(ref T target_OBJ, string obj_name) 
                                                                where T : MonoBehaviour
    {
        if(target_OBJ == null)
        {
            target_OBJ = GameObject.Find(obj_name).GetComponent<T>();
            if(target_OBJ == null)
            {
                Debug.Log("Can't find the object for check_ref of " + typeof(T));
            }
        }
    }

    public static void reset_animator_triggers(Animator animator)
    {
        foreach (AnimatorControllerParameter parameter in animator.parameters)
        {
            try
            {
                animator.ResetTrigger(parameter.name);
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        }
    }

    /// <summary>
    /// Load setting file from Json.
    /// </summary>
    public static T load_setting<T>(string setting_path, string setting_file_name)
    {
        try
        {
            string from_json = File.ReadAllText(setting_path + setting_file_name);
            return JsonUtility.FromJson<T>(from_json);
        }
        catch (Exception e) { Debug.Log("Reading settings error " + e); return default(T); }
    }

    /// <summary>
    /// Generate setting file template.
    /// </summary>
    public static void generate_setting<T>(T setting_class, string setting_path, string setting_file_name)
    {
        try
        {
            if (!Directory.Exists(setting_path))
            {
                Directory.CreateDirectory(setting_path);
            }
            string from_class = JsonUtility.ToJson(setting_class);
            Debug.Log("Writing file " + setting_file_name + "!!!");
            File.WriteAllText(setting_path + setting_file_name, from_class);
        }
        catch (Exception e) { Debug.Log("Generating settings error " + e); }
    }

    public static void active_UI_page(Transform target_page,List<Transform> pages)
    {
        foreach(Transform page in pages)
        {
            if (GameObject.ReferenceEquals(target_page, page))
            {
                page.gameObject.SetActive(true);
            }
            else
            {
                page.gameObject.SetActive(false);
            }
        }
    }

    public static Vector3 world_to_canvas(Vector3 target_pos,Camera camera,Canvas canvas,bool boundary = false)
    {
        Vector3 screenPos = new Vector3();
        try
        {
            screenPos = camera.WorldToViewportPoint(target_pos);
        }
        catch (Exception e) 
        {
            Debug.Log("world_to_canvas "+e);
            screenPos.z = -1.0f;
        }
        if(boundary)
        {
            if (screenPos.x < 0.0f) { screenPos.x = 0.0f; }
            else if (screenPos.x > 1.0f) { screenPos.x = 1.0f; }
            if (screenPos.y < 0.0f) { screenPos.y = 0.0f; }
            else if (screenPos.y > 1.0f) { screenPos.y = 1.0f; }
        }
        if (screenPos.z < 0) { screenPos = new Vector3(int.MaxValue, int.MaxValue, 0.0f); }
        else
        {
            screenPos.x *= canvas.GetComponent<RectTransform>().rect.width;
            screenPos.x -= canvas.GetComponent<RectTransform>().rect.width / 2.0f;
            screenPos.y *= canvas.GetComponent<RectTransform>().rect.height;
            screenPos.y -= canvas.GetComponent<RectTransform>().rect.height / 2.0f;
            screenPos.z = 0.0f;
        }

		return screenPos;

    }

    public static Vector3 world_to_canvas_LDCorner(Vector3 target_pos, Camera camera)
    {
        Vector3 screenPos = camera.WorldToScreenPoint(target_pos, Camera.MonoOrStereoscopicEye.Mono);
        if (screenPos.z < 0) { screenPos = new Vector3(int.MaxValue, int.MaxValue, 0.0f); }
        else
        {
            screenPos.z = 0.0f;
        }

        return screenPos;

    }

    public static void send_udp(IPAddress iPAddress, int port, byte[] package)
    {
        Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram,
                                    ProtocolType.Udp);
        IPEndPoint endPoint = new IPEndPoint(iPAddress, port);
        socket.SendTo(package, endPoint);
        socket.Close();
    }

    public static void print_queue<T>(ref Queue<T> queue)
    {
        string res = "";
        foreach (T item in queue.ToArray())
        {
            res += item.ToString() + ", ";
        }
        Debug.Log("Queue " + res);
    }
}
