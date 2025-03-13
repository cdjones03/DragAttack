using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class WebSocketClient : MonoBehaviour
{
    UdpClient udpClient;
    IPEndPoint remoteEndPoint;
    Thread receiveThread;

    public int port = 12345; // Same as Arduino's UDP port

    void Start()
    {
        remoteEndPoint = new IPEndPoint(IPAddress.Any, port);
        udpClient = new UdpClient(port);

        receiveThread = new Thread(new ThreadStart(ReceiveData));
        receiveThread.IsBackground = true;
        receiveThread.Start();

        Debug.Log("Listening for UDP packets on port " + port);
    }

    void ReceiveData()
    {
        while (true)
        {
            try
            {
                byte[] data = udpClient.Receive(ref remoteEndPoint);
                string message = Encoding.ASCII.GetString(data);
                Debug.Log("Received: " + message);
            }
            catch (Exception e)
            {
                Debug.LogError("Error receiving data: " + e.Message);
            }
        }
    }

    void OnDestroy()
    {
        if (receiveThread != null)
        {
            receiveThread.Abort();
        }

        if (udpClient != null)
        {
            udpClient.Close();
        }
    }
}
