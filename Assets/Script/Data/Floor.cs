using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Floor",menuName = "Floors")]
public class Floor : ScriptableObject
{
    public int InitRoom;
    public int BattleRoom;
    public int BossRoom;
    public int PortalRoom;
}
