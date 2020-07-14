using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class WebRequest : UnitySingleton<WebRequest>
{
    /// <summary>
    /// GET请求
    /// </summary>
    /// <param name="url">请求地址，例如 “www.baidu.com”</param>
    /// <param name="action">请求发起后处理回调结果的委托</param>
    /// <returns></returns>
    public void Get(string url, Action<UnityWebRequest> actionResult)
    {
        StartCoroutine(_Get(url, actionResult));
    }

    IEnumerator _Get(string url, Action<UnityWebRequest> actionResult)
    {
        using (UnityWebRequest request =  UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();
            if (actionResult != null)
            {
                actionResult(request);
            }
        }
    }
    /// <summary>
    /// POST请求
    /// </summary>
    /// <param name="url">请求地址，like “www.baidu.com”</param>
    /// /// <param name="lstformData">请求发送的数据</param>
    /// <param name="action">请求发起后处理回调结果的委托</param>
    /// <returns></returns>
    public void Post(string url, List<IMultipartFormSection> lstformData,Action<UnityWebRequest> actionResult)
    {
        StartCoroutine(_Post(url, lstformData,actionResult));
    }
    IEnumerator _Post(string url, List<IMultipartFormSection> lstformData,Action<UnityWebRequest> actionResult)
    {
        using (UnityWebRequest request = UnityWebRequest.Post(url, lstformData))
        {
            yield return request.SendWebRequest();
            if (request != null)
            {
                actionResult(request);
            }
        }
    }
    /// <summary>
    /// 下载文件
    /// </summary>
    /// <param name="url">请求地址</param>
    /// /// <param name="downloadFilePathAndName">储存文件的路径和文件名，like 'Application.persistentDataPath+"/unity3d.html"'</param>
    /// <param name="action">请求发起后处理回调结果的委托,处理请求对象</param>
    /// <returns></returns>
    ///
    public void DownloadFile(string url, string downlaodFilePathAndName, Action<UnityWebRequest> actionResult)
    {
        StartCoroutine(_downloadFile(url, downlaodFilePathAndName, actionResult));
    }

    IEnumerator _downloadFile(string url, string downlaodFilePathAndName, Action<UnityWebRequest> actionResult)
    {
        UnityWebRequest request = new UnityWebRequest(url);
        request.downloadHandler = new DownloadHandlerFile(downlaodFilePathAndName);
        yield return request.SendWebRequest();
        if (request != null)
        {
            actionResult(request);
        }
    }

    /// <summary>
    /// 请求AssetBundle
    /// </summary>
    /// <param name="url">请求地址，assetBundle存放地址，like 'http://www.my-server.com/myData.unity3d'</param>
    /// <param name="action">请求发起后处理回调结果的委托,处理请求对象</param>
    /// <returns></returns>
    ///
    public void GetAssetBundle(string url, Action<AssetBundle> actionResult)
    {
        StartCoroutine(_getAssetBundle(url, actionResult));
    }

    IEnumerator _getAssetBundle(string url, Action<AssetBundle> actionResult)
    {
        UnityWebRequest request = new UnityWebRequest(url);
        DownloadHandlerAssetBundle handler = new DownloadHandlerAssetBundle(request.url,uint.MaxValue);
        request.downloadHandler = handler;
        yield return request.SendWebRequest();
        AssetBundle bundle = null;
        Debug.Log(request.isNetworkError + "======isNetworkError=====");
        if (!(request.isNetworkError | request.isHttpError))
        {
            bundle = handler.assetBundle;
        }
        if (actionResult != null)
        {
            actionResult(bundle);
        }
    }
    /// <summary>
    /// 请求Texture
    /// </summary>
    /// <param name="url">请求地址，assetBundle存放地址，like 'http://www.my-server.com/image.png'</param>
    /// <param name="action">请求发起后处理回调结果的委托,处理请求对象</param>
    /// <returns></returns>
    ///
    public void GetTexture(string url, Action<Texture2D> actionResult)
    {
        StartCoroutine(_getTexture(url, actionResult));
    }

    IEnumerator _getTexture(string url, Action<Texture2D> actionResult)
    {
        UnityWebRequest request = new UnityWebRequest(url);
        DownloadHandlerTexture handler = new DownloadHandlerTexture();
        request.downloadHandler = handler;
        yield return request.SendWebRequest();
        Texture2D texture = null;
        if (!(request.isNetworkError | request.isHttpError))
        {
            texture = handler.texture;
        }
        if (actionResult != null)
        {
            actionResult(texture);
        }
    }
    /// <summary>
    /// 请求AudioClip
    /// </summary>
    /// <param name="url">请求地址，assetBundle存放地址，like 'http://www.my-server.com/mysound.mp3'</param>
    /// <param name="audioType">音频文件类型</param>
    /// <param name="action">请求发起后处理回调结果的委托,处理请求对象</param>
    /// <returns></returns>
    ///
    public void GetAudioClip(string url,AudioType audioType, Action<AudioClip> actionResult)
    {
        StartCoroutine(_getAudioClip(url, audioType, actionResult));
    }

    IEnumerator _getAudioClip(string url,AudioType audioType, Action<AudioClip> actionResult)
    {
        UnityWebRequest request = new UnityWebRequest(url);
        DownloadHandlerAudioClip handler = new DownloadHandlerAudioClip(request.url,audioType);
        request.downloadHandler = handler;
        yield return request.SendWebRequest();
        AudioClip audioClip = null;
        if (!(request.isNetworkError | request.isHttpError))
        {
            audioClip = handler.audioClip;
        }
        if (actionResult != null)
        {
            actionResult(audioClip);
        }
    }
    /// <summary>
    /// 通过PUT方式将字节流传到服务器
    /// </summary>
    /// <param name="url">服务器目标地址 like 'http://www.my-server.com/upload' </param>
    /// <param name="contentBytes">需要上传的字节流</param>
    /// <param name="resultAction">处理返回结果的委托</param>
    /// <returns></returns>
    public void UploadByPut(string url, byte[] contentBytes, Action<bool> actionResult)
    {
        StartCoroutine(_UploadByPut(url, contentBytes, actionResult, ""));
    }
    /// <summary>
    /// 通过PUT方式将字节流传到服务器
    /// </summary>
    /// <param name="url">服务器目标地址 like 'http://www.my-server.com/upload' </param>
    /// <param name="contentBytes">需要上传的字节流</param>
    /// <param name="resultAction">处理返回结果的委托</param>
    /// <param name="resultAction">设置header文件中的Content-Type属性</param>
    /// <returns></returns>
    IEnumerator _UploadByPut(string url, byte[] contentBytes, Action<bool> actionResult, string contentType = "application/octet-stream")
    {
        UnityWebRequest uwr = new UnityWebRequest();
        UploadHandler uploader = new UploadHandlerRaw(contentBytes);

        // Sends header: "Content-Type: custom/content-type";
        uploader.contentType = contentType;

        uwr.uploadHandler = uploader;

        yield return uwr.SendWebRequest();

        bool res = true;
        if (uwr.isNetworkError || uwr.isHttpError)
        {
            res = false;
        }
        if (actionResult != null)
        {
            actionResult(res);
        }
    }
}
