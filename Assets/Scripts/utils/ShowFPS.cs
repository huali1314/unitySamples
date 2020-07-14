using System;
using System.Threading;
using System.Net.Mime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowFPS : UnitySingleton<ShowFPS>
{
    //更新间隔
    public float f_UpdateInterval = 0.5f;
    //上次更新间隔
    private float f_LastInterval;
    private int i_Frames = 0;
    private float f_Fps;
    private GUIStyle style1 = new GUIStyle();
    private GUIStyle style2 = new GUIStyle();
    private GUIStyle style3 = new GUIStyle();
    //服务器端ping值
    public float sSPing{
        set;
        get;
    }
    //客户端ping值
    public float cSPing{
        set;
        get;
    }
    void Start(){
        Application.targetFrameRate = 300;
        f_LastInterval = Time.realtimeSinceStartup;
        i_Frames = 0;
        style1.fontSize = 16;
        style2.fontSize = 16;
        style3.fontSize = 16;
        // style.normal.textColor = new Color(255,255,0,255);
    }
    void OnGUI(){
        if (f_Fps < 30.0f){
            style1.normal.textColor = new Color(255,0,0,255);
        }else{
            style1.normal.textColor = new Color(0,255,0,255);
        }
        if (sSPing > 100.0f){
            style2.normal.textColor = new Color(255,0,0,255);
        }else{
            style2.normal.textColor = new Color(0,255,0,255);
        }
        if (cSPing > 100.0f){
            style3.normal.textColor = new Color(255,0,0,255);
        }else{
            style3.normal.textColor = new Color(0,255,0,255);
        }
        GUI.Label(new Rect(0,0,200,200),"FPS:" + f_Fps.ToString("f2"),style1);
        GUI.Label(new Rect(0,15,200,200),"sSPing:" + sSPing.ToString("f2"),style2);
        GUI.Label(new Rect(0,30,200,200),"cSPing:" + cSPing.ToString("f2"),style3);
        // if(GUI.Button(new Rect(0,45,50,20),"test")){
        //     UnityEngine.Debug.Log("======button test========");
        // };
    }
    
    void Update(){
        ++i_Frames;
        if(Time.realtimeSinceStartup > f_LastInterval + f_UpdateInterval){
            f_Fps = i_Frames/(Time.realtimeSinceStartup - f_LastInterval);
            i_Frames = 0;
            f_LastInterval = Time.realtimeSinceStartup;
        }
    }
}
