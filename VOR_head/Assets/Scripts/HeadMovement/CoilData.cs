using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System;
using System.Net;
using System.Threading;

public class CoilData : MonoBehaviour {

    public Quaternion currentHeadOrientation { get; set; }
    public Vector3 currentHeadVelocity { get; set; }
    public UInt32 simulinkSample { get; set; }

    //static information;
    private const int port = 5123;
    private static IPAddress addr = new IPAddress(new byte[] { 192, 168, 2, 130 });
    private bool stopListening;
    private static UdpClient udpClient = new UdpClient(port);
    private static IPEndPoint EP = new IPEndPoint(addr, port);
    //members;
    private Thread RCThread;

    // Use this for initialization
    void Start () {
        this.currentHeadOrientation = new Quaternion();
        this.currentHeadVelocity = new Vector3();
        this.simulinkSample = (UInt32)0;

        RCThread = new Thread(read_coil);
        RCThread.Start();
    }
	
	// Update is called once per frame
	void Update () {

    }

    private void read_coil()
    {
        //int port = 5123;
        //IPAddress addr = new IPAddress(new byte[] { 192, 168, 2, 130 });
        //stopListening = false;
        //udpClient = new UdpClient(port);
        //IPEndPoint EP = new IPEndPoint(addr, port);
        try
        {
            // read quaternions
            while (!stopListening)
            {
                Debug.Log("read_coil");
                byte[] receiveBytes = udpClient.Receive(ref EP);

                int offset = 0;
                while (offset + 32 <= receiveBytes.Length)
                {
                    // process orientation (16 bytes)
                    float s = BitConverter.ToSingle(receiveBytes, offset);
                    float w = BitConverter.ToSingle(receiveBytes, offset + 4);
                    float x = BitConverter.ToSingle(receiveBytes, offset + 8);
                    float y = BitConverter.ToSingle(receiveBytes, offset + 12);
                    float z = BitConverter.ToSingle(receiveBytes, offset + 16);
                    float vx = BitConverter.ToSingle(receiveBytes, offset + 20);
                    float vy = BitConverter.ToSingle(receiveBytes, offset + 24);
                    float vz = BitConverter.ToSingle(receiveBytes, offset + 28);
                    offset += 32;

                    currentHeadOrientation = new Quaternion(x, y, z, w);
                    currentHeadVelocity = new Vector3(vx, vy, vz);
                    simulinkSample = (UInt32)s;
                }

                Debug.Log("currentHeadOrientation1 " + currentHeadOrientation);
                Debug.Log("currentHeadVelocity1 " + currentHeadVelocity);
                Debug.Log("simulinkSample1 " + simulinkSample);
            }
        }
        catch (Exception e)
        {
            Debug.Log(e);
            Debug.Log("[polhemus] PlStream terminated in PlStream::read_liberty()");
            Console.WriteLine("[polhemus] PlStream terminated in PlStream::read_liberty().");
        }
        finally
        {
            udpClient.Close();
            udpClient = null;
        }
    }

    private void OnApplicationQuit()
    {
        try
        {
            // signal shutdown
            stopListening = true;

            // attempt to join for 500ms
            if (!RCThread.Join(500))
            {
                // force shutdown
                RCThread.Abort();
                if (udpClient != null)
                {
                    udpClient.Close();
                    udpClient = null;
                }
            }
        }
        catch (Exception e)
        {
            Debug.Log(e);
            Debug.Log("[polhemus] PlStream was unable to close the connection thread upon application exit. This is not a critical exception.");
        }
    }
}
