using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class WebCamTest : MonoBehaviour
{
    [SerializeField]
    private Vector2Int webCamSize = new Vector2Int(1920, 1080);
    [SerializeField]
    private int webCamFPS = 20;
    [SerializeField]
    private RawImage rawImage;
    private WebCamTexture webCamTexture;
    // Start is called before the first frame update
    void Start()
    {
        //StartCamera();
    }

    private void StartCamera()
    {
        //请求设备权限
        Application.RequestUserAuthorization(UserAuthorization.WebCam);
        if (Application.HasUserAuthorization(UserAuthorization.WebCam))
        {
            WebCamDevice[] webCam = WebCamTexture.devices;
            webCamTexture = new WebCamTexture(webCam[0].name, webCamSize.x,webCamSize.y, webCamFPS);
            webCamTexture.filterMode = FilterMode.Bilinear;
            //webCamTexture.filterMode = FilterMode.Point;
            //webCamTexture.filterMode = FilterMode.Trilinear;
            rawImage.texture = webCamTexture;
            webCamTexture.Play();
        }
    }
    private void Play()
    {
        webCamTexture.Play();
    }
    private void Stop()
    {
        webCamTexture.Stop();
    }
    private void Pause()
    {
        webCamTexture.Pause();
    }
    private void OnGUI()
    {
        if (GUI.Button(new Rect(0, 0, 100, 40), "Camera"))
        {
            StartCamera();
        }
        if (GUI.Button(new Rect(0, 40, 100, 40), "Stop"))
        {
            Stop();
        }
        if (GUI.Button(new Rect(0, 80, 100, 40), "Pause"))
        {
            Pause();
        }
        if (GUI.Button(new Rect(0, 120, 100, 40), "Play"))
        {
            Play();
        }
        if (GUI.Button(new Rect(0, 160, 100, 40), "Shot1"))
        {
            StartCoroutine("Shot1");
        }
        if (GUI.Button(new Rect(0, 200, 100, 40), "Shot2"))
        {
            StartCoroutine("Shot2");
        }
        if (GUI.Button(new Rect(0, 240, 100, 40), "ScreenCapture"))
        {
            StartCoroutine("CaptureScreen");
        }
    }
    private IEnumerator Shot1()
    {

        yield return new  WaitForEndOfFrame();
        string path = Application.dataPath + "/ShotPicture/";
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        FileStream image = File.Open(path + "Shot1.png", FileMode.Create, FileAccess.ReadWrite);
        Texture2D texture2D = new Texture2D(rawImage.texture.width, rawImage.texture.height, TextureFormat.RGBA32, false);
        RenderTexture currentRT = RenderTexture.active;
        //获取一个临时渲染纹理
        RenderTexture renderTexture = RenderTexture.GetTemporary(rawImage.texture.width, rawImage.texture.height, 32);
        //拷贝一份texture数据到renderTexture中
        Graphics.Blit(rawImage.texture, renderTexture);
        //设置当前激活的renderTexture
        RenderTexture.active = renderTexture;
        //从当前激活的renderTexture中读取像素数据
        texture2D.ReadPixels(new Rect(0, 0, rawImage.texture.width, rawImage.texture.height), 0, 0,false);
        //应用因SetPixel或SetPixels引起的改变
        texture2D.Apply();
        //还原当前激活的帧缓冲区为默认帧缓冲
        RenderTexture.active = currentRT;
        //释放临时申请的渲染纹理
        RenderTexture.ReleaseTemporary(renderTexture);
        //数据格式转换
        byte[] bytes = texture2D.EncodeToPNG();
        //将数据写入到创建的空图片上
        image.Write(bytes,0,bytes.Length);
        //关闭文件流
        image.Close();
    }
    private IEnumerator Shot2()
    {

        yield return new WaitForEndOfFrame();
        string path = Application.dataPath + "/ShotPicture/";
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        FileStream image = File.Open(path + "Shot2.png", FileMode.Create, FileAccess.ReadWrite);
        Texture2D texture2D = new Texture2D(Screen.width, Screen.height, TextureFormat.RGBA32, false);
        //这里直接读取默认帧缓冲区中的像素数据
        texture2D.ReadPixels(new Rect(0, 0, Screen.width, Screen.width), 0, 0, false);
        texture2D.Apply();

        byte[] bytes = texture2D.EncodeToPNG();
        image.Write(bytes, 0, bytes.Length);
        image.Close();
    }
    private IEnumerator CaptureScreen()
    {
        yield return new WaitForEndOfFrame();
        string path = Application.dataPath + "/ShotPicture/";
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        string fileName = path + "capScreen.png";
        FileStream fs = File.Open(fileName,FileMode.Create,FileAccess.ReadWrite);
        Texture2D texture2D =  ScreenCapture.CaptureScreenshotAsTexture();
        byte[] bytes = texture2D.EncodeToPNG();
        fs.Write(bytes, 0, bytes.Length);
        fs.Close();
    }
}
