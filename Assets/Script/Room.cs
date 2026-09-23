using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Room : MonoBehaviour
{
    [SerializeField] private Tilemap wallTilemap;
    [Header("生怪")]
    [SerializeField] private int enemyWave = 2;
    [SerializeField] private int enemyCount = 3;
    [SerializeField] private GameObject[] enemyPrefab;
    [SerializeField] private GameObject wallPrefab;
    private int currentWave;
    private int residual;

    private List<Vector3> walkablePostions = new List<Vector3>();
    private void Start()
    {
        residual = enemyCount;
        currentWave = enemyWave;
        RefreshDoors();
        BuildWalkablePositions();
    }

    private void OnEnable()
    {
        GameEvents.OnEnemyDied += EnemyDied;
    }

    private void OnDisable()
    {
        GameEvents.OnEnemyDied -= EnemyDied;
    }

    enum RoomState{
        Wait,
        Battling,
        Cleaned
    }
    [SerializeField]private RoomState currentState = RoomState.Wait;

    enum RoomType
    {
        InitRoom,
        NormalBattle,
        BossRoom,
        PortalRoom
    }
    [SerializeField] private RoomType roomType = RoomType.InitRoom;

    //获取门关于中心偏移
    public Vector3 GetDoorPos(Direct direct)
    {
        Door[] doors = GetComponentsInChildren<Door>();

        foreach(Door door in doors)
        {
            if (door.direction == direct) return door.transform.localPosition;
        }

        return Vector3.zero;
    }

    //enemy死亡事件接受函数
    private void EnemyDied(Enemy e)
    {
        if (e.transform.parent != transform) return;

        residual--;
        Debug.Log("该波剩余：" + residual);
        if (residual <= 0)
        {
            if (currentWave > 1)
            {
                currentWave--;
                residual = enemyCount;
                SpawnEnemies();
                Debug.Log("currentWave:" + currentWave);
            }
            else
            {
                currentState = RoomState.Cleaned;
                RefreshDoors();
            }
        }
    }
    
    //enemy生成
    private void SpawnEnemies()
    {

        /*int i;
        for(i = 0; i < enemyCount; i++)
        {
            Instantiate(enemyPrefab[0],*//*GetRandomWalkableTilePos()*//*, Quaternion.identity, transform);
        }*/
        switch (roomType)
        {
            case RoomType.NormalBattle:
                Vector3 pos;
                for (int i = 0; i < enemyCount; i++)
                {
                    pos = walkablePostions[Random.Range(0, walkablePostions.Count)];
                    Instantiate(enemyPrefab[0], pos, Quaternion.identity, transform);
                }
                break;
            case RoomType.BossRoom:
                Instantiate(enemyPrefab[0], transform);
                break;
        }
        

    }

    //随机位置防撞墙
    /*private Vector3 GetRandomWalkableTilePos()
    {

        Vector3 pos;
        int attempt;
        float height = GetRoomHeight() / 2 - 3, width = GetRoomWidth() / 2 - 3;
        for (attempt = 0; attempt < 30; attempt++)
        {
            pos = new Vector3(
                Random.Range(-width, width + 1),
                Random.Range(-height, height + 1),
            0);
            pos += transform.position;
            if (wallTilemap.GetTile(wallTilemap.WorldToCell(pos)) == null)
                return pos;
        }
        return transform.position;
    }*/

    //可行走位置列表
    private void BuildWalkablePositions()
    {
        BoundsInt bounds = wallTilemap.cellBounds;
        Vector3Int cell;
        for(int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for(int y = bounds.yMin; y< bounds.yMax; y++)
            {
                cell = new Vector3Int(x, y, 0);
                if (!HasWallNearby(cell)) continue;
                
                walkablePostions.Add(wallTilemap.GetCellCenterWorld(cell));
            }
        }
    }

    private bool HasWallNearby(Vector3Int center)
    {
        for(int x = -1; x <= 1; x++)
            for(int y = -1; y <= 1; y++)
                if (wallTilemap.GetTile(center + new Vector3Int(x, y, 0)) != null) return false;
        return true;
    }







    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if(currentState == RoomState.Wait)
            {
                SpawnEnemies();
                currentState = RoomState.Battling;
                RefreshDoors();
            }
        }
    }

    //该方向有链接
    public void SetConnected(Direct dir)
    {
        foreach(Door door in GetComponentsInChildren<Door>())
        {
            if(door.direction == dir)
            {
                door.connected = true;
                return;
            }
            
        }
    }

    //房间状态刷新
    private void RefreshDoors()
    {
        bool open = (currentState != RoomState.Battling);
        foreach(Door door in GetComponentsInChildren<Door>())
        {
            if (door.connected) door.SetOpen(open);
        }
    }

    /*public static implicit operator Room(RoomNode v)
    {
        throw new System.NotImplementedException();
    }*/

    //不用的门换墙换锅换不锈钢盆
    public void ApplyWalls()
    {
        Vector3 pos;
        foreach(Door door in GetComponentsInChildren<Door>())
        {
            if (!door.connected)
            {
                pos = door.transform.position;
                Destroy(door.gameObject);
                Instantiate(wallPrefab, pos, Quaternion.identity, transform);
            }
        }
    }

    public bool IsWalkable(Vector2Int cell)
    {
        if (!wallTilemap.cellBounds.Contains((Vector3Int)cell)) return false;
        return wallTilemap.GetTile((Vector3Int)cell) == null;
        
    }

    public Vector2Int WorldToCell(Vector3 worldPos)
    {
        Vector3Int cellPos =  wallTilemap.WorldToCell(worldPos);

        return (Vector2Int)cellPos;
    }

    public Vector2 CellToWorld(Vector2Int cellPos)
    {

        Vector2 worldPos = wallTilemap.GetCellCenterWorld((Vector3Int)cellPos);

        return worldPos;
    }
}
