using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MyNetWorkManager : NetworkManager
{
    // Start is called before the first frame update
    public override void OnStartServer()
    {
        Debug.Log("Server Started");
        //base.OnStartServer();
    }

    public override void OnStopServer()
    {
        Debug.Log("Server Stopped");
        //base.OnStopServer();
    }

    public override void OnClientConnect(NetworkConnection conn)
    {
        Debug.Log("Connected to Server");
        //base.OnClientConnect(conn);
    }

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        Debug.Log("Disconnected to Server");
        //base.OnClientDisconnect(conn);
    }
}
