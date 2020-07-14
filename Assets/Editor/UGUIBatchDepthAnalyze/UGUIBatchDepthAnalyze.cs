using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace StaticUITool
{
    /// <summary>
    /// UGUI分析工具
    /// 使用方法：选中Canvas物体，点击分析UI即可
    /// 在UI右边会出现一个字符串，如:1/0/1/2 代表 合批ID/深度/材质ID/贴图ID
    /// 合批ID相同的会合批为一个DC
    /// 材质ID、贴图ID、深度均会影响合批，主要你能改的是材质和贴图。
    /// *注意:材质ID和贴图ID均为伪ID
    /// 
    /// 2020年3月9日更新内容：
    /// Mask组件会打断合批，该工具会标红Mask裁剪图无法合批的情况
    /// Mask裁剪图无法合批主要有2个原因，1.Mask自身无法合批 2.Mask下子UI之间存在不可合批且相交的情况,可能有其他原因尚未发现    
    /// 
    /// ***额外说明***:
    /// 尚未支持分析其他打断合批的情况，如需扩展请研究下方代码:
    /// 分析物体数据:CheckCanvasBatchDepthMateri、AnalyzeUI 方法
    /// 判断是否可合批: IsCanBatch 方法 
    /// 
    /// 作者: Milk 
    /// 更新时间: 2020年3月9日
    /// CSDN博客: https://blog.csdn.net/qq_39574690
    /// </summary>
    [InitializeOnLoad]
    public class UGUIBatchDepthAnalyze
    {
        // 层级窗口项回调,添加这个层级回到函数后，当鼠标移动到层级视图中时，会调用这个方法
        private static readonly EditorApplication.HierarchyWindowItemCallback hiearchyItemCallback;

        private static Texture2D hierarchyIcon;
        private static Texture2D HierarchyIcon
        {
            get
            {
                if (UGUIBatchDepthAnalyze.hierarchyIcon == null)
                {
                    UGUIBatchDepthAnalyze.hierarchyIcon = (Texture2D)Resources.Load("icon_1");
                }
                return UGUIBatchDepthAnalyze.hierarchyIcon;
            }
        }

        /// <summary>
        /// 静态构造
        /// </summary>
        static UGUIBatchDepthAnalyze()
        {
            UGUIBatchDepthAnalyze.hiearchyItemCallback = new EditorApplication.HierarchyWindowItemCallback(UGUIBatchDepthAnalyze.DrawHierarchyIcon);
            EditorApplication.hierarchyWindowItemOnGUI = (EditorApplication.HierarchyWindowItemCallback)Delegate.Combine(
                EditorApplication.hierarchyWindowItemOnGUI,
                UGUIBatchDepthAnalyze.hiearchyItemCallback);
        }
        //记录UI物体数据
        static Dictionary<Transform, GameObjectData> dict;
        //记录UI层级（不是深度哦，就是普通的我们认识的层级）
        static Dictionary<Transform, int> orderDict = new Dictionary<Transform, int>();

        //映射用的
        static Dictionary<int, int> materialMapDict = new Dictionary<int, int>();
        static Dictionary<int, int> textureMapDict = new Dictionary<int, int>();

        static bool isStart = false;

        //UI结构层级(即属于UI树的层)
        static Dictionary<Transform, int> layerDict = new Dictionary<Transform, int>();

        //重绘制Hierarchy面板
        private static void DrawHierarchyIcon(int instanceID, Rect selectionRect)
        {
            UnityEngine.Debug.Log("=====DrawHierarchyIcon=======");
            //根据instanceID获取Hierarchy面板上的物体（注意每一个物体都会进入这个方法渲染）
            GameObject go = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
            Rect rectCheck = new Rect(selectionRect);
            //层级偏移量（可调整一个系数2）        
            int curOrder = 0;
            float orderOffset = 0;

            if (go != null && go.activeInHierarchy && IsNeedRenderer(go.transform))
            {
                if (layerDict.ContainsKey(go.transform))
                {
                    layerDict.TryGetValue(go.transform, out curOrder);
                    orderOffset = curOrder * 10;
                }
            }
            //可自行调整打印出的-/-/-/- 字符串偏移量150    
            rectCheck.x += rectCheck.width - 150 - orderOffset;
            rectCheck.width = 150;

            if (Selection.activeGameObject != null && Selection.activeGameObject.GetComponent<Canvas>() != null)
            {
                if (GUI.Button(new Rect(80, 20, 80, 30), "分析Canvas"))
                {
                    //计算UI层级队列，UI层级使用于下方的排序Sort
                    int order = 0;
                    orderDict.Clear();
                    GetUIOrder(orderDict, Selection.activeGameObject.transform, ref order);

                    //计算UI结构树层级,为了让Hierarchy面板打印的字符串凹进去（表现好看一点）
                    int layer = -1;
                    layerDict.Clear();
                    GetUILayer(layerDict, Selection.activeGameObject.transform, layer);

                    //重置开始深度计算标记
                    isStart = false;
                    //计算UI深度、材质ID、贴图ID
                    if (dict != null)
                    {
                        dict.Clear();
                    }
                    dict = CheckCanvasBatchDepthMateri(Selection.activeGameObject.GetComponent<Canvas>());

                    //排序UI物体数据
                    List<GameObjectData> list = new List<GameObjectData>();
                    foreach (var v in dict.Values)
                    {
                        list.Add(v);
                    }
                    list.Sort((a, b) =>
                    {
                        //1.根据深度
                        if (a.depth != b.depth)
                        {
                            return a.depth < b.depth ? -1 : 1;
                        }
                        //2.根据材质
                        if (a.materialID != b.materialID)
                        {
                            return a.materialID < b.materialID ? -1 : 1;
                        }
                        //3.根据贴图
                        if (a.textureID != b.textureID)
                        {
                            return a.textureID < b.textureID ? -1 : 1;
                        }
                        //4.根据UI层级
                        int orderA = 0;
                        int orderB = 0;
                        orderDict.TryGetValue(a.go.transform, out orderA);
                        orderDict.TryGetValue(b.go.transform, out orderB);
                        return orderA < orderB ? -1 : 1;
                    });

                    //制作材质ID、贴图ID映射表，为了让表现更好看，因为真实的贴图ID和材质ID都太长了！
                    int mid = 0;
                    int tid = 0;
                    materialMapDict.Clear();
                    textureMapDict.Clear();
                    for (int i = 0; i < list.Count; i++)
                    {
                        if (!materialMapDict.ContainsKey(list[i].materialID))
                        {
                            materialMapDict.Add(list[i].materialID, mid++);
                        }
                        if (!textureMapDict.ContainsKey(list[i].textureID))
                        {
                            textureMapDict.Add(list[i].textureID, tid++);
                        }
                    }

                    //计算UI 合批ID
                    int batchID = 0;
                    if (list.Count > 0)
                    {
                        list[0].batchID = batchID;
                        for (int i = 1; i < list.Count; i++)
                        {
                            //按顺序判断材质和贴图是否一样 一样的则为一个合批ID
                            //相邻层 材质ID和贴图ID相同时 为一个批次，即可合批！ （待测试是否正确）
                            GameObjectData data = list[i];
                            GameObjectData lastData = list[i - 1];
                            if (data.depth != lastData.depth || data.materialID != lastData.materialID || data.textureID != lastData.textureID)
                            {
                                batchID++;
                            }
                            data.batchID = batchID;
                        }
                    }

                    //打印排序后的物体数据情况
                    //for (int i = 0; i < list.Count; i++)
                    //{
                    //    Debug.Log(list[i].go + "," + list[i].depth + "," +  + list[i].materialID + "," + list[i].textureID + "," + orderDict[list[i].go.transform]);
                    //}
                    #region 分析Mask打断合批情况
                    //过滤出所有Mask物体
                    List<GameObjectData> maskList = new List<GameObjectData>();
                    for (int i = 0; i < list.Count; i++)
                    {
                        GameObjectData data = list[i];
                        Mask mask = data.go.GetComponent<Mask>();
                        if (mask != null && mask.enabled)
                        {
                            maskList.Add(data);
                            //Debug.Log(data.go + "拥有Mask" + ",Mask BatchID:" + data.batchID);
                        }
                    }
                    if (maskList.Count > 0)
                    {
                        int maskBatchId = -1;
                        //搜索出不可合批的Mask并标记为裁剪图不能合批
                        for (int i = 0; i < maskList.Count; i++)
                        {
                            GameObjectData data = maskList[i];
                            if (maskBatchId == -1)
                            {
                                maskBatchId = data.batchID;
                            }
                            //存在不可合批的Mask
                            if (maskBatchId != data.batchID)
                            {
                                //标记为不可合批Mask裁剪图，即会增加1个DC
                                data.isCanNotBatchMaskUI = true;
                                //Debug.Log(">>>>>>>>>>>>>>>>>>>>mask不可合批" + data.go);
                            }
                            if (!data.isCanNotBatchMaskUI)
                            {
                                //遍历子UI，检查是否存在不可合批的子UI相交
                                Transform maskTrans = data.go.transform;
                                Transform[] childsTrans = maskTrans.GetComponentsInChildren<Transform>();
                                Dictionary<int, GameObjectData> tempDict = new Dictionary<int, GameObjectData>();
                                tempDict.Clear();
                                for (int j = 0; j < childsTrans.Length; j++)
                                {
                                    if (data.isCanNotBatchMaskUI) break;//若已经确定不可合批则退出循环                                    
                                    Transform childTrans = childsTrans[j];
                                    if (childTrans != maskTrans)
                                    {
                                        RectTransform childRectTransform = childTrans.GetComponent<RectTransform>();
                                        GameObjectData childData;
                                        if (dict.TryGetValue(childsTrans[j], out childData))
                                        {
                                            //最终发现其实是这样的...
                                            for (int k = 0; k < childsTrans.Length; k++)
                                            {
                                                Transform tempChild = childsTrans[k];
                                                if (tempChild != maskTrans && tempChild != childTrans)
                                                {
                                                    RectTransform tempChildRectTrans = tempChild.GetComponent<RectTransform>();
                                                    GameObjectData tempData;
                                                    if (dict.TryGetValue(childsTrans[k], out tempData))
                                                    {
                                                        //存在相交不可合批时则无法合批mask裁剪图
                                                        if (tempData.batchID != childData.batchID && tempChildRectTrans.Overlaps(childRectTransform))
                                                        {
                                                            data.isCanNotBatchMaskUI = true;
                                                            break;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    #endregion
                }
            }

            // 绘制Label来覆盖原有的名字
            if (go != null && go.activeInHierarchy && IsNeedRenderer(go.transform))
            {
                if (dict != null && go.GetComponentInParent<Canvas>() != null && go.GetComponent<Canvas>() == null)
                {
                    GameObjectData data;
                    GUIStyle style = new GUIStyle();
                    style.richText = true;
                    if (dict.TryGetValue(go.transform, out data))
                    {
                        string str = "";
                        if (data.isCanNotBatchMaskUI)
                        {
                            str = "<color=red>B{0}/D{1}/M{2}/T{3}</color>";//标红代表Mask裁剪图无法合批
                        }
                        else
                        {
                            str = "B{0}/D{1}/M{2}/T{3}";
                        }
                        GUI.Label(rectCheck, string.Format(str, data.batchID, data.depth, materialMapDict[data.materialID], textureMapDict[data.textureID]), style);
                    }
                    else
                    {
                        GUI.Label(rectCheck, "-/-/-/-");
                    }
                }
            }
        }

        /// <summary>
        /// UI物体数据
        /// </summary>
        public class GameObjectData
        {
            public GameObject go; //物体
            public int materialID;//材质ID
            public int depth;     //深度
            public int batchID;   //合批ID
            public int textureID; //贴图ID
            public bool isCanNotBatchMaskUI; //是否不可以合批Mask裁剪图 默认为false(即可以)
        }

        static Dictionary<Transform, GameObjectData> CheckCanvasBatchDepthMateri(Canvas canvas)
        {
            //搜索整个Canvas的子物体们的合批、深度、材质        
            Dictionary<Transform, GameObjectData> dictionary = new Dictionary<Transform, GameObjectData>();
            if (canvas != null)
            {
                //开始分析            
                AnalyzeUI(canvas.transform, dictionary);
            }
            return dictionary;
        }

        /// <summary>
        /// 获取UI层级队列
        /// </summary>
        /// <param name="dict"></param>
        /// <param name="trans"></param>
        /// <param name="order"></param>
        static void GetUIOrder(Dictionary<Transform, int> dict, Transform trans, ref int order)
        {
            if (trans == null) return;
            dict.Add(trans, order++);
            if (trans.childCount > 0)
            {
                for (int i = 0; i < trans.childCount; i++)
                {
                    GetUIOrder(dict, trans.GetChild(i), ref order);
                }
            }
        }

        /// <summary>
        /// 获取UI树结构层级
        /// </summary>
        /// <param name="dict"></param>
        /// <param name="trans"></param>
        /// <param name="layer"></param>
        static void GetUILayer(Dictionary<Transform, int> dict, Transform trans, int layer)
        {
            if (trans == null) return;
            dict.Add(trans, layer);
            if (trans.childCount > 0)
            {
                layer++;
                for (int i = 0; i < trans.childCount; i++)
                {
                    GetUILayer(dict, trans.GetChild(i), layer);
                }
            }
        }
        /// <summary>
        /// 是否需要渲染
        /// </summary>
        /// <param name="trans"></param>
        /// <returns></returns>
        static bool IsNeedRenderer(Transform trans)
        {
            //TODO 不渲染的情况都要排除... 还差考虑组件的 Mask CanvasGroup 等情况
            Graphic renderer = trans.GetComponent<Graphic>();
            if (trans == null || !trans.gameObject.activeInHierarchy || (renderer != null && renderer.color.a == 0))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 深度优先级遍历
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="dict"></param>
        static void AnalyzeUI(Transform trans, Dictionary<Transform, GameObjectData> dict)
        {
            if (!IsNeedRenderer(trans))
            {
                return;
            }
            Graphic renderer = trans.GetComponent<Graphic>();
            Transform[] childs = GetChildren(trans);

            //根据子物体和它自身的信息获取depth
            //1. 无子物体时，深度为0
            //2. 有1个子物体且相交时，可Batch时，深度为子物体深度；不可Batch时，深度为子物体深度+1
            //3. 有n个子物体且相交时，根据(2.)规则计算所有depth_i, 取max(depth_1, depth_2, ... depth_x);作为层级   

            //物体数据
            GameObjectData gameObjectData = new GameObjectData()
            {
                go = trans.gameObject
            };
            if (renderer != null)
            {
                gameObjectData.materialID = renderer.material.GetInstanceID();
                gameObjectData.textureID = renderer.mainTexture.GetNativeTexturePtr().ToInt32();
            }
            //深度计算开始
            //1.没有下层物体情况
            //获取下层物体列表
            List<Transform> downObjArr = new List<Transform>();
            int curLayer = 0;
            if (orderDict.TryGetValue(trans, out curLayer))
            {
                foreach (var v in orderDict)
                {
                    if (v.Value < curLayer)
                    {
                        downObjArr.Add(v.Key);
                    }
                }
            }
            else
            {
                Debug.LogError("获取不到当前层级" + trans);
                return;
            }
            //1.没有下层物体时
            if (downObjArr.Count == 0)
            {
                gameObjectData.depth = 0;
            }
            //第2.3种情况有子物体
            if (downObjArr != null && downObjArr.Count > 0)
            {
                //2. 有1个下层物体且相交时，可Batch时，深度为下层物体深度；不可Batch时，深度为下层物体深度+1
                if (downObjArr.Count == 1)
                {
                    if (IsNeedRenderer(downObjArr[0]))
                    {
                        //相交时
                        if (downObjArr[0].GetComponent<RectTransform>().Overlaps(trans.GetComponent<RectTransform>()))
                        {
                            gameObjectData.depth = CalculateDepth(trans, downObjArr[0], dict);
                        }
                        else
                        {

                        }
                    }
                }
                else
                {
                    //3.有n个下层物体且相交时，根据(2.)规则计算所有depth_i, 取max(depth_1, depth_2, ... depth_x);作为层级   
                    List<int> depthList = new List<int>();
                    for (int i = 0; i < downObjArr.Count; i++)
                    {
                        if (IsNeedRenderer(downObjArr[i]))
                        {
                            //相交时
                            if (trans.GetComponent<RectTransform>().Overlaps(downObjArr[i].GetComponent<RectTransform>()))
                            {
                                int count = CalculateDepth(trans, downObjArr[i], dict);
                                depthList.Add(count);
                            }
                            else
                            {

                            }
                        }
                    }
                    depthList.Sort((a, b) =>
                    {
                        return a == b ? 0 : a > b ? -1 : 1;
                    });
                    if (depthList.Count >= 1)
                    {
                        gameObjectData.depth = depthList[0];//取最大的depth
                    }
                }
            }
            //Debug.Log(trans + " " + gameObjectData.depth + " " + gameObjectData.materialID);
            Canvas canvas = trans.GetComponent<Canvas>();
            if (canvas == null)
            {
                dict.Add(trans, gameObjectData);
            }
            if (canvas != null && !isStart || canvas == null)
            {
                //允许第一个Canvas开头深入, 但第二次遇到Canvas时，由于isStart为true, 因此不可继续深入第二个Canvas!
                if (canvas != null && !isStart)
                    isStart = true;

                //继续深入
                if (childs != null && childs.Length > 0)
                {
                    for (int i = 0; i < childs.Length; i++)
                    {
                        AnalyzeUI(childs[i], dict); //递归
                    }
                }
            }
        }

        #region 错误的情况
        /// <summary>
        /// 这是个人理解错误的情况 当时认为是下层物体就是子物体，结果是层级队列的下层意思 如上方法
        /// 你也可以看看 这种处理后的效果，不过我看过 这种确实是错误的...
        /// </summary>
        /// <param name="t1"></param>
        /// <param name="t2"></param>
        /// <param name="dict"></param>
        /// <returns></returns>
        //static void AnalyzeUI(Transform trans, Dictionary<Transform, GameObjectData> dict)
        //{        
        //    Graphic renderer = trans.GetComponent<Graphic>();
        //    if(!IsNeedRenderer(trans))
        //    {
        //        return;
        //    }

        //    Transform[] childs = GetChildren(trans);
        //    if (childs != null && childs.Length > 0)
        //    {
        //        for (int i = 0; i < childs.Length; i++)
        //        {
        //            AnalyzeUI(childs[i], dict);
        //        }
        //    }
        //    //根据子物体和它自身的信息获取depth
        //    //1. 无子物体时，深度为0
        //    //2. 有1个子物体且相交时，可Batch时，深度为子物体深度；不可Batch时，深度为子物体深度+1
        //    //3. 有n个子物体且相交时，根据(2.)规则计算所有depth_i, 取max(depth_1, depth_2, ... depth_x);作为层级   

        //    //物体数据
        //    GameObjectData gameObjectData = new GameObjectData()
        //    {
        //        go = trans.gameObject
        //    };      
        //    if (renderer != null)
        //    {
        //        gameObjectData.materialID = renderer.material.GetInstanceID();
        //        gameObjectData.textureID = renderer.mainTexture.GetNativeTexturePtr().ToInt32();
        //    }
        //    //深度计算开始
        //    //1.没有子物体情况
        //    if (childs == null || childs.Length <= 0)
        //    {
        //        gameObjectData.depth = 0;
        //    }
        //    //第2.3种情况有子物体
        //    if (childs != null && childs.Length > 0)
        //    {
        //        //2.
        //        if (childs.Length == 1 && IsNeedRenderer(childs[0]))
        //        {
        //            //相交时
        //            if (childs[0].GetComponent<RectTransform>().Overlaps(trans.GetComponent<RectTransform>()))
        //            {
        //                gameObjectData.depth = CalculateDepth(trans, childs[0], dict);
        //            }
        //            else
        //            {

        //            }
        //        }
        //        else
        //        {
        //            //3.
        //            List<int> depthList = new List<int>();
        //            for (int i = 0; i < childs.Length; i++)
        //            {
        //                if (IsNeedRenderer(childs[i]))
        //                {
        //                    //相交时
        //                    if (trans.GetComponent<RectTransform>().Overlaps(childs[i].GetComponent<RectTransform>()))
        //                    {
        //                        depthList.Add(CalculateDepth(trans, childs[i], dict));
        //                    }
        //                    else
        //                    {

        //                    }
        //                }
        //            }
        //            depthList.Sort((a,b)=> {
        //                return a == b ? 0 : a > b ? -1 : 1;
        //            });
        //            if (depthList.Count >= 1)
        //            {
        //                gameObjectData.depth = depthList[0];
        //            }
        //        }
        //    }
        //    Debug.Log(trans + " " + gameObjectData.depth + " " + gameObjectData.materialID);
        //    if (trans.GetComponent<Canvas>() == null)
        //    {
        //        dict.Add(trans, gameObjectData);
        //    }
        //}
        #endregion

        ///计算深度，根据判断是否合批来计算t1的深度返回出去 , t2是下层物体, dict是一个存储数据的字典
        static int CalculateDepth(Transform t1, Transform t2, Dictionary<Transform, GameObjectData> dict)
        {
            GameObjectData data;
            dict.TryGetValue(t2.transform, out data);
            if (data != null)
            {
                if (IsCanBatch(t1, t2))
                {
                    return data.depth;//depth为子物体的深度
                                      //return dict[t2.transform].depth;
                }
                else
                {
                    return data.depth + 1; //depth为子物体的深度 + 1            
                                           //return dict[t2.transform].depth + 1;
                }
            }
            if (t2.GetComponent<Canvas>() == null)
            {
                Debug.LogError(t1 + "," + t2 + ":居然没有深度,导致t1的深度为-1");
                return -1;
            }
            else
            {
                return 0;//Canvas本身就没深度为0
            }
        }

        /// <summary>
        /// 判断2个物体是否可合批 (根据实际情况可能需要改动，因为暂且无法确保包含所有情况 本人水平差）
        /// 
        /// 1、比较材质(材质本身、材质上的贴图）
        /// 2、若比较材质相同时，继续比较图片的贴图（非材质贴图）即Image的贴图
        /// </summary>
        /// <param name="t1"></param>
        /// <param name="t2"></param>
        /// <returns></returns>
        static bool IsCanBatch(Transform t1, Transform t2)
        {
            //检查材质是否相同来判断是否可以Batch
            bool isCan = false;
            //比较材质
            Graphic g1 = t1.GetComponent<Graphic>();
            Graphic g2 = t2.GetComponent<Graphic>();
            if (g1 == null || g2 == null)
            {
                //Debug.Log(t1 + "," + t2 + "Graphic其中一个为空 可合批");
                isCan = true;
            }
            else
            {
                Material m1 = g1.material;
                Material m2 = g2.material;
                if (m1 == null || m2 == null)
                {
                    //Debug.Log(t1 + "," + t2 + "Material其中一个为空 可合批"); //这种可能是不正确的，可能材质其中一个为空代表不能合批
                    isCan = true;
                }
                else
                {
                    if (m1.Equals(m2))
                    {
                        //比较纹理
                        Texture tex1 = m1.mainTexture;
                        Texture tex2 = m2.mainTexture;
                        if (tex1 == null && tex2 != null || tex1 != null && tex2 == null)//其中一个为空，另一个不为空，则表示材质实际不同
                        {
                            //Debug.Log(t1 + "," + t2 + "材质的纹理其中一个为空 不可合批！");
                            isCan = false;
                        }
                        if (tex1 == null && tex2 == null)
                        {
                            //Debug.Log(t1 + "," + t2 + "材质的纹理都为空 可合批！");
                            isCan = true;
                        }
                        if (tex1 != null && tex2 != null)
                        {
                            if (tex1.Equals(tex2))
                            {
                                //Debug.Log(t1 + "," + t2 + "材质的纹理相同 可合批！");
                                isCan = true;
                            }
                            else
                            {
                                //Debug.Log(t1 + "," + t2 + "材质的纹理不相同 不可合批！");
                                isCan = false;
                            }
                        }
                    }
                }
                //当材质认为一样时（材质一样，材质贴图一样时），继续考虑Graphic的图片情况(即是否属于同一个图集)
                if (isCan)
                {
                    //考虑完材质 考虑图片的纹理图
                    Texture mainTex1 = g1.mainTexture;
                    Texture mainTex2 = g2.mainTexture;
                    if (mainTex1 == null && mainTex2 != null) { /*Debug.Log(t1 + "," + t2 + "图片贴图其中一个为空 不可合批！");*/ isCan = false; }
                    if (mainTex1 != null && mainTex2 == null) { /*Debug.Log(t1 + "," + t2 + "图片贴图其中一个为空 不可合批！");*/ isCan = false; }
                    if (mainTex2 == null && mainTex2 == null) { /*Debug.Log(t1 + "," + t2 + "图片贴图2个都为空，可以合批！！");*/ isCan = true; }

                    if (mainTex1 != null && mainTex2 != null)
                    {
                        if (mainTex1.Equals(mainTex2))
                        {
                            //Debug.Log(t1 + "," + t2 + "图片贴图相同 可合批!！");
                            isCan = true;
                        }
                        else
                        {
                            //Debug.Log(t1 + "," + t2 + "图片贴图不相同 不可合批!！");
                            isCan = false;
                        }
                    }
                }
            }
            return isCan;
        }

        /// <summary>
        /// 获取其下所有子物体
        /// </summary>
        /// <param name="trans"></param>
        /// <returns></returns>
        static Transform[] GetChildren(Transform trans)
        {
            if (trans == null || trans.childCount <= 0)
            {
                return null;
            }
            Transform[] childs = new Transform[trans.childCount];
            for (int i = 0; i < trans.childCount; i++)
            {
                childs[i] = trans.GetChild(i);
            }
            return childs;
        }
    }
}