using UnityEngine;
using System.Net.Sockets;
using System;
using System.Net;
using System.Threading;

public class CoilData : MonoBehaviour {

    //public EC_GameController ECGC_script;

    public Quaternion currentHeadOrientation { get; set; }
    public Vector3 currentHeadVelocity { get; set; }
    public UInt32 simulinkSample { get; set; }
    public Vector2 Left_eye_voltage { get; set; }   //horizontal then vertical;
    public Vector2 Right_eye_voltage { get; set; }
    public float[] Raw_data { get; set; }

    //static information;
    private const int port = 5123;
    private static IPAddress addr;
    private bool stopListening;
    private static UdpClient udpClient;
    private static IPEndPoint EP;
    //members;
    private Thread RCThread;

    public static CoilData IS;

    private void Awake()
    {
        try
        {
            addr = new IPAddress(new byte[] { 192, 168, 2, 130 });
            udpClient = new UdpClient(port);
            EP = new IPEndPoint(addr, port);
        }
        catch(Exception e)
        {
            Debug.Log(e);
        }

        IS = this;
    }

    // Use this for initialization
    void Start () {
        this.currentHeadOrientation = new Quaternion();
        this.currentHeadVelocity = new Vector3();
        this.simulinkSample = (UInt32)0;
        this.Raw_data = new float[6];

        this.RCThread = new Thread(read_coil);

        RCThread.Start();
    }

    private void read_coil()
    {
        try
        {
            // read quaternions
            while (!stopListening)
            {
                //Debug.Log("read_coil");
                byte[] receiveBytes = udpClient.Receive(ref EP);

                int offset = 0;
                while (offset + 72 <= receiveBytes.Length)
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
                    float rh = BitConverter.ToSingle(receiveBytes, offset + 32);  // right eye horizontal
                    float rv = BitConverter.ToSingle(receiveBytes, offset + 36);  // right eye vertical
                    float lh = BitConverter.ToSingle(receiveBytes, offset + 40);  // left eye horizontal
                    float lv = BitConverter.ToSingle(receiveBytes, offset + 44);  // left eye vertical
                    float hdx = BitConverter.ToSingle(receiveBytes, offset + 48);
                    float hdy = BitConverter.ToSingle(receiveBytes, offset + 52);
                    float hdz = BitConverter.ToSingle(receiveBytes, offset + 56);
                    float htx = BitConverter.ToSingle(receiveBytes, offset + 60);
                    float hty = BitConverter.ToSingle(receiveBytes, offset + 64);
                    float htz = BitConverter.ToSingle(receiveBytes, offset + 68);
                    offset += 72;

                    currentHeadOrientation = new Quaternion(x, y, z, w);
                    currentHeadVelocity = new Vector3(vx, vy, vz);
                    simulinkSample = (UInt32)s;
                    Left_eye_voltage = new Vector2(lh, lv);
                    Right_eye_voltage = new Vector2(rh, rv);
                    Raw_data = new float[]{hdx,hdy,hdz,htx,hty,htz};
                }

                //Debug.Log("currentHeadOrientation1 " + currentHeadOrientation);
                //Debug.Log("currentHeadVelocity1 " + currentHeadVelocity);
                //Debug.Log("simulinkSample1 " + simulinkSample);
            }
        }
        catch (Exception e)
        {
            Debug.Log(e);
            Debug.Log("read coil terminated");
            //Console.WriteLine("[polhemus] PlStream terminated in PlStream::read_liberty().");
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
            Debug.Log("unable to close the connection thread upon application exit. This is not a critical exception.");
        }
    }

    private void OnDestroy()
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
            Debug.Log("unable to close the connection thread upon application exit. This is not a critical exception.");
        }
    }
}
