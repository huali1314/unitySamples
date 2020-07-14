using System.Diagnostics;
using System.Globalization;
using System;
using System.Net.Mime;
using System.IO;
using UnityEngine;
using UnityEditor;
public class MapEditor : EditorWindow//编辑器窗口扩展，写自己的unity窗口
{
    private static MapEditor instance = null;
    public static MapEditor Instance{
        get{
            if(instance == null){
                instance = GetWindow<MapEditor>(typeof(MapEditor).Name);
            }
            return instance;
        }
        private set{}
    }
    //打开格子编辑器
    private static MapGizmos mapGizmos = null;
    private static MapData mapData = new MapData();
    private string mapFileName = "PathMap_Fight";
    public bool editorGrid = false;
    public float radius = 4;
    [MenuItem("CustomTools/地图编辑/寻路编辑")]
    static void Open(){
        if(instance = null){
            instance = GetWindow<MapEditor>(typeof(MapEditor).Name);
        }
        instance.Show();
        FindMapGizmos();
    }
    static void FindMapGizmos(){
        GameObject obj = GameObject.Find("MapGizmos");
        if(obj == null){
            obj = new GameObject("MapGizmos");
            mapGizmos = obj.AddComponent<MapGizmos>();
        }else{
            mapGizmos = obj.GetComponent<MapGizmos>();
        }
        mapGizmos.mapData = mapData;
    }

    public void SaveData(){
        string text = mapData.Serialized();
        string path = EditorUtility.SaveFilePanel("保存地图数据",Application.dataPath + "/pathMap",mapFileName,"map");
        if(!path.EndsWith(".map")){
            return;
        }
        FileStream fs = File.Open(path,FileMode.Create,FileAccess.Write);
        byte[] myByte = System.Text.Encoding.UTF8.GetBytes(text);
        fs.Write(myByte,0,myByte.Length);
        fs.Close();
    }
    public void LoadData(){
        string path = EditorUtility.OpenFilePanel("读取地图数据",Application.dataPath + "pathMap","map");
        if(!path.EndsWith(".map")){
            return;
        }
        using(FileStream fsRead = new FileStream(path,FileMode.Open)){
            int fsLen = (int)fsRead.Length;
            byte[] heByte = new byte[fsLen];
            int r = fsRead.Read(heByte,0,heByte.Length);
            string text = System.Text.Encoding.UTF8.GetString(heByte);
            text.Replace("\r\n","");
            UnityEngine.Debug.Log(@text);
            mapData.Deserialized(text);
        }
        mapGizmos.mapData = mapData;
    }
    //窗口UI上刷新调用
    void OnGUI(){
        if(mapGizmos == null){
            FindMapGizmos();
        }
        EditorGUILayout.LabelField("寻路编辑","");
        EditorGUILayout.BeginHorizontal();
        if(GUILayout.Button("读取地图信息")){
            LoadData();
        }
        if(GUILayout.Button("存储地图信息")){
            SaveData();
        }
        EditorGUILayout.EndHorizontal();
        mapFileName = EditorGUILayout.TextField("地图文件名(不要使用中文)",mapFileName);
        mapData.mapLen = int.Parse(EditorGUILayout.TextField("地图长度",mapData.mapLen.ToString()));
        mapData.mapWidth = int.Parse(EditorGUILayout.TextField("地图宽度",mapData.mapWidth.ToString()));
        if(mapData.GetValueCount()!= mapData.mapLen * mapData.mapWidth){
            mapData.ClearValues();
            for(int i = 0;i<mapData.mapLen;i++){
                for(int j = 0;j<mapData.mapWidth;j++){
                    mapData.AddValue(1);
                }
            }
        }
        editorGrid = EditorGUILayout.Toggle("编辑器不可通行区域(Ctrl设置障碍，Alt关闭障碍)",editorGrid);
        radius = float.Parse(EditorGUILayout.TextField("编辑半径",radius.ToString()));
    }

    public void EditorObserver(float x,float y,int value){
        int min_x = System.Convert.ToInt32(x - radius);
        if(min_x < 0){
            min_x = 0;
        }
        int min_y = System.Convert.ToInt32(y - radius);
        if(min_y < 0){
            min_y = 0;
        }
        int max_x = System.Convert.ToInt32(x + radius);
        if(max_x > mapData.mapLen){
            max_x = mapData.mapLen;
        }
        int max_y = System.Convert.ToInt32(y + radius);
        if(max_y > mapData.mapWidth){
            max_y = mapData.mapWidth;
        }
        UnityEngine.Debug.Log("x======" + x);
        UnityEngine.Debug.Log("y======" + y);
        UnityEngine.Debug.Log("value======" + value);
        if(radius == 0 && x > 0 && y > 0 && x < mapData.mapLen && y < mapData.mapWidth){
            mapData.SetValue((int)x,(int)y,value);
        }
        for(int i = min_x;i< max_x;i++){
            for(int j = min_y;j < max_y;j++){
                mapData.SetValue(i,j,value);
            }
        }
    }
}
