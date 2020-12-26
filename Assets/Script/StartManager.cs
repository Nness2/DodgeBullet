using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartManager : MonoBehaviour
{

    public Image ImageStartEvent;
    public bool StartButtonInteratable;
    public bool PlayerReady;
    [SerializeField] Text countDownText;
    // Start is called before the first frame update
    void Start()
    {
           // bool StartButtonInteratable = false;
           // bool PlayerReady = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Return) && StartButtonInteratable)
        {
            StartGame();
        }
    }

    public void ShowStartButton()
    {
        ImageStartEvent.enabled = true;
    }

    public void HideStartButton()
    {
        ImageStartEvent.enabled = false;
    }

    public void InteratableEnable()
    {
        StartButtonInteratable = true;

        Color c = ImageStartEvent.color;
        c.a = 1f;
        ImageStartEvent.color = c;
    }

    public void InteratableDesable()
    {
        StartButtonInteratable = false;

        Color c = ImageStartEvent.color;
        c.a = 0.35f;
        ImageStartEvent.color = c;
    }

    public void StartGame()
    {
        //InteratableDesable();
        //HideStartButton();

        GameObject[] characters = GameObject.FindGameObjectsWithTag("MainCharacter");
        foreach (GameObject child in characters)
        {
            var FCScript = child.GetComponent<FullControl>();
            if (FCScript.isLocal)
            {
                //FCScript.TeamManager();
                child.GetComponent<FullControl>().CmdStartTimer();
            }
        }
    }

    
}
