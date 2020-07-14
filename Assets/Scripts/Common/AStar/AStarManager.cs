using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Burst;
public class AStarManager:UnitySingleton<AStarManager>{

    //地图格子数组
    Grid[,] grids;
    //开启列表:放入所有可能是终点的Grid
    ArrayList openList;
    //关闭列表:每次查找找出的f值最小的Grid
    ArrayList closeList;

    //地图宽度
    private int width;
    //地图长度
    private int height;
    //终点位置
    private Vector2 endPosition;
    //寻路结果栈
    private Stack<string> result;
    //根据地图数据对格子数组进行初始化
    public void InitWithMapInfo(int mapWidth, int mapHeight) {
        width = mapWidth;
        height = mapHeight;
        grids = new Grid[mapWidth, mapHeight];
        openList = new ArrayList();
        closeList = new ArrayList();
        result = new Stack<string>();
        for (int i = 0;i < mapWidth;i++)
        {
            for (int j = 0; j < mapHeight; j++)
            {
                grids[i, j] = new Grid(i, j,GridType.Normal);
            }
        }
    }

    //根据起始点和终点查找路径
    public void FindPath(UnityEngine.Vector2 startPos, UnityEngine.Vector2 endPos,Action<Stack<string>> func)
    {
        openList.Clear();
        closeList.Clear();
        grids[(int)startPos.x, (int)startPos.y].type = GridType.Start;
        grids[(int)endPos.x, (int)endPos.y].type = GridType.End;
        endPosition = endPos;
        //将起点放入开启列表
        openList.Add(grids[(int)startPos.x, (int)startPos.y]);
        //取出当前开启列表中f值最小的Grid
        Grid curGrid = openList[0] as Grid;

        while (openList.Count > 0 && curGrid.type != GridType.End)
        {
            curGrid = openList[0] as Grid;
            //判断当前Grid是否是终点Grid
            if (curGrid.type == GridType.End)
            {
                UnityEngine.Debug.Log("=======Find Path========");
                GenerateResult(curGrid);
                func(result);
            }
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (i != 0 || j != 0)
                    {
                        int x = curGrid.x + i;
                        int y = curGrid.y + j;
                        if (x >= 0 && y >= 0 && x < width && y < height && curGrid.type != GridType.Obstacle &&!closeList.Contains(grids[x,y]))
                        {
                            //计算g值
                            int g = (int)Mathf.Sqrt(Mathf.Abs(i) + Mathf.Abs(j)) + curGrid.g;
                            if (grids[x, y].g == 0 || grids[x,y].g > g)
                            {
                                grids[x, y].g = g;
                                grids[x, y].parent = curGrid;
                            }
                            //计算h值
                            int h = (Mathf.Abs(x - (int)endPos.x) + Mathf.Abs(y - (int)endPos.y));
                            grids[x, y].h = h;
                            //计算出h值
                            int f = g + h;
                            grids[x, y].f = f;
                            if (!openList.Contains(grids[x, y]))
                            {
                                openList.Add(grids[x, y]);
                            }
                            openList.Sort();
                        }
                    }
                }
            }
            closeList.Add(curGrid);
            openList.Remove(curGrid);
            if (openList.Count == 0)
            {
                UnityEngine.Debug.Log("===========not Find Path=======");
            }
        }
    }
    public void GenerateResult(Grid grid)
    {
        Grid parent = grid.parent as Grid;
        if (parent != null)
        {
            result.Push(parent.x + "|" + parent.y);
            GenerateResult(parent);
        }
    }
    public void DisplayPath()
    {
        while(result.Count != 0)
        {
            UnityEngine.Debug.Log("========" + result.Pop());
        }
    }
}
