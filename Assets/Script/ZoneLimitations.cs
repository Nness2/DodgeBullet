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
    private List<GameObject[]> BlueSpawnsZone = new List<GameObject[]>();
    private List<GameObject[]> RedSpawnsZone = new List<GameObject[]>();

    [SyncVar(hook = nameof(OnChangeState))]
    public bool teamBlue;
    [SyncVar(hook = nameof(OnChangeState))]
    public int state;
    private int currentState;
    private Health health;

    private IEnumerator coroutine;


    // Start is called before the first frame update
    void Start()
    {
        state = 0;
        currentState = 0;
        if (isLocalPlayer)
        {
            health = transform.GetComponent<Health>();

            BlueSpawnsZone.Add(GameObject.FindGameObjectsWithTag("BlueFieldSpawner"));
            BlueSpawnsZone.Add(GameObject.FindGameObjectsWithTag("BluePrisonFieldSpawner"));
            BlueSpawnsZone.Add(GameObject.FindGameObjectsWithTag("NeutreFieldSpawner1"));
            BlueSpawnsZone.Add(GameObject.FindGameObjectsWithTag("NeutreFieldSpawner2"));

            RedSpawnsZone.Add(GameObject.FindGameObjectsWithTag("RedFieldSpawner"));
            RedSpawnsZone.Add(GameObject.FindGameObjectsWithTag("RedPrisonFieldSpawner"));
            RedSpawnsZone.Add(GameObject.FindGameObjectsWithTag("NeutreFieldSpawner2"));
            RedSpawnsZone.Add(GameObject.FindGameObjectsWithTag("NeutreFieldSpawner1"));
        }

        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void FixedUpdate()
    {
        if (!isLocalPlayer)
            return;

        if (currentState != state)
        {
            UpdateZone();
            currentState = state;
        }
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

        var GIScript = GetComponent<GameInfos>();
        //foreach (string blueChild in child.GetComponent<GameInfos>().BlueTeam)
        if (teamBlue)
        {
            for (int i = 0; i < GIScript.BlueTeam.Count; i++)
            {
                if (GIScript.BlueTeam[i].GetComponent<GameInfos>().selfName == GIScript.selfName)
                {
                    controller.enabled = false;
                    transform.position = BlueSpawnsZone[state][i].transform.position;
                    transform.rotation = BlueSpawnsZone[state][i].transform.localRotation;
                    controller.enabled = true;
                }
            }
        }
                //foreach (string redChild in child.GetComponent<GameInfos>().RedTeam)
        else //if (!teamBlue)
        {
            for (int i = 0; i < GIScript.RedTeam.Count; i++)
            {
                if (GIScript.RedTeam[i].GetComponent<GameInfos>().selfName == GIScript.selfName)
                {
                    controller.enabled = false;
                    transform.position = RedSpawnsZone[state][i].transform.position;
                    transform.rotation = RedSpawnsZone[state][i].transform.localRotation;
                    controller.enabled = true;
                }
            }
        }
    }
        /*
        if (teamBlue)
        {
            controller.enabled = false;
            transform.position = BlueSpawnsZone[state][0].transform.position;
            transform.rotation = BlueSpawnsZone[state][0].transform.localRotation;
            controller.enabled = true;
        }

        if (!teamBlue)
        {
            controller.enabled = false;
            transform.position = RedSpawnsZone[state][0].transform.position;
            transform.rotation = RedSpawnsZone[state][0].transform.localRotation;
            controller.enabled = true;
        }*/
    

    IEnumerator ZoneDamage(int damages)
    {
        while (true)
        {
            var wait = new WaitForSeconds(1f);
            bool kill = health.TakeDamage(damages);
            if (kill)
            {
                UpState();
                ///upState permet une synchronisation mais probleme de zone chez les rouges, voir si on peut se contenter d'un simple incrémentation, peut etre ajouter un rst
                //state++;
                //UpdateZone();
            }
                
            yield return wait;
        }

    }

    [Command] //Appelé par le client mais lu par le serveur
    public void UpState()
    {
        state++;
    }

    
    [Command] //Appelé par le client mais lu par le serveur
    public void CmdDownState()
    {
        DownState();
    }
    
    [TargetRpc] 
    public void DownState()
    {
        state--;
    }

    void OnChangeState(int oldValue, int newValue)
    {
        state = newValue;
    }

    void OnChangeState(bool oldValue, bool newValue)
    {
        teamBlue = newValue;
    }

}
