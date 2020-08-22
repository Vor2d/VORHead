using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using UnityEngine.SceneManagement;
using UnityEngine.XR;

using HMTS_enum;
using System.Linq;

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
                                                                                float degree)
    {
        float abs_degree = Mathf.Abs(degree);
        float dest_on_screen_cm = (abs_degree / 360.0f) * 2 * Mathf.PI * player_screen_cm;   //DOSC;
        //Debug.Log("dest_on_screen_cm " + dest_on_screen_cm);
        float prop_DOSC_to_halfwcm = dest_on_screen_cm / (screen_width_cm / 2.0f);
        float dest_on_screen_pixel = ((float)Screen.width / 2.0f) * prop_DOSC_to_halfwcm
                                        + ((float)Screen.width / 2.0f);
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
                                                                                float degree)
    {
        if ((degree + 90.0f) % 180.0f == 0)
        {
            return 90.0f;
        }
        float abs_degree = Mathf.Abs(degree);
        float dest_on_screen_cm = Mathf.Tan(abs_degree * Mathf.PI / 180.0f) * player_screen_cm;   //DOSC
        float prop_DOSC_to_halfhcm = dest_on_screen_cm / (screen_height_cm / 2.0f);
        float dest_on_screen_pixel = ((float)Screen.height / 2.0f) * prop_DOSC_to_halfhcm
                                        + ((float)Screen.height / 2.0f);
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
    public static Vector3 RealToVirtual(float player_screen_cm, float screen_width_cm, 
                                        float rot_degreex, float rot_degreey)
    {
        float screen_htow_scale = (float)Screen.height / (float)Screen.width;
        float screen_height_cm = screen_htow_scale * screen_width_cm;
        float virtual_rotex = 0.0f;
        float virtual_rotey = 0.0f;

        virtual_rotex = RealToVirtualx(player_screen_cm, screen_height_cm, rot_degreex);
        virtual_rotey = RealToVirtualy(player_screen_cm, screen_width_cm, rot_degreey);

        return new Vector3(virtual_rotex, virtual_rotey, 0.0f);
    }

    public static Vector3 RealToVirtual_curved(float player_screen_cm, float screen_width_cm,
                                                float rot_degreex, float rot_degreey)
    {
        float screen_htow_scale = (float)Screen.height / (float)Screen.width;
        float screen_height_cm = screen_htow_scale * screen_width_cm;
        float virtual_rotex = 0.0f;
        float virtual_rotey = 0.0f;

        virtual_rotex = RealToVirtualx(player_screen_cm, screen_height_cm, rot_degreex);
        virtual_rotey = RealToVirtualy_curved(player_screen_cm, screen_width_cm, rot_degreey);

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
    public static Vector3 getVRspeed(bool UsingXR = true)
    {
        Vector3 angularVelocityHead = new Vector3();
        if (UsingXR) { angularVelocityHead = XRDeviceManager.get_head_Angularspeed(); }
        else
        {
            //angularVelocityHead =
            //    (OVRPlugin.GetNodeAngularVelocity(OVRPlugin.Node.Head, OVRPlugin.Step.Render).
            //    FromFlippedZVector3f()) * Mathf.Rad2Deg;
        }
        return angularVelocityHead;
    }

    //Method to get headset rotation;
    //Quaternion VRrotation = InputTracking.GetLocalRotation(XRNode.CenterEye);
    public static Quaternion getVRrotation(bool UsingXR = true)
    {
        if (UsingXR) { return XRDeviceManager.get_head_Orientation(); }
        else
        {
            //Quaternion quatf_orientation =
            //    OVRPlugin.GetNodePose(OVRPlugin.Node.EyeCenter, OVRPlugin.Step.Render).
            //    ToOVRPose().orientation;
            //Quaternion VRrotation = new Quaternion(quatf_orientation.x, quatf_orientation.y,
            //                                        quatf_orientation.z, quatf_orientation.w);

            //return VRrotation;
        }
        return Quaternion.identity;
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
        //UnityEngine.XR.InputTracking.Recenter();
        XRDeviceManager.InitXRDevice();
        XRDeviceManager.recenter();
    }

    public static float recenter_VR(Transform Cam_Par_TRANS, Transform Cam_TRANS)
    {
        XRDeviceManager.InitXRDevice();
        XRDeviceManager.recenter();
        float refer_height = Cam_Par_TRANS.position.y - Cam_TRANS.position.y;
        Cam_Par_TRANS.position = new Vector3(Cam_Par_TRANS.position.x, refer_height, Cam_Par_TRANS.position.z);
        return refer_height;
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

    public static void start_timer_down(ref float timer, ref bool timer_flag, float init_time)
    {
        timer_flag = true;
        timer = init_time;
    }

    public static bool up_ch_timer_down(ref float timer, ref bool timer_flag, float delta_time = -1.0f)
    {
        if(timer_flag)
        {
            if (delta_time < 0) { timer -= Time.deltaTime; }
            else { timer -= delta_time; }
            return check_timer_down(timer, ref timer_flag);
        }
        return false;
    }

    public static bool check_timer_down(float timer,ref bool timer_flag)
    {
        if (timer < 0.0f)
        {
            timer = float.MaxValue;
            timer_flag = false;
            return true;
        }
        return false;
    }

    public static void start_timer_up(ref float timer, ref bool timer_flag)
    {
        timer_flag = true;
        timer = 0.0f;
    }

    public static bool up_ch_timer_up(ref float timer, ref bool timer_flag, float target_time, 
        float delta_time = -0.1f)
    {
        if (timer_flag)
        {
            if (delta_time < 0) { timer += Time.deltaTime; }
            else { timer += delta_time; }
            return check_timer_up(timer, ref timer_flag, target_time);
        }
        return false;
    }

    public static bool check_timer_up(float timer, ref bool timer_flag, float target_time)
    {
        if (timer >= target_time)
        {
            timer = float.MinValue;
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
                if (parameter.type == AnimatorControllerParameterType.Trigger)
                { animator.ResetTrigger(parameter.name); }
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

    public static Vector3 world_to_canvas(Vector3 target_pos,Camera camera,Canvas canvas)
    {
        Vector3 screenPos = camera.WorldToViewportPoint(target_pos);
        screenPos.x *= canvas.GetComponent<RectTransform>().rect.width;
        screenPos.x -= canvas.GetComponent<RectTransform>().rect.width / 2.0f;
        screenPos.y *= canvas.GetComponent<RectTransform>().rect.height;
        screenPos.y -= canvas.GetComponent<RectTransform>().rect.height / 2.0f;
        screenPos.z = 0.0f;

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

    public static IEnumerator show_obj(Transform target_TRANS,float time)
    {
        target_TRANS.GetComponent<MeshRenderer>().enabled = true;
        yield return new WaitForSeconds(time);
        target_TRANS.GetComponent<MeshRenderer>().enabled = false;
    }

    public static void restart_scene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public static Vector2[] get_perpen_vec(Vector2 origin)
    {
        return new Vector2[] { new Vector2(origin.y, -origin.x), new Vector2(-origin.y, origin.x) };
    }

    public static (int,int) seconds_to_time(int seconds)
    {
        int sec = seconds % 60;
        int min = seconds / 60;
        return (min < 0 ? 0 : min, sec < 0 ? 0 : sec);
    }

    /// <summary>
    /// Convert seconds to a time string;
    /// </summary>
    /// <param name="seconds"></param>
    /// <param name="str_type">0: "MIN:SEC"</param>
    /// <returns></returns>
    public static string seconds_to_time(int seconds,int str_type)
    {
        (int,int) timei = seconds_to_time(seconds);
        string res = "";
        string min_s = "", sec_s = "";
        switch(str_type)
        {
            case 0:
                if (timei.Item1 < 10) { min_s += "0"; }
                min_s += timei.Item1.ToString();
                res += min_s;
                res += ":";
                if (timei.Item2 < 10) { sec_s += "0"; }
                sec_s += timei.Item2.ToString();
                res += sec_s;
                break;
        }
        return res;
    }

    /// <summary>
    /// Generate the grid positions by rows and cols;
    /// </summary>
    /// <param name=""></param>
    /// <returns></returns>
    public static Vector2[] grid_generation(float height, float width, int rows, int cols, 
        float hori_gap, float vert_gap)
    {
        float total_height = rows * height + (rows - 1) * vert_gap;
        float total_width = cols * width + (cols - 1) * hori_gap;
        return grid_generation(total_width, total_height, rows, cols);
    }

    public static Vector2[] grid_generation(float hori_total_width, float vert_total_height, float width, 
        float height)
    {
        int rows = Mathf.FloorToInt(vert_total_height / height);
        int cols = Mathf.FloorToInt(hori_total_width / width);
        return grid_generation(hori_total_width, vert_total_height, rows, cols);
    }

    /// <summary>
    /// Generate the grid by total width and height;
    /// </summary>
    /// <param name="hori_total_width"></param>
    /// <param name="vert_total_height"></param>
    /// <param name="rows"></param>
    /// <param name="cols"></param>
    /// <returns></returns>
    public static Vector2[] grid_generation(float hori_total_width, float vert_total_height, int rows, int cols)
    {
        float right_board = hori_total_width / 2.0f;
        float up_board = vert_total_height / 2.0f;
        float x = 0.0f;
        float y = 0.0f;
        float hori_dist = 0.0f;
        float vert_dist = 0.0f;
        if (rows <= 1)
        {
            y = 0.0f;
        }
        else
        {
            vert_dist = vert_total_height / (float)(rows - 1);
        }
        if (cols <= 1)
        {
            x = 0.0f;
        }
        else
        {
            hori_dist = hori_total_width / (float)(cols - 1);
        }

        List<Vector2> poss = new List<Vector2>();
        Vector2 pos = new Vector2();
        for (int row = 0; row < rows; row++)
        {
            if (rows > 1)
            {
                y = -up_board + row * vert_dist;
            }
            for (int col = 0; col < cols; col++)
            {
                if (cols > 1)
                {
                    x = -right_board + col * hori_dist;
                }
                pos = new Vector3(x, y);
                poss.Add(pos);
            }
        }
        return poss.ToArray();
    }

    /// <summary>
    /// Calculate the size ratio by two Unity size;
    /// </summary>
    /// <param name="size"></param>
    /// <param name="target_size"></param>
    /// <returns></returns>
    public static Vector3 scale_cal(Vector2 size, Vector2 target_size)
    {
        Vector2 size_diff = target_size - size;
        Vector2 size_ratio = size_diff / size;
        return new Vector3(size_ratio.x + 1.0f, size_ratio.y + 1.0f, 1.0f);
    }

    public static Vector3 scale_cal(Vector3 size, Vector3 target_size)
    {
        Vector3 size_diff = target_size - size;
        float x = size_diff.x / size.x;
        float y = size_diff.y / size.y;
        float z = size_diff.z / size.z;
        Vector3 size_ratio = new Vector3(x, y, z);
        return size_ratio + new Vector3(1.0f, 1.0f, 1.0f);
    }

    /// <summary>
    /// Calculate the rectangles for the mesh; frame size as a square;
    /// </summary>
    /// <param name="tex_index"></param>
    /// <returns>{upper left point, down right point}</returns>
    public static Vector2[] mesh_size_cal(Texture2D texture, float frame_size)
    {
        Vector2[] poss = new Vector2[2];
        float h_w_ratio = (float)texture.height / (float)texture.width;
        float mesh_h = 0.0f, mesh_w = 0.0f;
        if (h_w_ratio >= 1.0f)
        {
            mesh_h = frame_size;
            mesh_w = frame_size / h_w_ratio;
        }
        else
        {
            mesh_h = frame_size * h_w_ratio;
            mesh_w = frame_size;
        }
        poss[0] = new Vector2(mesh_w / 2.0f * -1.0f, mesh_h / 2.0f);
        poss[1] = new Vector2(mesh_w / 2.0f, mesh_h / 2.0f * -1.0f);
        return poss;
    }

    /// <summary>
    /// Calculate the size by frame size and pixel per unity;
    /// </summary>
    /// <param name="texture"></param>
    /// <param name="frame_size"></param>
    /// <returns>({upper left point, down right point}, ratio)</returns>
    public static (Vector2[],float) mesh_size_cal_ratio(Texture2D texture, float frame_size, float PPU)
    {
        Vector2[] size = mesh_size_cal(texture, frame_size);
        float hori_size = (texture.width * (1 / PPU)) / 2.0f;
        float ratio = Mathf.Abs(size[0].x) / hori_size;
        return (size, ratio);
    }

    public static Sprite texture_to_sprite(Texture2D texture)
    {
        return Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), 
            new Vector2(0.5f, 0.5f));
    }

    public static int index_mod(int index, int mod_n)
    {
        int temp_index = index + (Math.Abs(index / mod_n) + 1) * mod_n;
        return temp_index % mod_n;
    }

    [Obsolete("Not for int")]
    public static T dict_find<T,T1>(Dictionary<T,T1> dict, T1 target) where T1 : class where T : class
    {
        foreach(KeyValuePair<T,T1> KV in dict)
        {
            if (KV.Value == target) { return KV.Key; }
        }
        return default(T);
    }

    /// <summary>
    /// Find key from value, works for int,int; Return INT32MIN if not found;
    /// </summary>
    /// <param name="dict"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    public static int dict_find_int(Dictionary<int, int> dict, int target)
    {
        foreach(KeyValuePair<int, int> KV in dict)
        {
            if (KV.Value == target) { return KV.Key; }
        }
        return Int32.MinValue;
    }

    public static void change_render_order(Transform transform, string render_layer = "", 
        int sorting_order = Int32.MinValue)
    {
        if (render_layer != "") 
        { transform.GetComponent<MeshRenderer>().sortingLayerName = render_layer; }
        if (sorting_order != Int32.MinValue)
        { transform.GetComponent<MeshRenderer>().sortingOrder = sorting_order; }
    }

    public static Vector3 random_pos_gener(Vector3 center, Vector3 range)
    {
        float x = UnityEngine.Random.Range(center.x - range.x, center.x + range.x);
        float y = UnityEngine.Random.Range(center.y - range.y, center.y + range.y);
        float z = UnityEngine.Random.Range(center.z - range.z, center.z + range.z);
        return new Vector3(x, y, z);
    }

    [Obsolete("Not implemented yet")]
    /// <summary>
    /// Check over lap, only for cubes;
    /// </summary>
    /// <param name="pos1"></param>
    /// <param name="pos2"></param>
    /// <param name="size"></param>
    /// <returns></returns>
    public static bool overlap_check_3D(Vector3 pos1, Vector3 pos2, Vector3 size)
    {
        return true;
    }
}
