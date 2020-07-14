using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
public class UIManager : UnitySingleton<UIManager>
{
    //UI界面字典，存储所有面板Prefab的路径
    private Dictionary<UIPanelType, string> mPanelPathDictionary;
    //保存所有实例化面板的游戏物体的父类组件
    private Dictionary<UIPanelType, BasePanel> mPanelPool;
    //当前实例化面板的栈容器
    private Stack<BasePanel> mPanelStack;
    //根物体的变换
    private Transform mUIRootTransform;
    //序列化
    [Serializable]
    //存储UI面板的列表
    public class UIPanelTypeJson
    {
        public List<UIPanelInformation> infoList;
    }

    public override void Awake()
    {
        base.Awake();
        ParseUIPanelTypeJson();
    }
    ///<summary>
    //从配置文件中解析出相对应的object类
    ///</summary>
    ///<returns></returns>
    private void ParseUIPanelTypeJson()
    {
        mPanelPathDictionary = new Dictionary<UIPanelType, string>();
        TextAsset textAsset = Resources.Load<TextAsset>("UIPanelType");
        //将json对象转化为UIPanelTypeJson类
        UIPanelTypeJson jsonObject = JsonUtility.FromJson<UIPanelTypeJson>(textAsset.text);
        foreach (UIPanelInformation info in jsonObject.infoList)
        {
            mPanelPathDictionary.Add(info.panelType, info.path);
        }
    }
    ///<summary>
    ///根据类型获得相应的界面
    ///</summary>
    ///<param name="panelType">指定页面类型</param>
    ///<returns>返回该页面的BasePanel</returns>
    private BasePanel GetPanel(UIPanelType panelType)
    {
        if (mPanelPool == null)
        {
            mPanelPool = new Dictionary<UIPanelType, BasePanel>();
        }
        BasePanel panel;
        //从页面池中尝试找到指定界面的实例
        mPanelPool.TryGetValue(panelType, out panel);
        if (panel == null)
        {
            //如果找不到，从配置中获得该界面路径进行实例化
            mPanelPool.Remove(panelType);
            string path;
            mPanelPathDictionary.TryGetValue(panelType, out path);

            GameObject instancePanel = Instantiate(Resources.Load("Prefabs/" + path) as GameObject);
            if (instancePanel != null)
            {
                if (mUIRootTransform == null)
                {
                    mUIRootTransform = GameObject.Find("Canvas").transform;
                }
                instancePanel.transform.SetParent(mUIRootTransform, false);
                var targetPanel = instancePanel.GetComponent<BasePanel>();
                mPanelPool.Add(panelType, targetPanel);
                return targetPanel;
            }
        }
        return panel;
    }

    ///<summary>
    ///显示指定面板
    ///</summary>
    ///<param name="panelType">指定页面类型</param>
    public void PushPanel(UIPanelType panelType)
    {
        if (mPanelStack == null)
        {
            mPanelStack = new Stack<BasePanel>();
        }
        if (mPanelStack.Count > 0)
        {
            //Peek只获取栈顶的实例，不对栈内部进行操作，Peek调用后栈内容不变
            //Pop获取栈顶的实例，同时会对栈顶数据进行出栈操作。
            var topPanel = mPanelStack.Peek();
            //获取上一个界面，并对上一个界面进行暂停
            topPanel.OnPause();
        }
        BasePanel panel = GetPanel(panelType);
        //最新界面调用进入函数
        panel.OnEnter();
        //将当前界面设置为画布的最前方界面
        panel.transform.SetAsLastSibling();
        //入栈操作
        mPanelStack.Push(panel);
    }
    /// <summary>
    /// 关闭页面并显示新的页面
    /// </summary>
    /// <param name="panelType"></param>
    /// <param name="isPopCurrentPanel">true时, 关闭当前页面; false时, 关闭所有页面</param>
    public void PushPanel(UIPanelType panelType, bool isPopCurrentPanel)
    {
        if (isPopCurrentPanel)
        {
            PopCurrentPanel();
        }
        else
        {
            PopAllPanel();
        }
        PushPanel(panelType);
    }
    /// <summary>
    /// 返回上一个界面
    /// </summary>
    public bool BackToLastPanel()
    {
        //判断当前栈是否为空
        if (mPanelStack == null)
        {
            mPanelStack = new Stack<BasePanel>();
        }
        if (mPanelStack.Count <= 1)
        {
            return false;
        }
        //关闭栈顶界面的显示
        var topPanel = mPanelStack.Pop();
        topPanel.OnExit();
        //恢复当前显示界面的交互
        BasePanel topPanel1 = mPanelStack.Peek();
        topPanel1.OnResume();
        return true;
    }
    /// <summary>
    /// 隐藏当前面板
    /// </summary>
    private void PopCurrentPanel()
    {
        if (mPanelStack == null)
        {
            mPanelStack = new Stack<BasePanel>();
        }
        if (mPanelStack.Count <= 0) return;
        BasePanel topPanel = mPanelStack.Pop();
        topPanel.OnExit();
    }
    /// <summary>
    /// 隐藏所有面板
    /// </summary>
    private void PopAllPanel()
    {
        if (mPanelStack == null)
        {
            mPanelStack = new Stack<BasePanel>();
        }
        if (mPanelStack.Count <= 0) return;
        while (mPanelStack.Count > 0)
        {
            BasePanel topPanel = mPanelStack.Pop();
            topPanel.OnExit();
        }
    }
    /// <summary>
    /// 切换场景前，调用该方法清空当前场景的数据
    /// </summary>
    private void RefreshDataOnSwitchScene()
    {
        mPanelPool.Clear();
        mPanelStack.Clear();
    }

    public void SwitchScene(string SceneName)
    {
        EventCenter.PreAllEvents();
        EventCenter.CleanUp();
        EventCenter.PreAllEvents();
        RefreshDataOnSwitchScene();
        UnityEngine.SceneManagement.SceneManager.LoadScene(SceneName);
    }





}
