using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Mirror;

public class GameInfos : NetworkBehaviour
{
    enum Color : int{ None, Blue, Red};

    public int teamSize;

    public int selfColor;
    public int selfPosition;
    public string selfName;
    public string realName;

    public Canvas canvas;

    public Button blueButton;
    public Button redButton;

    public Text blueText;
    public Text redText;

    private string display = "";

    //[SyncVar(hook = nameof(OnChangeBlueList))]
    public List<string> BlueTeam;
    //[SyncVar(hook = nameof(OnChangeRedList))]
    public List<string> RedTeam;

    private bool callMe;

    private bool teamsReady;

    private bool nameChecked;

    // Use this for initialization
    void Start()
    {
        nameChecked = false;
        teamSize = 3;
        selfPosition = -1;
        selfColor = (int)Color.None;
        teamsReady = false;
        BlueTeam = new List<string>();
        RedTeam = new List<string>();
        
        callMe = true;
        selfName = GameObject.FindGameObjectWithTag("name").GetComponent<SaveName>().PlayerName;
        realName = GameObject.FindGameObjectWithTag("name").GetComponent<SaveName>().PlayerName;

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
            canvas.gameObject.SetActive(!canvas.gameObject.activeSelf);

        if (callMe && !teamsReady)
        {
            AddText();
            callMe = false;
            if (BlueTeam.Count >= teamSize && RedTeam.Count >= teamSize)
            {
                teamsReady = true;
            }
        }

        if (BlueTeam.Count >= teamSize || selfColor == (int)Color.Blue)
            blueButton.GetComponent<Button>().interactable = false;
        else
            blueButton.GetComponent<Button>().interactable = true;

        if (RedTeam.Count >= teamSize || selfColor == (int)Color.Red)
            redButton.GetComponent<Button>().interactable = false;
        else
            redButton.GetComponent<Button>().interactable = true;


    }




    void OnChangeBlueList(List<string> oldValue, List<string> newValue)
    {
        BlueTeam = newValue;
    }
    void OnChangeRedList(List<string> oldValue, List<string> newValue)
    {
        RedTeam = newValue;
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

    #region Unity addName
    public void addMyName(bool isBlue)
    {
        if (!isLocalPlayer)
            return;

        //Premier join team on ferme le canvas
        if (selfColor == (int)Color.None)
            canvas.gameObject.SetActive(false);

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
        CmdaddMyName(isBlue, selfName, selfColor, selfPosition);
    }


    [Command]
    public void CmdaddMyName(bool isBlue, string name, int targetColor, int targetPos)
    {


        GameObject[] characters = GameObject.FindGameObjectsWithTag("MainCharacter");

        foreach (GameObject child in characters)
        {
            if (child.GetComponent<FullControl>().isLocal)
            {
                var gameInfos = child.GetComponent<GameInfos>();

                removeChangeTeam(gameInfos.BlueTeam, gameInfos.RedTeam, targetColor, name);

                if (isBlue)
                {
                    gameInfos.BlueTeam.Add(name);
                    //gameInfos.selfPosition = gameInfos.BlueTeam.Count;
                    TargetAttributeListPosition(gameInfos.BlueTeam.Count);
                }
                else
                {
                    gameInfos.RedTeam.Add(name);
                    //gameInfos.selfPosition = gameInfos.RedTeam.Count;
                    TargetAttributeListPosition(gameInfos.RedTeam.Count);

                }


                gameInfos.callMe = true;
                ClientNamePropagate(gameInfos.BlueTeam, gameInfos.RedTeam);

            }
        }
    }

    [ClientRpc]
    public void ClientaddMyName(List<string> newBlueTeam, List<string> newRedTeam)
    {

        BlueTeam = newBlueTeam;
        RedTeam = newRedTeam;
        callMe = true;
    }

    [TargetRpc]
    public void TargetAttributeListPosition(int pos)
    {
        GameObject[] characters = GameObject.FindGameObjectsWithTag("MainCharacter");
        foreach (GameObject child in characters)
        {
            if (child.GetComponent<FullControl>().isLocal)
            {
                var gameinfo = child.GetComponent<GameInfos>();
                gameinfo.selfPosition = pos;

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
    public void TargetGetNames(List<string> newBlueTeam, List<string> newRedTeam)
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
    public void ClientNamePropagate(List<string> newBlueTeam, List<string> newRedTeam)
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
    }

    void removeChangeTeam(List<string> newBlueTeam, List<string> newRedTeam, int color, string name)
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

        foreach (string child in BlueTeam)
        {
            if (child == realName)
            {
                similarNbr++;
            }
        }
        foreach (string child in RedTeam)
        {
            if (child == realName)
            {
                similarNbr++;
            }
        }
        if (similarNbr > 0)
            selfName = selfName + " " + similarNbr;
    }
}