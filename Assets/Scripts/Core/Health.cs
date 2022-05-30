using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour, IDamageable
{

    [SerializeField, Header("HEALTH")] private int maxHealth = 4;
    [SerializeField] private int currentHealth;

    public int GetCurrentHealth => currentHealth;
    public int GetMaxHealth => maxHealth;

    private void Awake()
    {
        currentHealth = maxHealth;
    }



    public void TakeDamage(int amount)
    {
        currentHealth -= amount;

        if (this.gameObject.tag == "Player")
        {
            GetComponent<PlayerController>().currentlyDying = true;
        }
        if (currentHealth < 1) Die();

    }

    public void Die()
    {
        //Debug.Log("Death is Disabled For testing");
        if (this.gameObject.tag == "Player") return;
 
        Destroy(this.gameObject);
    }

    public void IncreaseHealth(int value)
    {
        currentHealth += value;
    }

}
