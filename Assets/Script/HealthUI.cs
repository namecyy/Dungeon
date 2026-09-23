using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    private Text health;

    private void OnEnable()
    {
        health = GetComponent<Text>();
        health.text = "ÉúÃü:" + PlayerData.Instance.GetCurrentHealth();
        GameEvents.OnHealthChanged += HealthUpdate;
    }

    private void OnDisable()
    {
        GameEvents.OnHealthChanged -= HealthUpdate;
    }

    private void HealthUpdate(int currentHealth,int maxHealth)
    {
        health.text = "ÉúÃü:"+currentHealth;
    }

    
}
