using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    private Rigidbody2D rb;
    private Vector2 shootDir;
    [SerializeField]private GameObject FirePoint;
    [SerializeField] private GameObject BulletPrefab;
    
    private float shootSpeed = 20f;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (GameManager.Instance.GetState() == GameState.Menu) return;
        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        shootDir = transform.right;
        GameObject bullet = Instantiate(BulletPrefab, FirePoint.transform.position, FirePoint.transform.rotation);
        bullet.GetComponent<Rigidbody2D>().velocity = shootDir * shootSpeed;
    }
}
