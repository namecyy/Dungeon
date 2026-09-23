using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }

    private bool invincible = false;
    private float invincibleCooldown = 0.8f;

    private void Awake()
    {
        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    public void TakeDamage(int damage)
    {
        if (invincible) return;
        PlayerData.Instance.TakeDamage(damage);
        invincible = true;
        StartCoroutine(EndInvincible());
    }

    private IEnumerator EndInvincible()
    {

        yield return new WaitForSeconds(invincibleCooldown);
        invincible = false;
    }

}
