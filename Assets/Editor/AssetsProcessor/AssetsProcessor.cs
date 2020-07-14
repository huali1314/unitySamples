using UnityEngine;
using System.Collections;
using UnityEditor;

public class AssetsProcessor : AssetPostprocessor
{
    //模型导入之前调用
    public void OnPreprocessModel()
    {
        Debug.Log("OnPreprocessModel=" + this.assetPath);
    }
    //模型导入后调用
    public void OnPostprocessModel(GameObject go)
    {
        Debug.Log("OnPostprocessModel=" + go.name);
    }
    //纹理导入之前调用，针对入到的纹理进行设置
    public void OnPreprocessTexture()
    {
        Debug.Log("OnPreProcessTexture=" + this.assetPath);
        TextureImporter impor = this.assetImporter as TextureImporter;
        impor.maxTextureSize = 512;
        impor.textureType = TextureImporterType.Sprite;
        impor.mipmapEnabled = false;

    }
    //纹理导入之后调用
    public void OnPostprocessTexture(Texture2D texture)
    {
        string lowerCaseAssetPath = assetPath.ToLower();
        Debug.Log("lowerCaseAssetPath:" + lowerCaseAssetPath);
        bool isInSpritesDirectory = lowerCaseAssetPath.IndexOf("/sprites/") != -1;

        if (isInSpritesDirectory)
        {
            TextureImporter textureImporter = (TextureImporter)assetImporter;
            textureImporter.textureType = TextureImporterType.Sprite;
        }
    }
    //音频导入之前调用
    public void OnPreprocessAudio()
    {
        //AudioImporter audio = this.assetImporter as AudioImporter;
        //audio. = AudioCompressionFormat.MP3;
    }
    //音频导入之后调用
    public void OnPostprocessAudio(AudioClip clip)
    {

    }
    
    //所有的资源的导入，删除，移动，都会调用此方法，注意，这个方法是static的
    public static void OnPostprocessAllAssets(string[] importedAsset, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        //Debug.Log("OnPostprocessAllAssets");
        //foreach (string str in importedAsset)
        //{
        //    Debug.Log("importedAsset = " + str);
        //}
        //foreach (string str in deletedAssets)
        //{
        //    Debug.Log("deletedAssets = " + str);
        //}
        //foreach (string str in movedAssets)
        //{
        //    Debug.Log("movedAssets = " + str);
        //}
        //foreach (string str in movedFromAssetPaths)
        //{
        //    Debug.Log("movedFromAssetPaths = " + str);
        //}
    }
}
