using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;
public class CustomRenderPipelineAsset : RenderPipelineAsset
{
    public Color clearColor = Color.green;
#if UNITY_EDITOR
    [MenuItem("SRP/Create Basic RenderPipeline Asset")]
    public static void CreateBasicAssetPipeline()
    {
        var instance = ScriptableObject.CreateInstance<CustomRenderPipelineAsset>();
        AssetDatabase.CreateAsset(instance, "Assets/Resources/BasicAssetPipeline.asset");
    }
#endif  
    protected override RenderPipeline CreatePipeline()
    {
        //应该是运行时被调用生成SRP
        return new CustomRenderPipeline(clearColor);
    }
}
