using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class StartTimer : MonoBehaviour
{

    float currentTime = 0f;
    float startingTime = 3.49f;

    private Text countDownText;

    public GameObject StartWall;
    private GameObject Wall;

    void Start()
    {

        Quaternion wallRot = Quaternion.Euler(0, 0, 90);
        Wall = Instantiate(StartWall, new Vector3(0, 0, 0), wallRot);

        countDownText = GameObject.FindGameObjectWithTag("TimerText").GetComponent<Text>();
        //var countDownScript = GameObject.FindGameObjectWithTag("TimerText").GetComponent<StartTimer>();
        currentTime = startingTime;
        //countDownText.enabled = false;
        
        GameObject.FindGameObjectWithTag("GameManager").GetComponent<StartManager>().InteratableDesable();
        GameObject.FindGameObjectWithTag("GameManager").GetComponent<StartManager>().HideStartButton();
        

    }

    // Update is called once per frame
    void Update()
    {
        currentTime -= 1 * Time.deltaTime;
        countDownText.GetComponent<Text>().text = currentTime.ToString("0");

        if (currentTime <= 0)
        {
            /*currentTime = 0;
            gameObject.GetComponent<Text>().enabled = false;
            gameObject.GetComponent<StartTimer>().enabled = false;
            countDownText.enabled = false ;*/

            Destroy(gameObject);
            Destroy(Wall);

            //gameObject.transform.parent.gameObject.transform.parent.GetComponent<ZoneLimitations>().UpState();


        }
    }


}
