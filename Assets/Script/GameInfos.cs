using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Mirror;

public class GameInfos : NetworkBehaviour
{


    public Text blueText;
    public Text redText;

    private string display = "";

    public List<string> BlueTeam;
    public List<string> RedTeam;

    private bool callMe;



    // Use this for initialization
    void Start()
    {
        BlueTeam = new List<string>();
        RedTeam = new List<string>();
        
        callMe = true;

    }

    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer)
            return;

        if (callMe)
        {
            AddText();
            callMe = false;
        }
    }

    void AddText()
    {
        display = "";
        foreach (string msg in BlueTeam)
        {
            display = display.ToString() + msg.ToString() + "\n";
        }
        blueText.text = display;

        display = "";
        foreach (string msg in RedTeam)
        {
            display = display.ToString() + msg.ToString() + "\n";
        }
        redText.text = display;
    }


    public void addMyName(bool isBlue)
    {
        if (!isLocalPlayer)
            return;
        string name = GameObject.FindGameObjectWithTag("name").GetComponent<SaveName>().PlayerName;
        CmdaddMyName(isBlue, name);
    }


    [Command]
    public void CmdaddMyName(bool isBlue, string name)
    {


        GameObject[] characters = GameObject.FindGameObjectsWithTag("MainCharacter");

        foreach (GameObject child in characters)
        {
            var gameInfos = child.GetComponent<GameInfos>();
            if (gameInfos != null)
            {
                if (isBlue)
                    gameInfos.BlueTeam.Add(name);
                else
                    gameInfos.RedTeam.Add(name);
                gameInfos.callMe = true;
            }
            //ClientaddMyName(isBlue, name);

        }
    }

    [ClientRpc]
    public void ClientaddMyName(bool isBlue, string name)
    {
        
        if (isBlue)
            BlueTeam.Add(name);
        else
            RedTeam.Add(name);

        callMe = true;

    }

}