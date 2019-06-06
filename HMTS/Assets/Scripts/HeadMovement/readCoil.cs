// read thread
//private void read_coil()
//{
//    int port = 5123;
//    IPAddress addr = new IPAddress(new byte[] { 192, 168, 2, 130 });
//    stopListening = false;
//    udpClient = new UdpClient(port);
//    IPEndPoint EP = new IPEndPoint(addr, port);

//    try
//    {
//        // read quaternions
//        while (!stopListening)
//        {
//            byte[] receiveBytes = udpClient.Receive(ref EP);

//            int offset = 0;
//            while (offset + 32 <= receiveBytes.Length)
//            {
//                // process orientation (16 bytes)
//                float s = BitConverter.ToSingle(receiveBytes, offset);
//                float w = BitConverter.ToSingle(receiveBytes, offset + 4);
//                float x = BitConverter.ToSingle(receiveBytes, offset + 8);
//                float y = BitConverter.ToSingle(receiveBytes, offset + 12);
//                float z = BitConverter.ToSingle(receiveBytes, offset + 16);
//                float vx = BitConverter.ToSingle(receiveBytes, offset + 20);
//                float vy = BitConverter.ToSingle(receiveBytes, offset + 24);
//                float vz = BitConverter.ToSingle(receiveBytes, offset + 28);
//                offset += 32;

//                currentHeadOrientation = new Quaternion(x, y, z, w);
//                currentHeadVelocity = new Vector3(vx, vy, vz);
//                simulinkSample = (UInt32)s;
//            }
//        }
//    }
//    catch (Exception e)
//    {
//        Debug.Log(e);
//        Debug.Log("[polhemus] PlStream terminated in PlStream::read_liberty()");
//        Console.WriteLine("[polhemus] PlStream terminated in PlStream::read_liberty().");
//    }
//    finally
//    {
//        udpClient.Close();
//        udpClient = null;
//    }
//}
