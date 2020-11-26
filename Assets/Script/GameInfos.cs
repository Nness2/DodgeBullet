using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameInfos : MonoBehaviour
{


    public Text blueText;
    public Text redText;

    private string display = "";

    List<string> BlueTeam;
    List<string> RedTeam;

    private bool callMe;



    // Use this for initialization
    void Start()
    {
        BlueTeam = new List<string>();
        RedTeam = new List<string>();

        BlueTeam.Add("this ");
        BlueTeam.Add(" is ");
        RedTeam.Add(" a ");
        RedTeam.Add(" test ");
        callMe = true;

    }

    // Update is called once per frame
    void Update()
    {
        if (callMe)
        {
            AddText();
            callMe = false;
        }
    }

    void AddText()
    {
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


}