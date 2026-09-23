using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    [SerializeField] private int damage = 1;

    public BulletPool pool;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<Player>().TakeDamage(damage);
            pool.Release(gameObject);
        }

        if (collision.CompareTag("Wall"))
        {
            pool.Release(gameObject);
        }
    }
    
}
