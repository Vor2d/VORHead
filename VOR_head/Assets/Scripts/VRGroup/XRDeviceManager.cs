using UnityEngine.XR;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class XRDeviceManager
{
    public static InputDevice HeadNode { get; set; }
    public static InputDevice LeftControllerNode { get; set; }
    public static InputDevice RightControllerNode { get; set; }
    public static XRInputSubsystem Subsystem { get; set; }

    public static bool Inited { get; set; } = false;

    public static void InitXRDevice()
    {
        if (Inited) { return; }
        HeadNode = init_device(XRNode.Head);
        LeftControllerNode = init_device(XRNode.LeftHand);
        RightControllerNode = init_device(XRNode.RightHand);
        Subsystem = get_subsys();

        Inited = true;
    }

    public static void UpdateXRDevice()
    {
        Inited = false;
        InitXRDevice();
    }

    private static InputDevice init_device(UnityEngine.XR.XRNode node)
    {
        var devices = new List<UnityEngine.XR.InputDevice>();
        try { UnityEngine.XR.InputDevices.GetDevicesAtXRNode(node, devices); }
        catch (Exception e) { Debug.LogError("GetDevicesAtXRNode Error! " + e); }


        if (devices.Count == 1)
        {
            Debug.Log(string.Format("Device name '{0}' with role '{1}'",
                devices[0].name, devices[0].characteristics.ToString()));
            return devices[0];
        }
        else if (devices.Count > 1)
        {
            Debug.LogError(string.Format("Found more than one node from '{0}'!", node.ToString()));
        }
        return new InputDevice();
    }

    private static XRInputSubsystem get_subsys()
    {
        List<XRInputSubsystem> XISs = new List<XRInputSubsystem>();
        try { SubsystemManager.GetInstances<XRInputSubsystem>(XISs); }
        catch (Exception e) { Debug.LogError("GetInstances<XRInputSubsystem> Error! " + e); }

        if (XISs.Count == 1)
        {
            Debug.Log("Found XRInputSubsystem " + XISs[0].ToString());
            return XISs[0];
        }
        else if (XISs.Count > 1)
        {
            Debug.LogError("Found more than one node from XRInputSubsystem!");
        }
        else if(XISs.Count == 0)
        {
            Debug.LogError("No XRInputSubsystem found!");
        }
        return new XRInputSubsystem();
    }

    public static float get_trigger(InputDevice device)
    {
        float trigger = 0.0f;
        device.TryGetFeatureValue(CommonUsages.trigger, out trigger);
        return trigger;
    }

    public static float get_trigger(XRNode node)
    {
        float trigger = 0.0f;
        switch (node)
        {
            case XRNode.LeftHand:
                LeftControllerNode.TryGetFeatureValue(CommonUsages.trigger, out trigger);
                break;
            case XRNode.RightHand:
                RightControllerNode.TryGetFeatureValue(CommonUsages.trigger, out trigger);
                break;
        }

        return trigger;
    }

    public static bool get_xbutton(InputDevice device)
    {
        bool xbutton = false;
        device.TryGetFeatureValue(CommonUsages.primaryButton, out xbutton);
        return xbutton;
    }

    public static bool get_ybutton(InputDevice device)
    {
        bool ybutton = false;
        device.TryGetFeatureValue(CommonUsages.secondaryButton, out ybutton);
        return ybutton;
    }

    public static bool get_xbutton(XRNode node)
    {
        bool xbutton = false;
        switch (node)
        {
            case XRNode.LeftHand:
                LeftControllerNode.TryGetFeatureValue(CommonUsages.primaryButton, out xbutton);
                break;
            case XRNode.RightHand:
                RightControllerNode.TryGetFeatureValue(CommonUsages.primaryButton, out xbutton);
                break;
        }
        return xbutton;
    }

    public static bool get_ybutton(XRNode node)
    {
        bool ybutton = false;
        switch (node)
        {
            case XRNode.LeftHand:
                LeftControllerNode.TryGetFeatureValue(CommonUsages.secondaryButton, out ybutton);
                break;
            case XRNode.RightHand:
                RightControllerNode.TryGetFeatureValue(CommonUsages.secondaryButton, out ybutton);
                break;
        }
        return ybutton;
    }

    public static InputDevice get_device(XRNode node)
    {
        InputDevice device = new InputDevice();
        switch (node)
        {
            case XRNode.Head:
                device = HeadNode;
                break;
            case XRNode.LeftHand:
                device = LeftControllerNode;
                break;
            case XRNode.RightHand:
                device = RightControllerNode;
                break;
        }
        return device;
    }

    public static float get_JS_horri(InputDevice device)
    {
        return get_JS_2d(device).x;
    }

    public static float get_JS_vert(InputDevice device)
    {
        return get_JS_2d(device).y;
    }

    /// <summary>
    /// Horizontal then vertical, left: -1, right: 1, down: -1, up 1;
    /// </summary>
    /// <returns></returns>
    public static Vector2 get_JS_2d(InputDevice device)
    {
        Vector2 axis2d = new Vector2();
        device.TryGetFeatureValue(CommonUsages.primary2DAxis,out axis2d);
        return axis2d;
    }

    public static void recenter()
    {
        Subsystem.TryRecenter();
    }

    public static Vector3 get_device_Angularspeed(XRNode node)
    {
        switch(node)
        {
            case XRNode.Head:
                return get_head_Angularspeed();
            case XRNode.LeftHand:
                return get_device_Angularspeed(LeftControllerNode);
            case XRNode.RightHand:
                return get_device_Angularspeed(RightControllerNode);
        }
        return Vector3.zero;
    }

    public static Vector3 get_head_Angularspeed()
    {
        return get_device_Angularspeed(HeadNode);
    }

    public static Vector3 get_device_Angularspeed(InputDevice device)
    {
        Vector3 AV = new Vector3();
        device.TryGetFeatureValue(CommonUsages.deviceAngularVelocity, out AV);
        AV *= Mathf.Rad2Deg;
        return AV;
    }

    public static Quaternion get_device_Orientation(XRNode node)
    {
        switch (node)
        {
            case XRNode.Head:
                return get_head_Orientation();
            case XRNode.LeftHand:
                return get_Orientation(LeftControllerNode);
            case XRNode.RightHand:
                return get_Orientation(RightControllerNode);
        }
        return Quaternion.identity;
    }

    public static Quaternion get_head_Orientation()
    {
        Quaternion ori = new Quaternion();
        HeadNode.TryGetFeatureValue(CommonUsages.deviceRotation, out ori);
        return ori;
    }

    public static Quaternion get_Orientation(InputDevice device)
    {
        Quaternion ori = new Quaternion();
        device.TryGetFeatureValue(CommonUsages.deviceRotation, out ori);
        return ori;
    }
}
