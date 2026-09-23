using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private enum TrackState { Direct, Chase }

    private Rigidbody2D rb;

    private TrackState trackState = TrackState.Direct;
    private float sightTimer = 0f;
    [SerializeField] private float sightConfirmTime = 0.3f;

    private int Health = 3;
    [SerializeField] private int enemy1Gold = 10;
    [SerializeField] private float enemySpeed = 2f;
    [SerializeField] private float attackRange = 0f;

    private Transform playerTransform;
    private Room room;
    private List<Vector2Int> path;

    private float repathTimer = 0f;
    private Vector2Int startCell;
    private Vector2Int goalCell;
    

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        room = transform.parent.GetComponent<Room>();
        playerTransform = Player.Instance.transform;

    }

    private void Update()
    {


    }

    private void FixedUpdate()
    {
        startCell = room.WorldToCell(transform.position);
        goalCell = room.WorldToCell(playerTransform.position);

        bool canSee = PathFinding.HasLineOfSight(startCell, goalCell, room.IsWalkable);
        UpdateTrackState(canSee);

        switch (trackState)
        {
            case TrackState.Direct:
                MoveToPlayer();
                break;
            case TrackState.Chase:
                ChasePlayer();
                break;
        }
    }

    private void UpdateTrackState(bool canSee)
    {
        switch (trackState)
        {
            case TrackState.Direct:
                // Lose sight -> chase immediately (avoid walking into walls)
                if (!canSee)
                {
                    trackState = TrackState.Chase;
                    sightTimer = 0f;
                }
                break;

            case TrackState.Chase:
                // Need stable sight before direct-chase (avoid corner jitter)
                if (canSee)
                {
                    sightTimer += Time.fixedDeltaTime;
                    if (sightTimer >= sightConfirmTime)
                    {
                        trackState = TrackState.Direct;
                        sightTimer = 0f;
                    }
                }
                else
                {
                    sightTimer = 0f;
                }
                break;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Bullet"))
        {
            Health--;
            if (Health <= 0)
            {
                PlayerData.Instance.AddGold(enemy1Gold);
                GameEvents.OnEnemyDied?.Invoke(this);
                Destroy(gameObject);
            }
        }
    }

    private void ChasePlayer()
    {
        MoveAlongPath();

        repathTimer += Time.fixedDeltaTime;
        if (repathTimer >= 0.5f)
        {
            repathTimer = 0f;
            RecalculatePath(startCell, goalCell);
        }
    }

    private void RecalculatePath(Vector2Int startCell, Vector2Int goalCell)
    {
        path = PathFinding.FindPath(startCell, goalCell, room.IsWalkable);
    }

    private void MoveAlongPath()
    {
        if (path == null || path.Count == 0) return;
        Vector2 nextPos = room.CellToWorld(path[0]);

        Vector2 direct = (nextPos - (Vector2)transform.position).normalized;
        float angle = Mathf.Atan2(direct.y, direct.x) * Mathf.Rad2Deg;

        rb.MovePosition(Vector2.MoveTowards(rb.position, nextPos, enemySpeed * Time.fixedDeltaTime));
        rb.SetRotation(angle);

        if ((nextPos - (Vector2)transform.position).sqrMagnitude < 0.1f * 0.1f)
        {
            path.RemoveAt(0);
        }
    }

    private void MoveToPlayer()
    {
        Vector2 direct = (playerTransform.position - transform.position).normalized;
        float angle = Mathf.Atan2(direct.y, direct.x) * Mathf.Rad2Deg;

        rb.MovePosition(Vector2.MoveTowards(rb.position, playerTransform.position, enemySpeed * Time.fixedDeltaTime));
        rb.SetRotation(angle);
    }
}
