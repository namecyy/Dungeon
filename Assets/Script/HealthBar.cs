using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Image fillImage;

    private void OnEnable()
    {
        GameEvents.OnBossHealthChanged += BossHealthBar;
    }

    private void OnDisable()
    {
        GameEvents.OnBossHealthChanged -= BossHealthBar;
    }

    private void BossHealthBar(int current,int max)
    {
        fillImage.fillAmount = (float)current / max;
        Debug.Log(current);
    }
}
