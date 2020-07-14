using System.Diagnostics;
using System.Net.Mime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class MapGizmos : MonoBehaviour
{
    //显示Grid
    public bool isShowGizmos = true;
    public float height = 2;

    //格子大小 实际每个格子是1的距离，要有间隔，这里取0.8
    float size = 0.8f;
    public MapData mapData = null;
    //用于做射线检测
    public BoxCollider  collider = null;
    // Start is called before the first frame update
    void Start()
    {
        //设置Layer  根据具体项目设置
        gameObject.layer = 31;
        collider = GetComponent<BoxCollider>();
        if(collider == null){
            collider = gameObject.AddComponent<BoxCollider>();
            collider.size = new Vector3(100,0.1f,100);
        }
    }
    //自程序运行后就会调用，之后每帧执行
    private void OnDrawGizmos(){
        if(mapData == null){
            return;
        }
        if(isShowGizmos == false || Application.isEditor == false){
            return;
        }
        UnityEngine.Debug.Log("mapData.mapLen:" + mapData.mapLen);
        UnityEngine.Debug.Log("mapData.mapWidth:" + mapData.mapWidth);
        for(int i = 0;i<mapData.mapLen;i++){
            for(int j = 0;j<mapData.mapWidth;j++){
                Gizmos.color = (mapData.GetValue(i,j) == 0)?(new Color(1,0,1,0.5f)):(new Color(1,1,1,0.5f));
                Gizmos.DrawCube(new Vector3(i,height,j),new Vector3(size,0.1f,size));
            }
        }
    }
    //在鼠标打击到脚本挂载的物体的身上的时候运行
    private void OnDrawGizmosSelected(){

    }
}
