using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapData
{
    //网格长度
    public int mapLen = 0;
    //网格宽度
    public int mapWidth = 0;
    //网格状态数据
    private List<int> values = new List<int>();

    public int GetValue(int index){
        if(index >= values.Count){
            UnityEngine.Debug.Log("MapData GetValue index is not exist! index=" +index.ToString());
            return -1;
        }
        return values[index];
    }
    public int GetValue(int x,int y){
        int index = x * mapWidth + y;
        GetValue(index);
        return values[index];
    }
    public void AddValue(int value){
        values.Add(value);
    }
    public void SetValue(int x,int y,int value){
        int index = x * mapWidth + y;
        values[index] = value;
    }
    public int GetValueCount(){
        return values.Count;
    }
    public void ClearValues(){
        values.Clear();
    }
    public string Serialized(){
        string str = mapLen.ToString() + "," + mapWidth.ToString() + ",";
        for(int i = 0;i < values.Count;i++){
            str += values[i].ToString() + ",";
        }
        return str;
    }
    public MapData Deserialized(string data){
        values.Clear();
        string[] strList = data.Split(',');
        mapLen = int.Parse(strList[0]);
        mapWidth = int.Parse(strList[1]);
        for(int i = 2;i< strList.Length;i++){
            if(!string.IsNullOrEmpty(strList[i])){
                values.Add(int.Parse(strList[i]));
            }
        }
        return this;
    }

}
