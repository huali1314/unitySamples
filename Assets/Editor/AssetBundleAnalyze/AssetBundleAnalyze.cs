using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Text;
using System.Reflection;
using UnityEngine.Profiling;

namespace StaticUITool
{
    //分析AB包资源冗余情况
    public class AssetBundleAnalyze : Editor
    {
        //分析资源信息
        public struct ResourcesRelativeABInfo
        {
            public string objName;//资源物体名称
            public string abName;//资源所在的全部AB包名称
            public int count;//所在AB包个数
            public string resourceType;//资源类型
            public string objStorage;//资源硬盘占用大小
            public string objMemory;//资源内存占用
        }
        //支持分析的资源类型
        public static List<Type> analyzeAssetType = new List<Type>
        {
            typeof(GameObject),
            typeof(Material),
            typeof(Texture2D),
            typeof(AnimationClip),
            typeof(AudioClip),
            typeof(Sprite),
            typeof(Shader),
            typeof(Font),
            typeof(Mesh),
            typeof(ParticleSystem),
            typeof(PrefabAssetType),
        };
        
        static void SaveInDict(Dictionary<string, ResourcesRelativeABInfo> dict, string resourcePath, UnityEngine.Object obj, string abPath)
        {
            //资源依赖信息结构体
            ResourcesRelativeABInfo info;
            if (!dict.TryGetValue(resourcePath, out info))
            {
                info = new ResourcesRelativeABInfo()
                {
                    abName = "",
                    count = 0,
                    objName = "",
                    objMemory = "",
                    resourceType = "",
                    objStorage = "",
                };
                info.resourceType = obj.GetType().ToString();
                info.objName = obj.name;
                info.objMemory = GetMemory(AssetDatabase.LoadMainAssetAtPath(resourcePath));
                info.objStorage = GetStorage(AssetDatabase.LoadMainAssetAtPath(resourcePath));
                dict.Add(resourcePath, info);
            }

            if (!info.abName.Contains(abPath))
            {
                info.abName += "'" + abPath + "'";
                info.count++;
            }
            dict[resourcePath] = info;
        }

        private static string GetStorage(UnityEngine.Object obj)
        {
            if (obj == null)
            {
                UnityEngine.Debug.Log("obj为空，无法获取硬盘大小");
                return "";
            }
            Sprite sprite = obj as Sprite;
            Texture texture = null;
            if (sprite != null)
            {
                texture = sprite.texture;
            }
            if (texture == null)
            {
                texture = obj as Texture;
            }
            if (texture != null)
            {
                Type type = System.Reflection.Assembly.Load("UnityEditor.dll").GetType("UnityEditor.TextureUtil");
                MethodInfo methodInfo = type.GetMethod("GetStorageMemorySize", BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public);

                UnityEngine.Debug.Log("硬盘占用：" + EditorUtility.FormatBytes((int)methodInfo.Invoke(null, new object[] { texture })));
                return EditorUtility.FormatBytes((int)methodInfo.Invoke(null, new object[] { texture }));
            }
            return "";

        }
        private static string GetMemory(UnityEngine.Object obj)
        {

            if (obj == null) return "";
            UnityEngine.Debug.Log("内存占用：" + EditorUtility.FormatBytes(Profiler.GetRuntimeMemorySizeLong(obj)));
            return EditorUtility.FormatBytes(Profiler.GetRuntimeMemorySizeLong(obj));
        }
        //获取一个AB包，根据abPath，内有缓存机制
        static AssetBundle GetABByABPath(Dictionary<string, AssetBundle> dict, string abPath)
        {
            if (!File.Exists(abPath))
            {
                UnityEngine.Debug.Log(abPath + "文件不存在");
                return null;
            }
            AssetBundle ab;
            if (!dict.TryGetValue(abPath, out ab))
            {
                ab = AssetBundle.LoadFromFile(abPath);
                dict.Add(abPath, ab);
            }
            return ab;
        }
        //存放AB包的目录
        static string ABDirPath = Application.streamingAssetsPath;
        [MenuItem("CustomTools/静态分析工具/分析AB包资源冗余", priority = 100)]
        public static void Check()
        {
            AssetBundle.UnloadAllAssetBundles(true);
            Resources.UnloadUnusedAssets();
            //找出工程内所有ab包  3种方法
            //（1）搜整个工程的.manifest后缀文件，取其前部分则为ab名称，这个后缀的文件是跟ab包同时生成的
            //（2）利用打包时的约束，如固定ab包后缀必须为.ab,之后过滤
            //（3）读取StreamingAssets文件来获取所有ab包名称
            //（4）AssetDataBase.GetAllAssetBundleNames()
            List<string> abPathList = new List<string>();
            Dictionary<string, ResourcesRelativeABInfo> dict = new Dictionary<string, ResourcesRelativeABInfo>();
            Dictionary<string, AssetBundle> abDict = new Dictionary<string, AssetBundle>();//缓存AB包
            //获取工程内所有ab包
            GetAllABPath(abDict, abPathList);

            //统计AB资源
            foreach(var abPath in abPathList)
            {
                string abTagName = abPath.Replace(ABDirPath + "/", "");
                //获取该AB依赖的全部AB包签名
                string[] abDependABNameArr = AssetDatabase.GetAssetBundleDependencies(abTagName,true);
                UnityEngine.Debug.Log("=========abDependABNameArr length==========" + abDependABNameArr.Length);
                for (int i = 0; i < abDependABNameArr.Length; i++)
                {
                    UnityEngine.Debug.Log("=========abDependABNameArr==========" + abDependABNameArr[i]);
                }

                //加载AB包
                AssetBundle ab = GetABByABPath(abDict, abPath);
                //获取AB包中的资源名
                string[] names = ab.GetAllAssetNames();
                //for (int i = 0; i < names.Length; i++)
                //{
                //    UnityEngine.Debug.Log("=========names==========" + i + names[i]);
                //}
                
                //获取资源所依赖的资源路径(包括脚本，图片，预制体等)
                string[] depens = AssetDatabase.GetDependencies(names);
                //for (int i = 0; i < depens.Length; i++)
                //{
                //    UnityEngine.Debug.Log("=========depens==========" + i + depens[i]);
                //}
                //分析的资源
                string[] allResourcePath = depens.Length > 0 ? depens : names;
                foreach (var resoucePath in allResourcePath)
                {
                    //通过ab包加载资源
                    UnityEngine.Object obj = ab.LoadAsset(resoucePath);
                    if (obj != null && analyzeAssetType.Contains(obj.GetType()))
                    {
                        //写入md
                        SaveInDict(dict, resoucePath, obj, abPath);
                    }
                    else
                    {
                        //若不能通过ab包加载资源，说明可能是依赖资源，可能存在冗余
                        if (obj == null)
                        {
                            bool isContain = false;
                            //从依赖的AB包中查找是否包含该依赖资源
                            foreach (var dependABName in abDependABNameArr)
                            {
                                AssetBundle dependAB = GetABByABPath(abDict, Application.streamingAssetsPath + "/" + dependABName);
                                if (dependAB.Contains(resoucePath))
                                {
                                    isContain = true;
                                    break;
                                }
                            }
                            //当所有依赖AB包中都无法找到该依赖资源时，说明这个依赖资源被隐式的打入了abPath（AB）中
                            if (!isContain)
                            {
                                obj = AssetDatabase.LoadMainAssetAtPath(resoucePath);
                                if (obj == null)
                                {
                                    UnityEngine.Debug.Log("依赖资源：" + resoucePath + ",无法被加载出来！");
                                    continue;
                                }
                                if (!analyzeAssetType.Contains(obj.GetType()))
                                {
                                    UnityEngine.Debug.Log("不支持此类型的分析：" + obj.GetType() + ",Object:" + obj.name);
                                    continue;
                                }
                                if (obj != null && analyzeAssetType.Contains(obj.GetType()))
                                {
                                    SaveInDict(dict, resoucePath, obj, abPath);
                                }
                            }
                        }
                        else if (!analyzeAssetType.Contains(obj.GetType()))
                        {
                            UnityEngine.Debug.Log("不支持此类型的分析：" + obj.GetType() + ",Object:" + obj);
                        }
                    }
                }
            }
            //写入文件
            ConvertMapToMarkDown(dict);

        }

        private static void GetAllABPath(Dictionary<string, AssetBundle> abDict, List<string> abPathList)
        {
            //方法①:在assetBundle资源目录中获取所有的.manifest文件，截取文件名，文件名即为assetBundle的文件名
            //string[] manifestArr = Directory.GetFiles(ABDirPath, "*.manifest", SearchOption.AllDirectories);
            //foreach (var v in manifestArr)
            //{
            //  abPathList.Add(ABDirPath + "/" + v.Substring(0, v.Length - 9));//截取.manifest前部分字符串
            //}
            //方法②:根据后缀获取所有的assetbundle资源，打包资源时要记得固定资源的后缀名为.ab
            string[] manifestArr = Directory.GetFiles(ABDirPath, "*.ab", SearchOption.AllDirectories);
            foreach (var v in manifestArr)
            {
                abPathList.Add(v);
            }
            //方法③:该方法可能并不适合您的项目，可自行选择任意其中一种，并改写代码以支持您的项目.
            //AssetBundle assetBundle = GetABByABPath(abDict, ABDirPath + "/StreamingAssets");
            //if (assetBundle == null)
            //{
            //    Debug.LogError("默认使用方法③获取工程内所有AB包路径，请您检查路径" + ABDirPath + "/StreamingAssets" + ",名为StreamingAssets的AB包是否存在!");
            //}
            //else
            //{
            //    AssetBundleManifest assetBundleManifest = assetBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
            //    string[] allABNameArr = assetBundleManifest.GetAllAssetBundles();
            //    foreach (var v in allABNameArr)
            //    {
            //        Debug.Log("=====================abName========" + ABDirPath + "/" + v);
            //        abPathList.Add(ABDirPath + "/" + v);
            //    }
            //}
        }
        #region ConvertMapToMarkDown 将数据转换成MACDown文件保存
        private static void ConvertMapToMarkDown(Dictionary<string, ResourcesRelativeABInfo> dict)
        {
            //放在工程的根路径下
            string path = Application.dataPath + "AnalyzeABResource.md";
            UnityEngine.Debug.Log("path:" + path);
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            //文件创建成功，进行内容填充
            using (FileStream fs = File.Create(path))
            {
                AddText(fs, "#ABRedundency_" + DateTime.Now.ToString("yyMMddHHmm") + " \r\n");
                AddText(fs, "资源名称|资源类型|资源硬盘占用大小|内存占用|AB文件数量|AB文件名\r\n");
                AddText(fs, "-----------------|----------------|-----------------|-------------------|-------------------|-------------------\r\n");
                string single = "";
                string repeat = "";
                float totalStorage = 0;
                float totalMemory = 0;
                foreach (var assetInfo in dict)
                {
                    if (assetInfo.Value.objStorage != null && assetInfo.Value.objStorage !="")
                    {
                        string tempStr = assetInfo.Value.objStorage;
                     
                        totalStorage += FormatToByte(tempStr);
                    }
                    if (assetInfo.Value.objMemory != null && assetInfo.Value.objMemory != "")
                    {
                        string tempStr = assetInfo.Value.objMemory;
                        totalMemory += FormatToByte(tempStr);
                    }
                    string str = "<font color=red>" + assetInfo.Key + "|</font><font color=green>" + assetInfo.Value.resourceType + "|</font><font color=blue>" + assetInfo.Value.objStorage + "|</font><font color=yellow>" + assetInfo.Value.objMemory + "|</font><font color=black>" + assetInfo.Value.count + "|</font><font color=darkviolet>" + assetInfo.Value.abName + "|</font>\r\n";
                    if (assetInfo.Value.count > 1)
                    {
                        repeat += str;
                    }
                    else
                    {
                        single += str;
                    }
                }
                AddText(fs, repeat);//让冗余的部分写到前面提示
                AddText(fs, single);//没有冗余的部分
                string s = "<font color=red>" + "NULL" + "|</font><font color=green>" + "null" + "|</font><font color=blue>" + FormatToUnits(totalStorage) + "|</font><font color=black>" + FormatToUnits(totalMemory) + "|</font><font color=black>" + "null" + "|</font><font color=darkviolet>" + "null" + "|</font>\r\n";
                AddText(fs, s);//添加总硬盘占用
            }
            UnityEngine.Debug.Log("分析完成");
            path = path.Substring(0, path.LastIndexOf("/"));
            path = path.Replace("/", "\\");
            //System.Diagnostics.Process.Start("explorer.exe", path);//生成后自动打开输出目录
        }
        //将读取出来的资源大小统一转换成Byte
        private static float FormatToByte(string str)
        {
           
            string tempStr = "";
            float totalByte = 0;
            if (str.IndexOf('K') != -1)
            {
                tempStr = str.Replace("KB", "");
                tempStr = tempStr.Trim();          
                totalByte = float.Parse(tempStr) * 1024f;
                return totalByte;
            }
            else if (str.IndexOf('M') != -1)
            {
                tempStr = str.Replace("MB", "");
                tempStr = tempStr.Trim();
                totalByte = float.Parse(tempStr) * 1024 *1024f;
                return totalByte;
            }
            else if (str.IndexOf('G') != -1)
            {
                tempStr = str.Replace("GB", "");
                tempStr = tempStr.Trim();
                totalByte = int.Parse(tempStr) * 1024f * 1024f * 1024f;
                return totalByte;
            }
            else
            {
                tempStr = str.Replace("B", "");
                tempStr = tempStr.Trim();
                totalByte = int.Parse(tempStr);
                return totalByte;
            }  
        }
        //根据文件大小转换成相应的单位
        private static string FormatToUnits(float byteValue)
        {
           
            if (byteValue >= 1024 && byteValue < 1024 * 1024)
            {
                return (byteValue / 1024) + "KB";
            }
            else if (byteValue >= 1024 * 1024 && byteValue < 1024 * 1024 * 1024)
            {
                return (byteValue / (1024 * 1024)) + "MB";
            }
            else if (byteValue >= 1024 * 1024 * 1024)
            {
                return (byteValue / (1024 * 1024 * 1024)) + "GB";
            }
            else
            {
                return byteValue + "B";
            }
        }
        //向文件流中写入数据
        private static void AddText(FileStream fs, string v)
        {
            byte[] info = new UTF8Encoding(true).GetBytes(v);
            fs.Write(info, 0, info.Length);
        }
        #endregion
    }

}

