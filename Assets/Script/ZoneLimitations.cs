﻿using System.Collections;
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

    public int DeadStateNumber;

    [SyncVar(hook = nameof(OnChangeTeam))]
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

            
            BlueSpawnsZone.Add(GameObject.FindGameObjectsWithTag("LobbyFieldSpawner"));
            BlueSpawnsZone.Add(GameObject.FindGameObjectsWithTag("BlueFieldSpawner"));
            BlueSpawnsZone.Add(GameObject.FindGameObjectsWithTag("BluePrisonFieldSpawner"));
            BlueSpawnsZone.Add(GameObject.FindGameObjectsWithTag("NeutreFieldSpawner1"));
            BlueSpawnsZone.Add(GameObject.FindGameObjectsWithTag("NeutreFieldSpawner2"));

            RedSpawnsZone.Add(GameObject.FindGameObjectsWithTag("LobbyFieldSpawner"));
            RedSpawnsZone.Add(GameObject.FindGameObjectsWithTag("RedFieldSpawner"));
            RedSpawnsZone.Add(GameObject.FindGameObjectsWithTag("RedPrisonFieldSpawner"));
            RedSpawnsZone.Add(GameObject.FindGameObjectsWithTag("NeutreFieldSpawner2"));
            RedSpawnsZone.Add(GameObject.FindGameObjectsWithTag("NeutreFieldSpawner1"));

            DeadStateNumber = BlueSpawnsZone.Count;
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

    }

    /*void OntriggerExit(Collider collision)
    {
        if (!isLocalPlayer)
            return;


        StopCoroutine(coroutine);
        Debug.Log("exit");
    }*/

    void OnTriggerEnter(Collider collision)
    {
        if (!isLocalPlayer)
            return;

        if (coroutine != null)
            StopCoroutine(coroutine);

                    
        if (controller.enabled == true)
        {
            if (collision.gameObject.tag == "BlueField" && !teamBlue || collision.gameObject.tag == "BlueField" && teamBlue && state != 1)
            {
                coroutine = ZoneDamage(35);
                StartCoroutine(coroutine);
            }
            else if (collision.gameObject.tag == "RedField" && teamBlue || collision.gameObject.tag == "RedField" && !teamBlue && state != 1)
            {
                coroutine = ZoneDamage(35);
                StartCoroutine(coroutine);
            }

            else if (collision.gameObject.tag == "BluePrison" && !teamBlue || collision.gameObject.tag == "BluePrison" && teamBlue && state != 2)
            {
                coroutine = ZoneDamage(35);
                StartCoroutine(coroutine);
            }

            else if (collision.gameObject.tag == "RedPrison" && teamBlue || collision.gameObject.tag == "RedPrison" && !teamBlue && state != 2)
            {
                coroutine = ZoneDamage(35);
                StartCoroutine(coroutine);
            }

            else if (collision.gameObject.tag == "Neutre1" && state != 3 && teamBlue)
            {
                coroutine = ZoneDamage(20);
                StartCoroutine(coroutine);
            }

            else if (collision.gameObject.tag == "Neutre2" && state != 4 && teamBlue)
            {
                coroutine = ZoneDamage(20);
                StartCoroutine(coroutine);
            }



            else if (collision.gameObject.tag == "Neutre1" && state != 4 && !teamBlue)
            {
                coroutine = ZoneDamage(20);
                StartCoroutine(coroutine);
            }

            else if (collision.gameObject.tag == "Neutre2" && state != 3 && !teamBlue)
            {
                coroutine = ZoneDamage(20);
                StartCoroutine(coroutine);
            }

            else if (collision.gameObject.tag == "CornerField")
            {
                coroutine = ZoneDamage(35);
                StartCoroutine(coroutine);
            }
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


        var GIScript = GetComponent<GameInfos>();
        //foreach (string blueChild in child.GetComponent<GameInfos>().BlueTeam)
        if (teamBlue)
        {
            for (int i = 0; i < GIScript.BlueTeam.Count; i++)
            {
                if (GIScript.BlueTeam[i].GetComponent<FullControl>().PlayerID == GetComponent<FullControl>().PlayerID)
                {
                    if (BlueSpawnsZone.Count > state)
                    {
                        controller.enabled = false;
                        //transform.position = BlueSpawnsZone[state][i].transform.position;
                        transform.rotation = BlueSpawnsZone[state][i].transform.localRotation;
                        StartCoroutine(UpDownAnim(transform.position, BlueSpawnsZone[state][i].transform.position, 0.3f));
                        //Debug.Log("Tab" + BlueSpawnsZone[state][i].transform.position);
                    }

                }
            }
        }
                //foreach (string redChild in child.GetComponent<GameInfos>().RedTeam)
        else //if (!teamBlue)
        {
            for (int i = 0; i < GIScript.RedTeam.Count; i++)
            {
                if (GIScript.RedTeam[i].GetComponent<FullControl>().PlayerID == GetComponent<FullControl>().PlayerID)
                {
                    if (RedSpawnsZone.Count > state)
                    {
                        controller.enabled = false;
                        //transform.position = RedSpawnsZone[state][i].transform.position;
                        transform.rotation = RedSpawnsZone[state][i].transform.localRotation;
                        StartCoroutine(UpDownAnim(transform.position, RedSpawnsZone[state][i].transform.position, 0.3f));
                        //StartCoroutine(UpDownAnim(transform.position, RedSpawnsZone[state][i].transform.position, 1f));
                    }
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
    IEnumerator UpDownAnim(Vector3 startPos, Vector3 Gotoposition, float waitTime)
    {
        float elapsedTime = 0;

        while (elapsedTime < waitTime)
        {
            transform.position = Vector3.Lerp(startPos, Gotoposition, (elapsedTime / waitTime));
            elapsedTime += Time.deltaTime;

            // Yield here
            yield return new WaitForEndOfFrame();
        }
        controller.enabled = true;
    }

    IEnumerator ZoneDamage(int damages)
    {
        while (true)
        {
            var wait = new WaitForSeconds(1f);
            bool kill = health.TakeDamage(damages);
            if (kill)
            {
                GetComponent<Health>().KillManager(-1, GetComponent<FullControl>().PlayerID);//, false, false);
                UpState();
                //UpdateZone();
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
    public void CmdInitState()
    {
        state = 0;
    }

    public void InitState()
    {
        state = 0;
    }

    [Command] //Appelé par le client mais lu par le serveur
    public void CmdDownState()
    {
        if (state == 1)
            return;
        state--;
        //DownState();
    }
    
    public void DownState()
    {
        if (state == 1)
            return;
        state--;
    }

    void OnChangeState(int oldValue, int newValue)
    {
        state = newValue;
        UpdateZone();
    }

    void OnChangeTeam(bool oldValue, bool newValue)
    {
        teamBlue = newValue;
    }

}
