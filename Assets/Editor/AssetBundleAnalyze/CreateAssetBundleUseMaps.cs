using UnityEditor;
using System.IO;
public class CreateAssetBundlesUseMaps
{
    [MenuItem("CustomTools/BuildBundles/Build AssetBundles Use Maps ")]
    static void BuildAllAssetBundlesUseMaps()
    {
        string assetBundleDirectory = "Assets/StreamingAssets";
        if (!Directory.Exists(assetBundleDirectory))
        {
            Directory.CreateDirectory(assetBundleDirectory);
        }
        AssetBundleBuild []buildMaps = new AssetBundleBuild[2];
        buildMaps[0].assetBundleName = "enemyBundle.ab";
        string[] enemyAssets = new string[3];
        enemyAssets[0] = "Assets/Textures/qfdt_biaoti.png";
        enemyAssets[1] = "Assets/Textures/qfdt_dikuang-dead.png";
        enemyAssets[1] = "Assets/Images/alert/bg_commom_tipsbg.png";
        buildMaps[0].assetNames = enemyAssets;

        buildMaps[1].assetBundleName = "heroBundle.ab";
        string[] heroAssets = new string[3];
        heroAssets[0] = "Assets/Textures/qfdt_sy.png";
        heroAssets[1] = "Assets/Textures/qfdt_zh_dingbu.png";
        heroAssets[2] = "Assets/Resources/Prefabs/Box.prefab"; 
        buildMaps[1].assetNames = heroAssets;

        BuildPipeline.BuildAssetBundles(assetBundleDirectory,buildMaps, BuildAssetBundleOptions.ChunkBasedCompression,
                                     BuildTarget.StandaloneOSX);
        AssetDatabase.Refresh();
    }
}