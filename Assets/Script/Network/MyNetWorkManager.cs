using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MyNetWorkManager : NetworkManager
{
    public struct CreateMMOCharacterMessage : NetworkMessage
    {
        public string _name;
    }

    public override void OnStartServer()
    {

        base.OnStartServer();
        NetworkServer.RegisterHandler<CreateMMOCharacterMessage>(OnCreateCharacter);
        Debug.Log("Server is opened");
    }

    public override void OnStopServer()
    {
        Debug.Log("Server Stopped");
    }

    public override void OnClientConnect(NetworkConnection conn)
    {
        // you can send the message here, or wherever else you want
        base.OnClientConnect(conn);

        string name;
        if (GameObject.FindGameObjectWithTag("name") != null)
            name = GameObject.FindGameObjectWithTag("name").GetComponent<SaveName>().PlayerName;
        else
        {
            name = "TestMode";
        }

        // you can send the message here, or wherever else you want
        CreateMMOCharacterMessage characterMessage = new CreateMMOCharacterMessage
        {
            _name = name,
        };

        conn.Send(characterMessage);

        Debug.Log("Connected to Server");

    }

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        Debug.Log("Disconnected to Server");
    }

    void OnCreateCharacter(NetworkConnection conn, CreateMMOCharacterMessage message)
    {
        // playerPrefab is the one assigned in the inspector in Network
        // Manager but you can use different prefabs per race for example
        GameObject gameobject = Instantiate(playerPrefab);

        // Apply data from the message however appropriate for your game
        // Typically Player would be a component you write with syncvars or properties
        var player = gameobject.GetComponent<GameInfos>();
        player.selfName = message._name;

        // call this to use this gameobject as the primary controller
        NetworkServer.AddPlayerForConnection(conn, gameobject);
    }
}
