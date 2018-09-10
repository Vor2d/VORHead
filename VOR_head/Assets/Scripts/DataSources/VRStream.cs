using UnityEngine;
using System;
using System.Threading;

public class VRStream : MonoBehaviour
{
    // internal state
    // private int max_slots;
    private Thread conThread;
    private bool appstart = false;
    //external management
    VRController vrController;
    //DataStream dataStream;

    // Use this for initialization
    void Awake()
    {
        Application.targetFrameRate = 240;
        vrController = GetComponent<VRController>();
        appstart = true;
        conThread = new Thread(read_vr);

        // start the read thread
        conThread.Start();
        //Debug.Log("1");
    }

    // Update is called once per frame
    void Update()
    {

    }

    // read thread
    private void read_vr()
    {
        try
        {
            while (appstart)
            {
                vrController.VelocityThread2();
            }
            //Debug.Log("1");
        }
        catch (Exception e)
        {
            Debug.Log(e);
            Debug.Log("[VR] VRStream terminated in VRStream::read_VR()");
            Console.WriteLine("[VR] VRStream terminated in VRStream::read_VR().");
        }
    }
    // cleanup
    private void OnApplicationQuit()
    {
        conThread.Abort();
        appstart = false;
        Debug.Log("[VR] VRStream shutdown.");
    }
    public void OnPlStreamQuit()
    {
        try
        {
            conThread.Abort();
        }
        catch (Exception e)
        {
            Debug.Log(e);
            Debug.Log("[VR] PlStream was unable to close the connection thread upon application exit. This is not a critical exception.");
        }
    }
}
