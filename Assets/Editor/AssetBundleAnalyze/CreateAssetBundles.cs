using UnityEditor;
using System.IO;
public class CreateAssetBundles
{
   [MenuItem("CustomTools/BuildBundles/Build AssetBundles ")]
   static void BuildAllAssetBundles(){
   		string assetBundleDirectory = "Assets/StreamingAssets";
   		if (!Directory.Exists(assetBundleDirectory)){
   			Directory.CreateDirectory(assetBundleDirectory);
   		}
   		BuildPipeline.BuildAssetBundles(assetBundleDirectory,BuildAssetBundleOptions.ChunkBasedCompression, 
                                        BuildTarget.StandaloneOSX);
   		AssetDatabase.Refresh();
   }
}
