﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.XR;
using System.IO;

public static class GeneralMethods {

    private static char[] file_spliter = new char[] { ' ', '\t' };
    private enum Direction { left, right };

    //y is horizontal; x is vertical;
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

    public static Vector3 RealToVirtual(float player_screen_cm, float screen_width_cm, 
                                        float rot_degreex,float rot_degreey)
    {
        float screen_htow_scale = (float)Screen.height / (float)Screen.width;
        float screen_height_cm = screen_htow_scale * screen_width_cm;
        float virtual_rotex = 0.0f;
        float virtual_rotey = 0.0f;

        virtual_rotex = RealToVirtualx(player_screen_cm, screen_height_cm, rot_degreex);
        virtual_rotey = RealToVirtualy(player_screen_cm, screen_width_cm, rot_degreey);

        return new Vector3(virtual_rotex, virtual_rotey, 0.0f);
    }

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

    public static void load_turn_jump_data_general(string path, 
                                        out List<float> turn_data, out List<float> jump_data)
    {
        turn_data = new List<float>();
        jump_data = new List<float>();
        Debug.Log("Loading file " + path);
        try
        {
            StreamReader reader = new StreamReader(path);
            while (!reader.EndOfStream)
            {


                //Turn degrees '\t' jump degrees;
                string[] splitstr = reader.ReadLine().Split(file_spliter);
                turn_data.Add(float.Parse(splitstr[0]));
                jump_data.Add(float.Parse(splitstr[1]));
            }
            reader.Close();

        }
        catch (Exception e)
        {
            Debug.Log("Reading file error! " + e);
        }
        Debug.Log("Loading complete! ");
    }

    //Monitor Change Position General Method;
    private static float DestinationCal(float init_dist,float ang_deg, int direc)
    {
        if((ang_deg+90)%180 == 0)
        {
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

    public static Vector3 PositionCal(float init_dist, float ang_degX, float ang_degY,
                                                                    int direcX, int direcY)
    {
        return new Vector3(DestinationCal(init_dist,ang_degX,direcX),
                            DestinationCal(init_dist,ang_degY,direcY),
                            init_dist);
    }


}
