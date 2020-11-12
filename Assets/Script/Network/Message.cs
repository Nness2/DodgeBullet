using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class MyMessage : MessageBase
{
    public NetworkInstanceId netId;
    public int stuff;
}

public class Player : NetworkBehaviour
{
    short MyMsgId = 1000;

    public override void OnStartClient()
    {
        // this should be somewhere else..
        NetworkManager.singleton.client.RegisterHandler(MyMsgId, OnMyMsg);
    }

    [Command]
    void CmdSendToMe()
    {
        var msg = new MyMessage();
        msg.stuff = 2456986;
        msg.netId = netId;

        base.connectionToClient.Send(MyMsgId, msg);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            CmdSendToMe();
        }
    }

    void DoStuff(int stuff)
    {
        Debug.Log("Got msg " + stuff + " for " + gameObject);
    }

    static void OnMyMsg(NetworkMessage netMsg)
    {
        var msg = netMsg.ReadMessage<MyMessage>();
        var player = ClientScene.FindLocalObject(msg.netId);
        player.GetComponent<Player>().DoStuff(msg.stuff);
    }
}