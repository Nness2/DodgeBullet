using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneLimitations : MonoBehaviour
{
    /*private GameObject BlueField;
    private GameObject RedField;
    private GameObject BluePrison;
    private GameObject RedPrison;
    private GameObject NeutreField1;
    private GameObject NeutreField2;*/
    public CharacterController controller;
    public List<GameObject> BlueSpawnsZone;
    public List<GameObject> RedSpawnsZone;


    public bool teamBlue;
    public int state = 0;

    private Health health;

    private IEnumerator coroutine;
    // Start is called before the first frame update
    void Start()
    {

        health = GetComponent<Health>();


        /*BlueField = GameObject.FindGameObjectWithTag("BlueField");
        RedField = GameObject.FindGameObjectWithTag("RedField");
        BluePrison = GameObject.FindGameObjectWithTag("BluePrison");
        RedPrison = GameObject.FindGameObjectWithTag("RedPrison");
        NeutreField1 = GameObject.FindGameObjectWithTag("Neutre1");
        NeutreField2 = GameObject.FindGameObjectWithTag("Neutre2");*/
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider collision)
    {
        StopAllCoroutines();

        if (collision.gameObject.tag == "BlueField" && !teamBlue || collision.gameObject.tag == "BlueField" && teamBlue && state != 0)
        {
            coroutine = ZoneDamage(35);
            StartCoroutine(coroutine);
            //health.TakeDamage(35);
            //Debug.Log("35 Damages");
        }

        if (collision.gameObject.tag == "RedField" && teamBlue || collision.gameObject.tag == "RedField" && !teamBlue && state != 0)
        {
            coroutine = ZoneDamage(35);
            StartCoroutine(coroutine);
            //health.TakeDamage(35);
            //Debug.Log("35 Damages");
        }

        if (collision.gameObject.tag == "BluePrison" && !teamBlue || collision.gameObject.tag == "BluePrison" && teamBlue && state != 1)
        {
            coroutine = ZoneDamage(50);
            StartCoroutine(coroutine);
            //health.TakeDamage(50);
            //Debug.Log("50 Damages");
        }

        if (collision.gameObject.tag == "RedPrison" && teamBlue || collision.gameObject.tag == "RedPrison" && !teamBlue && state != 1)
        {
            coroutine = ZoneDamage(50);
            StartCoroutine(coroutine);
            //health.TakeDamage(50);
            //Debug.Log("50 Damages");
        }

        if (collision.gameObject.tag == "Neutre1" && state != 2 && teamBlue)
        {
            coroutine = ZoneDamage(20);
            StartCoroutine(coroutine);
            //health.TakeDamage(20);
            //Debug.Log("20 Damage");
        }

        if (collision.gameObject.tag == "Neutre2" && state != 3 && teamBlue)
        {
            coroutine = ZoneDamage(20);
            StartCoroutine(coroutine);
            //health.TakeDamage(20);
            //Debug.Log("20 Damage");
        }



        if (collision.gameObject.tag == "Neutre1" && state != 3 && !teamBlue)
        {
            coroutine = ZoneDamage(20);
            StartCoroutine(coroutine);
            //health.TakeDamage(20);
            //Debug.Log("20 Damage");
        }

        if (collision.gameObject.tag == "Neutre2" && state != 2 && !teamBlue)
        {
            coroutine = ZoneDamage(20);
            StartCoroutine(coroutine);
            //health.TakeDamage(20);
            //Debug.Log("20 Damage");
        }
        /*if ((collision.gameObject.tag == "BlueField" && state == 0 || collision.gameObject.tag == "BluePrison" && state == 1 || collision.gameObject.tag == "Neutre1" && state == 2 || collision.gameObject.tag == "Neutre2" && state == 3) && teamBlue)
        {
            StopAllCoroutines();
        }

        if ((collision.gameObject.tag == "BlueField" && state == 0 || collision.gameObject.tag == "BluePrison" && state == 1 || collision.gameObject.tag == "Neutre1" && state == 2 || collision.gameObject.tag == "Neutre2" && state == 3) && !teamBlue)
        {
            StopAllCoroutines();
        }*/

    }

    /*void OnTriggerExit(Collider collision)
    {
        StopAllCoroutines();
    }*/


        public void NextZone()
    {
        if (teamBlue)
        {
            controller.enabled = false;
            transform.position = BlueSpawnsZone[state].transform.position;
            transform.rotation = BlueSpawnsZone[state].transform.localRotation;
            controller.enabled = true;
        }

        if (!teamBlue)
        {
            controller.enabled = false;
            transform.position = RedSpawnsZone[state].transform.position;
            transform.rotation = RedSpawnsZone[state].transform.localRotation;
            controller.enabled = true;
        }

    }

    IEnumerator ZoneDamage(int damages)
    {
        while (true)
        {
            var wait = new WaitForSeconds(1f);
            health.TakeDamage(damages);
            yield return wait;
        }

    }

}
