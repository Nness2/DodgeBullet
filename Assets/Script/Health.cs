using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

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
        //    return false;
        //Debug.Log("test");
        bool isDead = false;
        var ZL = GetComponent<ZoneLimitations>();
        currentHealth -= amount;

        if (currentHealth <= 0)
        {
            ZL.state++;
            if(ZL.state > 3)
            {
                Destroy(transform.gameObject);
                return false;
            }
            currentHealth = 100;
            ZL.UpdateZone();
            //Debug.Log("Dead");
            isDead = true;
        }

        healthBar.sizeDelta = new Vector2(currentHealth, healthBar.sizeDelta.y);
        if (isDead)
            return true;

        return false;
    }

    public void UpZone() 
    {

        //if (!isLocalPlayer)
        //    return;
        Debug.Log("moins");
        var ZL = GetComponent<ZoneLimitations>();

        currentHealth = 100;
        ZL.state--;
        ZL.UpdateZone();
    }
    

    public void KillManager(int killer, int killed)
    {
        if (!isLocalPlayer)
            return;

        CmdKillNotification(killer, killed);
    }

    
    [Command] //Appelé par le client mais lu par le serveur
    void CmdKillNotification(int killer, int killed)
    {
        propagateInfos(killer, killed);
    }

    [ClientRpc] 
    void propagateInfos(int killer, int killed)
    {
        Debug.Log("Killer = " + killer + " - Killed = " + killed);
    }
    
}
