using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;
using DodgeBullet;

public class Stats : NetworkBehaviour
{

    // Start is called before the first frame update
    [SyncVar(hook = nameof(OnChangekill))]
    public int selfKill;
    [SyncVar(hook = nameof(OnChangeDeath))]
    public int selfDeath;
    [SyncVar(hook = nameof(OnChangeBallTouch))]
    public int selfBallTouch;
    [SyncVar(hook = nameof(OnChangeDamage))]
    public int selfDamage;

    [SerializeField] private IntVariable _upZone;
    [SerializeField] private IntVariable _downZone;

    void Start()
    {
        selfKill = 0;
        selfDeath = 0;
        selfBallTouch = 0;
        selfDamage = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [Command]
    public void CmdUpdateStats(int kill, int death, int ballTouch, int damage)
    {
        selfKill = kill;
        selfDeath = death;
        selfBallTouch = ballTouch;
        selfDamage = damage;
    }

    void displayStats()
    {
        if (!isLocalPlayer)
            return;
        GameObject killText = GameObject.FindGameObjectWithTag("selfKill");
        GameObject deathText = GameObject.FindGameObjectWithTag("selfDeath");
        killText.GetComponent<Text>().text = selfKill.ToString();
        deathText.GetComponent<Text>().text = selfDeath.ToString();
    }

    void OnChangekill(int oldValue, int newValue)
    {
        selfKill = newValue;
        displayStats();
        if (isLocalPlayer)
        {
            if (selfKill % 2 == 0)
            {
                _upZone.Value++;
            }
        }
        if (isLocalPlayer)
        {
            if (selfKill % 5 == 0)
            {
                _downZone.Value++;
            }
        }
    }

    void OnChangeDeath(int oldValue, int newValue)
    {
        selfDeath = newValue;
        displayStats();
    }

    void OnChangeBallTouch(int oldValue, int newValue)
    {
        selfBallTouch = newValue;
    }

    void OnChangeDamage(int oldValue, int newValue)
    {
        selfDamage = newValue;
    }


    public void ResetStats()
    {
        selfKill = 0;
        selfDeath = 0;
        selfBallTouch = 0;
        selfDamage = 0;
    }
}
