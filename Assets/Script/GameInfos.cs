﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Mirror;
using System.Text.RegularExpressions;

public class GameInfos : NetworkBehaviour
{
    enum Color : int{ None, Blue, Red};

    public int teamSize;

    public int selfColor;
    [SyncVar(hook = nameof(OnChangeSelfName))]
    public string selfName;

    public Canvas canvas;

    public Button blueButton;
    public Button redButton;

    public Text blueText;
    public Text redText;
    public Text blueKda;
    public Text redKda;

    private string display = "";

    //[SyncVar(hook = nameof(OnChangeBlueList))]
    public List<GameObject> BlueTeam;
    //[SyncVar(hook = nameof(OnChangeRedList))]
    public List<GameObject> RedTeam;

    public List<string> BlueTeamKda;
    public List<string> RedTeamKda;

    private bool callMe;

    public bool teamsReady;
    private bool nameChecked;

    private GameObject GameMng;

    private GameObject StartImage;
    // Use this for initialization
    void Start()
    {
        GameMng = GameObject.FindGameObjectWithTag("GameManager");
        //StartImage = GameObject.FindGameObjectWithTag("startImage");
        nameChecked = false;
        teamSize = 1;
        selfColor = (int)Color.None;
        teamsReady = false;
        BlueTeam = new List<GameObject>();
        RedTeam = new List<GameObject>();
        
        callMe = true;
        selfName = GameObject.FindGameObjectWithTag("name").GetComponent<SaveName>().PlayerName;
    }




    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer)
            return;


        if (nameChecked)
        {
            checkSimilarName();
            nameChecked = false;
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            canvas.gameObject.SetActive(!canvas.gameObject.activeSelf);
            GetComponent<FullControl>().MouseLock(!canvas.gameObject.activeSelf);
        }

        if (callMe && !teamsReady)
        {
            AddText();
            callMe = false;
            if (BlueTeam.Count >= teamSize && RedTeam.Count >= teamSize)
            {
                teamsReady = true;
                if (isServer)
                    GameMng.GetComponent<StartManager>().InteratableEnable();

            }
            CmdCallColorManager();
        }

        if (BlueTeam.Count >= teamSize || selfColor == (int)Color.Blue)
            blueButton.GetComponent<Button>().interactable = false;
        else
            blueButton.GetComponent<Button>().interactable = true;

        if (RedTeam.Count >= teamSize || selfColor == (int)Color.Red)
            redButton.GetComponent<Button>().interactable = false;
        else
            redButton.GetComponent<Button>().interactable = true;

        //CHANGETHAT
        AddKda();

    }






    void AddText()
    {
        
        display = "";
        foreach (GameObject obj in BlueTeam)
        {
            var msg = obj.GetComponent<GameInfos>().selfName;
            display = display.ToString() + msg.ToString() + "\n";
        }
        blueText.text = display;

        display = "";
        foreach (GameObject obj in RedTeam)
        {
            var msg = obj.GetComponent<GameInfos>().selfName;
            display = display.ToString() + msg.ToString() + "\n";
        }
        redText.text = display;

    }

    void AddKda()
    {

        display = "";
        foreach (GameObject msg in BlueTeam)
        {
            int kill = msg.GetComponent<Stats>().selfKill;
            int death = msg.GetComponent<Stats>().selfDeath;
            display = display.ToString() + kill.ToString() + " / " + death.ToString() + "\n";
        }
        blueKda.text = display;

        display = "";
        foreach (GameObject msg in RedTeam)
        {
            int kill = msg.GetComponent<Stats>().selfKill;
            int death = msg.GetComponent<Stats>().selfDeath;
            display = display.ToString() + kill.ToString() + " / " + death.ToString() + "\n";

        }
        redKda.text = display;

    }

    #region Unity addName
    public void addMyName(bool isBlue)
    {
        if (!isLocalPlayer)
            return;

        //Premier join team on ferme le canvas
        if (selfColor == (int)Color.None)
        {
            canvas.gameObject.SetActive(false);
            GetComponent<FullControl>().MouseLock(true);
        }

        //On dit à l'object la couleur dont il correspond
        if (isBlue)
            selfColor = (int)Color.Blue;
        if (!isBlue)
            selfColor = (int)Color.Red;


        /*if (isBlue && BlueTeam.Count >= 3)
            return;
        if (!isBlue && RedTeam.Count >= 3)
            return;
           */
        CmdaddMyName(isBlue, gameObject, selfColor);
    }


    [Command]
    public void CmdaddMyName(bool isBlue, GameObject selfObj, int targetColor)
    {

        GameObject[] characters = GameObject.FindGameObjectsWithTag("MainCharacter");

        foreach (GameObject child in characters)
        {
            if (child.GetComponent<FullControl>().isLocal)
            {
                var gameInfos = child.GetComponent<GameInfos>();

                removeChangeTeam(gameInfos.BlueTeam, gameInfos.RedTeam, targetColor, selfObj);

                if (isBlue)
                {
                    gameInfos.BlueTeam.Add(selfObj);
                }
                else
                {
                    gameInfos.RedTeam.Add(selfObj);
                }


                gameInfos.callMe = true;
                ClientNamePropagate(gameInfos.BlueTeam, gameInfos.RedTeam);

            }
        }
    }



    #endregion


    #region Unity getName
    public void addGetNames()
    {
        if (!isLocalPlayer)
            return;
        //string name = GameObject.FindGameObjectWithTag("name").GetComponent<SaveName>().PlayerName;
        CmdGetNames();
        
    }


    [Command]
    public void CmdGetNames()
    {
        GameObject[] characters = GameObject.FindGameObjectsWithTag("MainCharacter");
        foreach (GameObject child in characters)
        {
            if (child.GetComponent<FullControl>().isLocal)
            {
                var gameinfo = child.GetComponent<GameInfos>();
                TargetGetNames(gameinfo.BlueTeam, gameinfo.RedTeam);
            }
        }


    }

    [TargetRpc]
    public void TargetGetNames(List<GameObject> newBlueTeam, List<GameObject> newRedTeam)
    {
        GameObject[] characters = GameObject.FindGameObjectsWithTag("MainCharacter");
        foreach (GameObject child in characters)
        {
            if (child.GetComponent<FullControl>().isLocal)
            {
                var gameinfo = child.GetComponent<GameInfos>();
                gameinfo.BlueTeam = newBlueTeam;
                gameinfo.RedTeam = newRedTeam;
                gameinfo.callMe = true;
                gameinfo.nameChecked = true;
            }
        }

    }
    #endregion

    [ClientRpc]
    public void ClientNamePropagate(List<GameObject> newBlueTeam, List<GameObject> newRedTeam)
    {
        //if (!isServer)
        //    return;

        GameObject[] characters = GameObject.FindGameObjectsWithTag("MainCharacter");
        foreach (GameObject child in characters)
        {
            if (child.GetComponent<FullControl>().isLocal)
            {
                var gameinfo = child.GetComponent<GameInfos>();
                gameinfo.BlueTeam = newBlueTeam;
                gameinfo.RedTeam = newRedTeam;
                gameinfo.callMe = true;
            }

        }
        //GetComponent<FullControl>().ColorManager();

    }

    void removeChangeTeam(List<GameObject> newBlueTeam, List<GameObject> newRedTeam, int color, GameObject name)
    {
        

        if (color == (int)Color.Blue)
        {
            newRedTeam.Remove(name);
            //TargetAttributeListPosition(newRedTeam.Count - 1);
        }

        if (color == (int)Color.Red)
        {
            newBlueTeam.Remove(name);
            //TargetAttributeListPosition(newBlueTeam.Count - 1);
        }

    }

    void checkSimilarName()
    {
        int similarNbr = 0;

        foreach (GameObject child in BlueTeam)
        {
            var name = child.GetComponent<GameInfos>().selfName;

            if (Regex.IsMatch(name, @"^" + selfName + " [0-9]+") || Regex.IsMatch(name, @"^" + selfName))
            {
                similarNbr++;
            }



        }
        foreach (GameObject child in RedTeam)
        {
            var name = child.GetComponent<GameInfos>().selfName;

            if (Regex.IsMatch(name, @"^" + selfName + " [0-9]+") || Regex.IsMatch(name, @"^" + selfName))
            {
                similarNbr++;
            }
        }
        if (similarNbr > 0)
        {
            //selfName = selfName + " " + similarNbr;
            CmdUpDateName(selfName + " " + similarNbr);
        }



    }

    [Command]
    void CmdCallColorManager()
    {
        ClientCallColorManager();
    }

    [ClientRpc]
    void ClientCallColorManager()
    {
        GameObject[] characters = GameObject.FindGameObjectsWithTag("MainCharacter");

        foreach (GameObject child in characters)
        {
            if(child.GetComponent<FullControl>().isLocal)
                GetComponent<FullControl>().ColorManager();
        }
    }

    void OnChangeSelfName(string oldValue, string newValue)
    {
        selfName = newValue;
    }
    [Command] //Appelé par le client mais lu par le serveur
    public void CmdUpDateName(string name)
    {
        selfName = name;
    }

}