# Dungeon —— 俯视角 2D Roguelite Demo

一款类《Enter the Gungeon》（挺进地牢）的俯视角 2D Roguelite 游戏 Demo，个人独立开发，用于学习与求职展示。

## 技术栈

- **引擎**：Unity 2022.3 LTS（2022.3.62f3c1）
- **语言**：C#
- **类型**：2D 俯视角 Roguelite

## 核心特性

### 🗺️ 程序化地牢生成
- 房间以**树形结构**组织，每个节点记录父房间与入口方向，运行时随机拼接
- **Fisher-Yates 洗牌**随机化房间顺序，每局地图都不同
- 门与门自动对齐连接；支持普通层 / Boss 层（每 3 层一个 Boss 层）

### 🧭 A* 寻路 + 视线检测
- 标准 **A\* 算法**（g 值 + 曼哈顿距离启发式），驱动敌人寻路
- 通过 `Func<Vector2Int, bool>` 委托注入"可行走"判断，寻路逻辑与具体地图**解耦**
- 附带 **Bresenham 直线算法**的视线检测（Line of Sight），用于敌人发现玩家

### ♻️ 泛型对象池
- 基于 `Func<T>` / `Action<T>` 委托的泛型对象池 `ObjectPool<T>`
- 创建 / 激活 / 销毁逻辑全部外置，池子本身不依赖任何具体类型
- 用于子弹等高频对象复用，显著降低 GC 开销

### 👹 Boss AI
- 多阶段 Boss 战斗
- 冲锋冲刺 + 撞墙反弹（`Vector2.Reflect` 镜面反射）等行为

### 📡 事件驱动架构
- 全局静态事件系统 `GameEvents`，解耦 UI、玩家、敌人、Boss 与游戏管理
- 涵盖金币、血量、玩家死亡、敌人死亡、Boss 血量、关卡推进、状态切换等事件

## 项目结构

```
Assets/
└── Script/
    ├── GameManager.cs                    # 游戏状态机 + 单例
    ├── RoomManager.cs                    # 程序化地牢生成（房间树 + 洗牌 + 门对齐）
    ├── PathFinding.cs                    # A* 寻路 + 视线检测
    ├── ObjectPool.cs                     # 泛型对象池
    ├── GameEvents.cs                     # 全局事件系统
    ├── Boss.cs                           # Boss AI（冲锋 + 撞墙反弹）
    ├── Enemy.cs                          # 敌人 AI（A* 追逐）
    ├── Player.cs / PlayerMovement.cs     # 玩家与移动
    ├── PlayerShoot.cs                    # 玩家射击
    ├── Bullet.cs / BulletPool.cs         # 子弹与对象池
    ├── Room.cs / RoomNode.cs / Door.cs   # 房间与连接
    ├── CameraFollow.cs                   # 相机跟随
    ├── UIManager.cs / HealthBar.cs       # UI 系统
    └── Data/Floor.cs                     # 楼层配置
```

## 如何运行

1. 使用 **Unity Hub** 打开本项目（Unity 2022.3 LTS）
2. 打开 `Assets/Scenes/MainMenu` 场景，点击 Play
3. 或直接运行游戏：`MainMenu → 开始游戏`

## 演示

 ▶ [🎬 观看游戏演示视频](https://github.com/namecyy/Dungeon/releases/download/video/Demonstration.mp4)

## 待办 / 后续计划

- [ ] 补充演示截图与视频
- [ ] 更多敌人种类与武器
- [ ] 音效与打击感优化
