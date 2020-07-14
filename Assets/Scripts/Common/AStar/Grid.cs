using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public enum GridType {
    Start,
    Normal,
    Obstacle,
    End
}
public class Grid : IComparable {
    //坐标
    public int x;
    public int y;
    //格子类型
    public GridType type; 
    //总消耗
    public int f;
    //
    public int g;
    //当前网格到终点的消耗
    public int h;
    //当前格子节点的父节点
    public Grid parent;
    public Grid(int x, int y, GridType type)
    {
        this.x = x;
        this.y = y;
        this.type = type;
    }
    public int CompareTo(object obj)
    {
        Grid grid = obj as Grid;
        if (this.f > grid.f)
        {
            return -1;
        }
        else if (this.f < grid.f)
        {
            return 1;
        }
        else
        {
            return 1;
        }
    }
}