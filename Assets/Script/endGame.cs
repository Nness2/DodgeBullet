using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;
using UnityEngine.SceneManagement;

public class endGame : MonoBehaviour
{
    public Button QuitBtn;
    public Button AgainBtn;
    public Text QuitText;
    public Text AgainText;
    public Image WinImg;
    public Image Losemg;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void displayEndGame(bool victory)
    {
        QuitBtn.GetComponent<Image>().enabled = true;
        AgainBtn.GetComponent<Image>().enabled = true;
        QuitText.GetComponent<Text>().enabled = true;
        AgainText.GetComponent<Text>().enabled = true;

        if (victory)
            WinImg.GetComponent<Image>().enabled = true;
        else
            Losemg.GetComponent<Image>().enabled = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        GameObject.FindGameObjectWithTag("cinemachineCamera").GetComponent<CinemachineFreeLook>().enabled = false;
        gameObject.transform.parent.gameObject.GetComponent<GameInfos>().canvas.gameObject.SetActive(true);
        gameObject.transform.parent.gameObject.GetComponent<GameInfos>().enabled = false;
        gameObject.transform.parent.gameObject.GetComponent<FullControl>().enabled = false;
        gameObject.transform.parent.gameObject.GetComponent<AnimationStateControler>().enabled = false;


    }


    public void ExitGame()
    {
        SceneManager.LoadScene("Menu");
    }

    public void WaitHost()
    {

    }


    public void ReplayGame()
    {
        StopAllCoroutines();

        //gameObject.transform.parent.GetComponent<ZoneLimitations>().InitState();
        gameObject.transform.parent.GetComponent<ZoneLimitations>().CmdInitState();



        QuitBtn.GetComponent<Image>().enabled = false;
        AgainBtn.GetComponent<Image>().enabled = false;
        QuitText.GetComponent<Text>().enabled = false;
        AgainText.GetComponent<Text>().enabled = false;
        WinImg.GetComponent<Image>().enabled = false;
        Losemg.GetComponent<Image>().enabled = false;

        var player = gameObject.transform.parent;
        var FC = player.gameObject.GetComponent<FullControl>();

        player.GetComponent<GameInfos>().enabled = true;
        player.GetComponent<FullControl>().enabled = true;
        player.GetComponent<AnimationStateControler>().enabled = true;
        FC.dead = false;
        FC.InGame = false;
        FC.CmdBackToLobby(FC.PlayerID);

        GameObject.FindGameObjectWithTag("cinemachineCamera").GetComponent<CinemachineFreeLook>().enabled = true;


        /*
        GameObject lobbySpawn = GameObject.FindGameObjectWithTag("LobbyFieldSpawner");
        FC.controller.enabled = false;
        gameObject.transform.parent.transform.position = lobbySpawn.transform.position;
        gameObject.transform.parent.transform.rotation = lobbySpawn.transform.localRotation;
        FC.controller.enabled = true;
        */

        GameObject[] characters = GameObject.FindGameObjectsWithTag("MainCharacter");

        foreach (GameObject child in characters)
        {
            child.GetComponent<Stats>().ResetStats();
        }
        var GI = player.GetComponent<GameInfos>();
        GI.callKda = true;

        //GI.BlueTeam = new List<GameObject>();
        //GI.RedTeam = new List<GameObject>();
        //GI.selfColor = 0;

        GI.callMe = true;

        var Hlth = gameObject.transform.parent.GetComponent<Health>();
        Hlth.currentHealth = 100;
        Hlth.healthBar.sizeDelta = new Vector2(Hlth.currentHealth, Hlth.healthBar.sizeDelta.y);
        var gameManager = GameObject.FindGameObjectWithTag("GameManager");
        gameManager.GetComponent<GameManager>().firstKill = false;
        var SM = gameManager.GetComponent<StartManager>();
        SM.ShowStartButton();
        
        //FC.UpdateDeadCam();
        //FC.CmdDisplayPlayer(FC.PlayerID, true);
        //var TimerText = GameObject.FindGameObjectWithTag("TimerText");
        //TimerText.GetComponent<StartTimer>().InitTimer();
        //TimerText.GetComponent<StartTimer>().top = false;

        StopAllCoroutines();

    }


    
}
