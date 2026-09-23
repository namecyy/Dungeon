using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    private Rigidbody2D rb;
    private Transform playerTransform;
    private BulletPool bulletPool;

    [Header("生命")]
    [SerializeField] private int maxHealth;
    private int currentHealth;

    [Header("攻击")]
    [SerializeField] private GameObject firePointFront,firePointCenter;
    [SerializeField] private GameObject bossBulletPrefab;
    [SerializeField] private float shootSpeed = 11f;

    private float bossSpeed = 3f;

    [SerializeField]private float attackRange = 7f;

    [SerializeField] private float chargeSpeed = 10;
    [SerializeField] private float chargeDistance = 20;



    [SerializeField] private GameObject remind;//前摇提醒
    [SerializeField] private float cooldownTime;//后摇时间

    private float offset = 40f;
    private float phase2SpreadAngle = 22.5f;
    int bulletCount = 6;

    private Coroutine bossLoopCoroutine;
    BossPhase phase = BossPhase.Phase1;
    private bool invincible = false;

    enum BossAttack
    {
        Spread,
        Radial,
        Charge
    }

    enum BossPhase
    {
        Phase1,Phase2
    }

    

    private void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        playerTransform = Player.Instance.transform;
        bulletPool = new BulletPool(bossBulletPrefab, 20);
        GameEvents.OnBossHealthChanged(currentHealth, maxHealth);
        bossLoopCoroutine = StartCoroutine(BossLoop());
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Bullet") && !invincible)
        {
            currentHealth--;
            GameEvents.OnBossHealthChanged(currentHealth, maxHealth);
            if (currentHealth <= 0)
            {
                GameEvents.OnBossDied?.Invoke();
                Destroy(gameObject);
            }
            else if (currentHealth <= maxHealth / 2 && phase == BossPhase.Phase1)
            {
                StopCoroutine(bossLoopCoroutine);   // 打断协程
                StartCoroutine(EnterPhase2());

            }
        }
    }

    private IEnumerator BossLoop()
    {
        while (true)
        {
            while((playerTransform.position - transform.position).sqrMagnitude >= attackRange * attackRange)
            {
                
                MoveToPlayer();
                yield return null;
            }
            
            yield return ScatterAttack();

            yield return new WaitForSeconds(cooldownTime);

        }
    }

    private IEnumerator ScatterAttack()
    {
        remind.SetActive(true);

        yield return new WaitForSeconds(0.5f);

        switch((BossAttack)Random.Range(0, 3))
        {
            case BossAttack.Spread: yield return FireSpread(); break;

            case BossAttack.Radial: FireRadial(); break;

            case BossAttack.Charge: yield return Charge(); break;
        }

        yield return new WaitForSeconds(0.3f);

        remind.SetActive(false);
    }

    private IEnumerator FireSpread()
    {

        Vector2 shootDir = ((Vector2)playerTransform.position - (Vector2)firePointFront.transform.position).normalized;
        float baseAngle = Mathf.Atan2(shootDir.y, shootDir.x) * Mathf.Rad2Deg;
        
        for(int i = -1; i <= 1; i++)
        {
            float angle = baseAngle + offset * i;                       
            Vector2 dir = Quaternion.Euler(0, 0, angle) * Vector2.right; //角度转方向向量  Quaternion.Euler(0, 0, angle)绕z轴转

            //GameObject bossBullet = Instantiate(bossBulletPrefab, firePointFront.transform.position, Quaternion.Euler(0, 0, angle));
            //bossBullet.GetComponent<Rigidbody2D>().velocity = dir * shootSpeed;

            SpawnBullet(firePointFront.transform.position, dir);

        }

        if(phase == BossPhase.Phase2)
        {
            yield return new WaitForSeconds(0.3f);

            for (int i = -1; i <= 1; i+=2)
            {
                float angle = baseAngle + phase2SpreadAngle * i;
                Vector2 dir = Quaternion.Euler(0, 0, angle) * Vector2.right;

                SpawnBullet(firePointFront.transform.position, dir);
            }
        }
    }

    private void FireRadial()
    {
        
        float angleStep = 360 / bulletCount;

        for(int i = 0; i < bulletCount; i++)
        {
            float angle = i * angleStep;
            Vector2 dir = Quaternion.Euler(0, 0, angle) * Vector2.right;

            SpawnBullet(firePointCenter.transform.position, dir);
        }
    }

    private IEnumerator Charge()
    {
        //yield return new WaitForSeconds(0.3f);
        Vector2 dir = (playerTransform.position - transform.position).normalized;
        Vector2 origion;
        float radius = GetComponent<CircleCollider2D>().radius;
        float travelled = 0f,step;
        while(travelled < chargeDistance)
        {

            step = chargeSpeed * Time.fixedDeltaTime;
            Vector2 origin = rb.position + dir * (radius + 0.05f);
            RaycastHit2D hit = Physics2D.Raycast(origin, dir, step, LayerMask.GetMask("Wall"));
            if (hit.collider != null)
                dir = Vector2.Reflect(dir, hit.normal);

            rb.MovePosition(rb.position + dir * step);
            travelled += step;

            yield return new WaitForFixedUpdate();
        }
        
        
    }

    private void MoveToPlayer()
    {
        rb.MovePosition(Vector2.MoveTowards(rb.position, playerTransform.position, bossSpeed * Time.fixedDeltaTime));
    }

    private IEnumerator EnterPhase2()
    {
        phase = BossPhase.Phase2;
        remind.SetActive(false);
        bulletPool.ClearActive();

        invincible = true;
        remind.SetActive(true);
        yield return new WaitForSeconds(1f);
        remind.SetActive(false);
        invincible = false;
        
        bossSpeed = 5f;
        bulletCount = 12;
        cooldownTime = 1.5f;

        bossLoopCoroutine = StartCoroutine(BossLoop());
    }
    

    private void SpawnBullet(Vector2 postion,Vector2 dir)
    {
        GameObject bullet = bulletPool.Get();
        bullet.transform.position = postion;
        bullet.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);
        
        bullet.GetComponent<Rigidbody2D>().velocity = dir * shootSpeed;
        bullet.GetComponent<EnemyBullet>().pool = bulletPool;

    }
}
