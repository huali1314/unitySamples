using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
public class test : MonoBehaviour
{
    Action<Stack<string>> action;
    public Image image;
    
    // Start is called before the first frame update
    void Start()
    {
        action += Move;
        EventCenter.AddListener<float>((Int32)TEST.TEST2,SwitchScene);
        AStarManager.Instance.InitWithMapInfo(20,20);
        AStarManager.Instance.FindPath(new Vector2(2,6),new Vector2(9,7),action);
        AStarManager.Instance.DisplayPath();
    }
    public void OnClickSwitchScene(){
        //Application.Quit();
        EventCenter.Broadcast((Int32)TEST.TEST2, 0.1f);
    }
    public void SwitchScene(float param){
        UnityEngine.Debug.Log("=======parm======" + param);
        UIManager.Instance.SwitchScene("Scene1");
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    public static void Move(Stack<string> path)
    {
        
    }
    private void OnGUI()
    {
        if (GUI.Button(new Rect(0, 0, 200, 30), "mipmapLevel"))
        {
            Texture2D t =  image.mainTexture as Texture2D;
            UnityEngine.Debug.Log("requestedMipmapLevel" + t.requestedMipmapLevel);
            UnityEngine.Debug.Log("mipmapCount" + t.mipmapCount);
            UnityEngine.Debug.Log("mipMapBias" + t.mipMapBias);
            UnityEngine.Debug.Log("texelSize" + t.texelSize);
            UnityEngine.Debug.Log("name" + t.name);
        }
        if (GUI.Button(new Rect(0, 40, 200, 30), "down mipmapLevel"))
        {
            Texture2D t = image.mainTexture as Texture2D;
            t.mipMapBias -= 1;
            t.requestedMipmapLevel -= 1;
            UnityEngine.Debug.Log("mipMapBias" + t.mipMapBias);
            UnityEngine.Debug.Log("requestedMipmapLevel" + t.requestedMipmapLevel);
        }
        if (GUI.Button(new Rect(0, 80, 200, 30), "up mipmapLevel"))
        {
            Texture2D t = image.mainTexture as Texture2D;
            t.mipMapBias += 1;
            t.requestedMipmapLevel += 1;
            UnityEngine.Debug.Log("mipMapBias" + t.mipMapBias);
            UnityEngine.Debug.Log("requestedMipmapLevel" + t.requestedMipmapLevel);
        }
    }
}
