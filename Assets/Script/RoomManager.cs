using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    private int level;
    [SerializeField] private GameObject initRoom;
    [SerializeField] private GameObject portalRoom;

    [SerializeField] private GameObject[] bossRoom;
    [SerializeField] private GameObject[] battleRoom;

    [SerializeField] private Floor ordinaryFloor;
    [SerializeField] private Floor bossFloor;

    List<RoomNode> nodes = new List<RoomNode>();

    private Floor currentFloor;
    private GameObject[] battleShuffled,bossShuffled;
    private void Start()
    {
        level = GameManager.Instance.GetLevelCounter();
        currentFloor = (level % 3 == 0 ? bossFloor : ordinaryFloor);

        //战斗房间随机
        battleShuffled = (GameObject[])battleRoom.Clone();
        Shuffle(battleShuffled);
        //boss房间随机
        bossShuffled = (GameObject[])bossRoom.Clone();
        Shuffle(bossShuffled);

        CreateMap();
    }
    private void CreateMap()
    {
        
        RoomNode root = new RoomNode
        {
            roomPrefab = initRoom,
            parent = null
        };
        nodes.Add(root);

        List<GameObject> rooms = new List<GameObject>();
        rooms.Add(initRoom);
        //随机 普通战斗房prefab
        for (int i = 0; i < currentFloor.BattleRoom; i++) rooms.Add(battleShuffled[i]);
        //随机 boss房prefab
        for (int i = 0; i < currentFloor.BossRoom; i++) rooms.Add(bossShuffled[i]);
        if (currentFloor.PortalRoom == 1) rooms.Add(portalRoom);
        

        //随机方向
        for(int i=1;i< rooms.Count; i++)
        {
            Direct entryDir;
            do
            {
                entryDir = (Direct)Random.Range(0, 4);
            } while (nodes[i - 1].parent != null && entryDir == GetOpposite(nodes[i - 1].entryDir));

            nodes.Add(new RoomNode
            {
                roomPrefab = rooms[i],
                parent = nodes[i - 1],
                entryDir = entryDir
            });
        }

        for(int i = 0; i < nodes.Count; i++)
        {
            RoomNode node = nodes[i];
            if(i == 0)
            {
                node.roomObj =  Instantiate(node.roomPrefab, Vector3.zero, Quaternion.identity);
                
            }
            else
            {
                node.roomObj = Instantiate(node.roomPrefab, Vector3.zero, Quaternion.identity);
                //父出口
                Vector3 parentExit = node.parent.roomObj.GetComponent<Room>().GetDoorPos(GetOpposite(node.entryDir));
                //节点入口
                Vector3 entry = node.roomObj.GetComponent<Room>().GetDoorPos(node.entryDir);
                //父Pos
                Vector3 parentPos = node.parent.roomObj.transform.position;

                //Debug.Log("pos:" + parentPos + ",子入口:" + entry + ",父出口:" + parentExit+",子入口方向："+node.entryDir);
                node.roomObj.transform.position = parentPos + parentExit - entry;
            }
        }

        roomConnected();


    }

    void roomConnected()
    {
        Room room;
        RoomNode node;

        for (int i = 0; i < nodes.Count; i++)
        {
            node = nodes[i];
            room = node.roomObj.GetComponent<Room>();
            if (node.parent != null)
            {
                room.SetConnected(node.entryDir);
            }
            if (i + 1 < nodes.Count)
            {
                room.SetConnected(GetOpposite(nodes[i + 1].entryDir));
            }
            room.ApplyWalls();
        }
        
        
    }

    private void Shuffle(GameObject[] array)
    {
        GameObject temp;
        int j;
        for (int i = array.Length - 1; i > 0; i--)
        {
            j = Random.Range(0, i + 1);
            temp = array[i];
            array[i] = array[j];
            array[j] = temp;
        }
    }

    //反方向
    Direct GetOpposite(Direct d)
    {
        switch (d)
        {
            case Direct.East: return Direct.West;
            case Direct.West: return Direct.East;
            case Direct.South: return Direct.North;
            case Direct.North: return Direct.South;
        }
        return Direct.East; 
    }
}
