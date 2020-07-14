using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleFog : MonoBehaviour
{
    [Range(0, 3)]
    public float Lerp = 0;//使用它来调整可视区域的大小
    public Texture2D MaskTex;
    public Shader ScreanShader;
    public Material GetMaterial
    {
        get
        {
            if (_material == null) _material = new Material(ScreanShader);
            return _material;
        }
    }
    private Material _material = null;
    //src是摄像机截取到的照片，dest是处理过的图片
    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        GetMaterial.SetTexture("_MainTex", src);
        GetMaterial.SetTexture("_MaskTex", MaskTex);
        GetMaterial.SetFloat("_Lerp", Lerp);
        Graphics.Blit(src, dest, GetMaterial);
    }
}