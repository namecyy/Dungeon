using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class PathFinding 
{
    public static List<Vector2Int> FindPath(Vector2Int start,Vector2Int goal,Func<Vector2Int,bool> isWalkable)
    {
        //g
        Dictionary<Vector2Int, int> gScore = new Dictionary<Vector2Int, int>();
        //已走路线
        Dictionary<Vector2Int, Vector2Int> cameFrom = new Dictionary<Vector2Int, Vector2Int>();
        //待处理列表
        List<Vector2Int> openSet = new List<Vector2Int>();

        gScore[start] = 0;
        openSet.Add(start);
        while(openSet.Count > 0)
        {
            Vector2Int current = openSet[0];
            //最小f
            foreach(Vector2Int cell in openSet)
            {
                int currentF = gScore[current] + Heuristic(current, goal);
                int cellF = gScore[cell] + Heuristic(cell, goal);

                if (cellF < currentF)
                {
                    current = cell;
                }
            }
            openSet.Remove(current);
            if (current == goal)
            {
                //返回路径
                return ReconstructPath(cameFrom, current);
            }

            foreach(Vector2Int next in GetNeighbors(current))
            {
                if (!isWalkable(next)) continue;

                int tentativeG = gScore[current] + 1;
                //若这边来的路径更短
                if (!gScore.ContainsKey(next) || tentativeG < gScore[next])
                {
                    gScore[next] = tentativeG;
                    cameFrom[next] = current;

                    if(!openSet.Contains(next))
                        openSet.Add(next);
                }
            }
        }

        return new List<Vector2Int>();
    }

    public static bool HasLineOfSight(Vector2Int from, Vector2Int to, Func<Vector2Int, bool> isWalkable)
    {
        //
        int dx = (int)MathF.Abs(to.x - from.x);
        int dy = (int)MathF.Abs(to.y - from.y);

        //每步移动方向（符号）
        int sx = from.x < to.x ? 1 : -1;
        int sy = from.y < to.y ? 1 : -1;

        int err = dx - dy;

        int x = from.x, y = from.y;
        while (true)
        {
            if (!isWalkable(new Vector2Int(x, y))) return false;
            if (x == to.x && y == to.y) return true;
            int e2 = 2 * err;
            if (e2 > -dy)
            {
                err -= dy;
                x += sx;
            }

            if (e2 < dx)
            {
                err += dx;
                y+=sy;
            }
        }
    }

    //h 两点距离
    static int Heuristic(Vector2Int start, Vector2Int goal)
    {

        return Mathf.Abs(start.x - goal.x)+ Mathf.Abs(start.y - goal.y);
    }

    static List<Vector2Int> GetNeighbors(Vector2Int cell)
    {
        return new List<Vector2Int> { 
            cell + Vector2Int.down,
            cell + Vector2Int.left,
            cell + Vector2Int.right,
            cell + Vector2Int.up
            };
    }

    static List<Vector2Int> ReconstructPath(Dictionary<Vector2Int,Vector2Int> cameFrom, Vector2Int current)
    {
        List<Vector2Int> path = new List<Vector2Int>();

        while (cameFrom.ContainsKey(current))
        {
            path.Add(current);
            current = cameFrom[current];
        }

        path.Reverse();
        return path;
    }

}
