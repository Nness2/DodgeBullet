using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{

    public const int maxHealth = 100;
    public int currentHealth = maxHealth;

    public RectTransform healthBar;

    void Start()
    {
        
    }

    //take Damage
    public void TakeDamage(int amount)
    {
        var ZL = GetComponent<ZoneLimitations>();
        currentHealth -= amount;

        if (currentHealth <= 0)
        {
            ZL.state++;
            currentHealth = 100;
            ZL.NextZone();
            Debug.Log("Dead");
        }

        healthBar.sizeDelta = new Vector2(currentHealth, healthBar.sizeDelta.y);
    }

}
