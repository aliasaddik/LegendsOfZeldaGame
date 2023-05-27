using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int maxHealth = 70;
    public int currentHealth;
    UIHealthBar healthBar;
    bool die;
    void Start()
    {
        maxHealth = 70;
        currentHealth = maxHealth;
        healthBar = GetComponentInChildren<UIHealthBar>();
    }

    void Update()
    {
        
    }

    public void onAttack(int damage)
    {
        currentHealth -= damage;
        Debug.Log(((float)currentHealth / (float)maxHealth));
        healthBar.setHealthBarPercentage(((float)currentHealth / (float)maxHealth));
        if(currentHealth <= 0)
        {
            die = true;
            healthBar.gameObject.SetActive(false);
        }

    }
}
