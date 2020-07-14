using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(MapGizmos))]
public class MapGizmosEditor : Editor//编辑器扩展
{
    //重写检视图的属性
    public override void OnInspectorGUI(){
        base.OnInspectorGUI();
    }
    //当层级视图中带有MapGizmos组件的物体被选中时，会响应相应的事件
    public void OnSceneGUI(){
        if(MapEditor.Instance == null || MapEditor.Instance.editorGrid == false){
            return;
        }
        Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
        RaycastHit hitInfo;
        if(Physics.Raycast(ray,out hitInfo,2000,1<<31)){
            float x = hitInfo.point.x;
            float z = hitInfo.point.z;
            Event e = Event.current;
            if(e.isKey){
                if(e.keyCode == KeyCode.C){
                    MapEditor.Instance.EditorObserver(x,z,0);//设置障碍
                }
                if(e.keyCode == KeyCode.V){
                    MapEditor.Instance.EditorObserver(x,z,1);//可通行
                }
            }
        }
    }
}
