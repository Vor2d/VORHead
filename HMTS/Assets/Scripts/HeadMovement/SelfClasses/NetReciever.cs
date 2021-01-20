using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System;
using TMPro;
using System.IO.Ports;

public class NetReciever : MonoBehaviour
{
    private string com = "COM5";
    private int brate = 9600;
    private int timeOut = 50;
    private SerialPort stream;

    public static NetReciever IS;

    private void Awake()
    {
        IS = this;
    }

    void Start()
    {
        stream = new SerialPort(com, brate);
        stream.ReadTimeout = timeOut;
        stream.Open();
        
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    public void WriteToArduino(string message)
    {
        stream.WriteLine(message);
        Debug.Log(message);
        stream.BaseStream.Flush();
    }

    
}
