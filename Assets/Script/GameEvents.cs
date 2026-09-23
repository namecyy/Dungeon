using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    MainMenu,
    Playing,
    Menu,
    GameOver,
    Victory
}

public static class GameEvents
{
    public static System.Action<int> OnGoldChanged; //金币变化
    public static System.Action<int,int> OnHealthChanged; //生命变化(当前生命值/最大生命值）

    public static System.Action OnPlayerDied; //Player死亡

    public static System.Action NextLevel;
    public static System.Action<GameState> OnGameStateChanged; // 游戏状态更替

    public static System.Action<Enemy> OnEnemyDied; //怪物死亡

    public static System.Action<int, int> OnBossHealthChanged;//Boss生命变化(当前生命值/最大生命值）
    public static System.Action OnBossDied; // Boss死亡
}
