using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    public static PlayerData Instance { get; private set;}

    private int gold=0;

    [SerializeField]private int maxHealth = 3;
    private int currentHealth;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            currentHealth = maxHealth;
        }
        else
        {
            Destroy(gameObject);
        }

        

    }


    public int GetGold()
    {
        return gold;
    }

    public void AddGold(int amount)
    {
        gold += amount;
        GameEvents.OnGoldChanged?.Invoke(gold);
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        GameEvents.OnHealthChanged?.Invoke(currentHealth,maxHealth);
        if (currentHealth <= 0)
        {
            GameEvents.OnPlayerDied?.Invoke();
        }
    }

    public int GetMaxHealth()
    {
        return maxHealth;
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    public void Reset()
    {
        gold = 0;
        currentHealth = maxHealth;
    }
}
