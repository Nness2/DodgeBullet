using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class StartTimer : MonoBehaviour
{

    float currentTime = 0f;
    float startingTime = 3.49f;

    private Text countDownText;

    public bool top;
    void Start()
    {
        top = false;
        countDownText = GameObject.FindGameObjectWithTag("TimerText").GetComponent<Text>();
        var countDownScript = GameObject.FindGameObjectWithTag("TimerText").GetComponent<StartTimer>();
        currentTime = startingTime;
        countDownText.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!top)
            return;

        currentTime -= 1 * Time.deltaTime;
        countDownText.GetComponent<Text>().text = currentTime.ToString("0");

        if (currentTime <= 0)
        {
            currentTime = 0;
            gameObject.GetComponent<Text>().enabled = false;
            gameObject.GetComponent<StartTimer>().enabled = false;
            gameObject.GetComponent<StartTimer>().enabled = false;
            countDownText.enabled = false ;
            GameObject.FindGameObjectWithTag("StartWall").SetActive(false);
        }
    }


}
