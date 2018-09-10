
using UnityEngine;
using System.Net.Sockets;
using System;
using System.Net;
using System.Threading;
using System.IO;

public class CoilStream : MonoBehaviour
{
    // port used for our UDP connection
    public int port = 5123;

    /*
    // tracker descriptors
    public PlTracker tracker_type = PlTracker.Liberty;
    public int max_systems = 1;
    public int max_sensors = 1;

    // slots used to store tracker output data
    public bool[] active;
    public uint[] digio;
    public Vector3[] positions; */
    public Vector4 orientation;
    public Vector3 angularVelocity;
    public UInt32 simulinkSample;

    // internal state
    // private int max_slots;

    private UdpClient udpClient;
    private Thread conThread;
    private bool stopListening;

    //external management
    CoilController coilController;

    // Use this for initialization
    void Awake()
    {
        Application.targetFrameRate = 240;
        try
        {
            coilController = GetComponent<CoilController>();
            orientation = Vector4.zero;  // is this needed?
            conThread = new Thread(new ThreadStart(read_coil));

            // start the read thread
            conThread.Start();
        }
        catch (Exception e)
        {
            Debug.Log(e);
            Debug.Log("[polhemus] PlStream terminated in PlStream::Awake().");
            Console.WriteLine("[polhemus] PlStream terminated in PlStream::Awake().");
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    // read thread
    private void read_coil()
    {
        IPAddress addr = new IPAddress(new byte[] { 192, 168, 2, 50 });
        stopListening = false;
        udpClient = new UdpClient(port);
        IPEndPoint EP = new IPEndPoint(addr, port);

        try
        {

            // read quaternions
            while (!stopListening)
            {
                byte[] receiveBytes = udpClient.Receive(ref EP);

                int offset = 0;
                while (offset + 32 <= receiveBytes.Length)  
                {
                    // process orientation (16 bytes)
                    float s = BitConverter.ToSingle(receiveBytes, offset);
                    float w = BitConverter.ToSingle(receiveBytes, offset +4 );
                    float x = BitConverter.ToSingle(receiveBytes, offset + 8);
                    float y = BitConverter.ToSingle(receiveBytes, offset + 12);
                    float z = BitConverter.ToSingle(receiveBytes, offset + 16);
                    float vx = BitConverter.ToSingle(receiveBytes, offset + 20);
                    float vy = BitConverter.ToSingle(receiveBytes, offset + 24);
                    float vz = BitConverter.ToSingle(receiveBytes, offset + 28);
                    offset += 32;

                    orientation = new Vector4(w, x, y, z);
                    angularVelocity = new Vector3(vx, vy, vz);
                    simulinkSample = (UInt32)s;
                }

                coilController.VelocityThread2(this);
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
    // cleanup
    private void OnApplicationQuit()
    {
        try
        {
            // signal shutdown
            stopListening = true;

            // attempt to join for 500ms
            if (!conThread.Join(500))
            {
                // force shutdown
                conThread.Abort();
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

    public void OnPlStreamQuit()
    {
        try
        {
            // signal shutdown
            stopListening = true;

            // attempt to join for 500ms
            if (!conThread.Join(500))
            {
                // force shutdown
                conThread.Abort();
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