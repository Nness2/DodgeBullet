using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using Cinemachine;

public class Health : NetworkBehaviour
{

    public const int maxHealth = 100;
    public int currentHealth = maxHealth;

    public RectTransform healthBar;

    void Start()
    {

    }

    //take Damage
    public bool TakeDamage(int amount) //return true si il y a kill
    {
        //if (!isLocalPlayer)
        //return false;
        bool isDead = false;
        var ZL = GetComponent<ZoneLimitations>();
        currentHealth -= amount;
        healthBar.sizeDelta = new Vector2(currentHealth, healthBar.sizeDelta.y);

        if (currentHealth <= 0)
        {

            //ZL.state++;
            if(ZL.state > 3)//2) //state est up après, il faut anticiper de 1, attention à l'utilisation du stateDown et du state--
            {
                var FC = GetComponent<FullControl>();
                //FC.UpdateDeadCam();
                //FC.CmdDisplayPlayer(FC.selfNumber, false);
                GetComponent<ZoneLimitations>().CmdInitState();
                FC.dead = true;
                FC.InGame = false;
                FC.CmdDeadPlayer(FC.PlayerID);
                return false;
            }
            currentHealth = 100;
            //ZL.UpdateZone();
            //Debug.Log("Dead");

            isDead = true;
        }

        healthBar.sizeDelta = new Vector2(currentHealth, healthBar.sizeDelta.y);
        if (isDead)
            return true;

        return false;
    }

    [Command]
    void DestroyPlayer(GameObject player)
    {
        Destroy(player);
    }

    /*public void UpZone() 
    {

        //if (!isLocalPlayer)
        //    return;
        Debug.Log("moins");
        var ZL = GetComponent<ZoneLimitations>();

        currentHealth = 100;
        ZL.state--;
        ZL.UpdateZone();
    }*/
    

    public void KillManager(int killer, int killed, bool firstKill)  // à appeler quand il y a une mecanique de kills
    {
        if (!isLocalPlayer)
            return;
        
        CmdKillNotification(killer, killed, firstKill);
    }

    //On peut modifier la valeur d'un local en modiffient son player depuis le serveur.
    [Command] //Appelé par le client mais lu par le serveur
    void CmdKillNotification(int killer, int killed, bool firstKill) 
    {
        //propagateInfos(killer, killed);
        Debug.Log("Killer = " + killer + " - Killed = " + killed);
        if (firstKill)
            ClientFirstKill(killer);
        GameObject[] characters = GameObject.FindGameObjectsWithTag("MainCharacter");

        foreach (GameObject child in characters)
        {
            if (child.GetComponent<FullControl>().PlayerID == killed)
            {
                child.GetComponent<Stats>().selfDeath++;
                child.GetComponent<GameInfos>().callKda = true;
            }

            if (child.GetComponent<FullControl>().PlayerID == killer)
            {
                child.GetComponent<Stats>().selfKill++;
                child.GetComponent<GameInfos>().callKda = true;

                if (!isServer)
                    return;
                child.GetComponent<ZoneLimitations>().DownState();
                //child.GetComponent<ZoneLimitations>().UpdateZone();

            }
            //Debug.Log(child.GetComponent<FullControl>().selfNumber);
        }
    }

    [ClientRpc] 
    void ClientFirstKill(int killer)  
    {
        GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().firstKill = true;

        GameObject[] characters = GameObject.FindGameObjectsWithTag("MainCharacter");

        foreach (GameObject child in characters)
        {
            if (child.GetComponent<FullControl>().PlayerID == killer)
            {
                child.GetComponent<FullControl>().GotBall = true;
            }
        }
    }

}
