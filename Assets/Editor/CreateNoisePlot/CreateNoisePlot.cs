using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.IO;

public class CreateNoisePlot : Editor
{
    private static float noiseHeavyMin = 0.0989423f;
    private static float noiseHeavyMax = 0.3989423f;
    [MenuItem("CustomTools/NoisePlot/CreateNormalNoisePlot")]
    private static void GenerateNormalNoisePlot()
    {
        CreateTeture2D(new Vector2(0.5f, 0.5f), 1, true, "NormalNoisePlot");
    }
    [MenuItem("CustomTools/NoisePlot/CreateReverseNoisePlot")]
    private static void GenerateReverseNoisePlot()
    {
        CreateTeture2D(new Vector2(0.5f, 0.5f), 1, false, "ReverseNoisePlot");
    }

    /// <summary>
    /// 高斯分布概率模型
    /// </summary>
    /// <param name="_x">随机变量</param>
    /// <param name="_μ">位置参数</param>
    /// <param name="_σ">尺度参数</param>
    /// <returns></returns>
    private static float NormalDistribution(float _x, float _μ, float _σ)
      {
         float _inverseSqrt2PI = 1 / Mathf.Sqrt(2 * Mathf.PI);
         float _powOfE = -(Mathf.Pow((_x - _μ), 2) / (2 * _σ * _σ));
         float _result = (_inverseSqrt2PI / _σ) * Mathf.Exp(_powOfE);
         return _result;
    }
    /// <summary>
    /// 通过高斯分布公式绘制一张离散效果图
    /// </summary>
    /// <param name="_centerPoint">中心点坐标</param>
    /// <param name="_consi">图颜色的阿尔法值</param>
    /// <returns></returns>
    public static void CreateTeture2D(Vector2 _centerPoint, float _consi,bool isNormalNoise,string name)
    {
         //创建一个2d纹理  
         Texture2D _newTex = new Texture2D(1024, 1024, TextureFormat.ARGB32, true);
         Color[] _colorBase = new Color[1024 * 1024];
         int _hwidth = (int)(1024 * _centerPoint.x);
         int _hheight = (int)(1024 * _centerPoint.y);
 
         float _per;
         int _index;
         for (int i = 0; i< 1024; i++)
         {
             for (int j = 0; j< 1024; j++)
             {
                 _per = (Mathf.Sqrt((i - _hwidth) * (i - _hwidth) + (j - _hheight) * (j - _hheight))) / 512;
                 float _tr = NormalDistribution(_per, 0, 1); //float _tr = NormalDistribution(0, 0, 1)=0.3989423f;也就是说中心点的概率为0.3989423f，即最大概率
                 bool _drawing = Random.Range(noiseHeavyMin, noiseHeavyMax) < _tr ? true : false;
                if (!isNormalNoise)
                {
                    _drawing = Random.Range(noiseHeavyMin, noiseHeavyMax) > _tr ? true : false;
                }
                if (_drawing)
                 {
                     _index = i* 1024 + j;
                     _colorBase[_index] = new Color(1, 0, 0,_tr * _consi); 
                 }
                 else
                 {
                     _colorBase[i * 1024 + j] = new Color(0, 0, 0, 0);
                 }
             }
         }
         _newTex.SetPixels(_colorBase);
         _newTex.Apply();
         _colorBase = null;
         System.GC.Collect();
        
        string path = EditorUtility.SaveFilePanel("NoisePlot", Application.dataPath + "/NoisePlot", name, "png");
        if (!path.EndsWith(".png"))
        {
            return;
        }
        FileStream fs = File.Open(path, FileMode.Create, FileAccess.Write);
        byte[] myByte = _newTex.EncodeToPNG();
        fs.Write(myByte, 0, myByte.Length);
        fs.Close();
    }
}
