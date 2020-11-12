using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ZoneLimitations : NetworkBehaviour
{
    /*private GameObject BlueField;
    private GameObject RedField;
    private GameObject BluePrison;
    private GameObject RedPrison;
    private GameObject NeutreField1;
    private GameObject NeutreField2;*/
    
    public CharacterController controller;
    private List<GameObject> BlueSpawnsZone = new List<GameObject>();
    private List<GameObject> RedSpawnsZone = new List<GameObject>();


    public bool teamBlue;
    [SyncVar(hook = nameof(OnChangeState))]
    public int state;

    private Health health;

    private IEnumerator coroutine;
    // Start is called before the first frame update
    void Start()
    {
        state = 0;
        if (isLocalPlayer)
        {
            health = transform.GetComponent<Health>();

            BlueSpawnsZone.Add(GameObject.FindGameObjectWithTag("BlueFieldSpawner"));
            BlueSpawnsZone.Add(GameObject.FindGameObjectWithTag("BluePrisonFieldSpawner"));
            BlueSpawnsZone.Add(GameObject.FindGameObjectWithTag("NeutreFieldSpawner1"));
            BlueSpawnsZone.Add(GameObject.FindGameObjectWithTag("NeutreFieldSpawner2"));

            RedSpawnsZone.Add(GameObject.FindGameObjectWithTag("RedFieldSpawner"));
            RedSpawnsZone.Add(GameObject.FindGameObjectWithTag("RedPrisonFieldSpawner"));
            RedSpawnsZone.Add(GameObject.FindGameObjectWithTag("NeutreFieldSpawner2"));
            RedSpawnsZone.Add(GameObject.FindGameObjectWithTag("NeutreFieldSpawner1"));
        }

        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider collision)
    {
        if (!isLocalPlayer)
            return;

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

    
    public void UpdateZone()
    {
        if (!isLocalPlayer)
            return;
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

    void OnChangeState(int oldValue, int newValue)
    {
        state = newValue;
    }



}
