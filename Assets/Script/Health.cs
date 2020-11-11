using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

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
        
        if (!isLocalPlayer)
            return false;
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

}
