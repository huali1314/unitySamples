using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
[RequireComponent(typeof(CanvasGroup))]
public class BasePanel : MonoBehaviour
{
	private Dictionary<Int32,Delegate> mCurPanelEventDic = new Dictionary<Int32,Delegate>();
	protected CanvasGroup canvasGroup;
	protected new string name;
	protected virtual void Awake(){
		name = GetType() + ">>";
		UnityEngine.Debug.Log("type = " + name);
		canvasGroup = gameObject.GetComponent<CanvasGroup>();
	}
	///<summary>
	///开启交互，界面显示
	///</summary>
	public virtual void OnEnter(){
        SetPanelAlpha(1);
        //gameObject.transform.localScale = Vector3.one;
        SetPanelInteractable(true);
		SetPanelBlockRaycast(true);
	}
	///<summary>
	///界面暂停，关闭交互
	///</summary>
	public virtual void OnPause(){
		SetPanelInteractable(false);
	}
	///<summary>
	///界面唤醒，恢复交互
	///</summary>
	public virtual void OnResume(){
		SetPanelInteractable(true);
	}
	///<summary>
	///界面退出，关闭界面
	///</summary>
	public virtual void OnExit(){
        SetPanelAlpha(0);
        //gameObject.transform.localScale = Vector3.zero;
        SetPanelInteractable(false);
		SetPanelBlockRaycast(false);
		UnmapAllEventListeners();
	}
	protected void  MapEventListener(Int32 eventType,Action handler){
		EventCenter.AddListener(eventType,handler);
		if (mCurPanelEventDic.ContainsKey(eventType)){
			mCurPanelEventDic[eventType] = (Action)mCurPanelEventDic[eventType] + handler;
		}else{
			mCurPanelEventDic.Add(eventType,handler);
		}
	}
	protected void  MapEventListener<T>(Int32 eventType,Action<T> handler){
		EventCenter.AddListener<T>(eventType,handler);
		if (mCurPanelEventDic.ContainsKey(eventType)){
			mCurPanelEventDic[eventType] = (Action<T>)mCurPanelEventDic[eventType] + handler;
		}else{
			mCurPanelEventDic.Add(eventType,handler);
		}
	}
	protected void  MapEventListener<T,U>(Int32 eventType,Action<T,U> handler){
		EventCenter.AddListener<T,U>(eventType,handler);
		if (mCurPanelEventDic.ContainsKey(eventType)){
			mCurPanelEventDic[eventType] = (Action<T,U>)mCurPanelEventDic[eventType] + handler;
		}else{
			mCurPanelEventDic.Add(eventType,handler);
		}
	}
	protected void  MapEventListener<T,U,V>(Int32 eventType,Action<T,U,V> handler){
		EventCenter.AddListener<T,U,V>(eventType,handler);
		if (mCurPanelEventDic.ContainsKey(eventType)){
			mCurPanelEventDic[eventType] = (Action<T,U,V>)mCurPanelEventDic[eventType] + handler;
		}else{
			mCurPanelEventDic.Add(eventType,handler);
		}
	}
	protected void  MapEventListener<T,U,V,X>(Int32 eventType,Action<T,U,V,X> handler){
		EventCenter.AddListener<T,U,V,X>(eventType,handler);
		if (mCurPanelEventDic.ContainsKey(eventType)){
			mCurPanelEventDic[eventType] = (Action<T,U,V,X>)mCurPanelEventDic[eventType] + handler;
		}else{
			mCurPanelEventDic.Add(eventType,handler);
		}
	}
	protected void  UnmapEventListener(Int32 eventType,Action handler){
		EventCenter.RemoveListener(eventType,handler);
		if(mCurPanelEventDic.ContainsKey(eventType)){
			mCurPanelEventDic[eventType] = (Action)mCurPanelEventDic[eventType] - handler;
		}
	}
	protected void  UnmapEventListener<T>(Int32 eventType,Action<T> handler){
		EventCenter.RemoveListener<T>(eventType,handler);
		if(mCurPanelEventDic.ContainsKey(eventType)){
			mCurPanelEventDic[eventType] = (Action<T>)mCurPanelEventDic[eventType] - handler;
		}
	}
	protected void  UnmapEventListener<T,U>(Int32 eventType,Action<T,U> handler){
		EventCenter.RemoveListener<T,U>(eventType,handler);
		if(mCurPanelEventDic.ContainsKey(eventType)){
			mCurPanelEventDic[eventType] = (Action<T,U>)mCurPanelEventDic[eventType] - handler;
		}
	}
	protected void  UnmapEventListener<T,U,V>(Int32 eventType,Action<T,U,V> handler){
		EventCenter.RemoveListener<T,U,V>(eventType,handler);
		if(mCurPanelEventDic.ContainsKey(eventType)){
			mCurPanelEventDic[eventType] = (Action<T,U,V>)mCurPanelEventDic[eventType] - handler;
		}
	}
	protected void  UnmapEventListener<T,U,V,X>(Int32 eventType,Action<T,U,V,X> handler){
		EventCenter.RemoveListener<T,U,V,X>(eventType,handler);
		if(mCurPanelEventDic.ContainsKey(eventType)){
			mCurPanelEventDic[eventType] = (Action<T,U,V,X>)mCurPanelEventDic[eventType] - handler;
		}
	}
	protected void Broadcast(Int32 eventType){
		EventCenter.Broadcast(eventType);
	}
	protected void Broadcast<T>(Int32 eventType,T arg1){
		EventCenter.Broadcast(eventType,arg1);
	}
	protected void Broadcast<T,U>(Int32 eventType,T arg1,U arg2){
		EventCenter.Broadcast(eventType,arg1,arg2);
	}
	protected void Broadcast<T,U,V>(Int32 eventType,T arg1,U arg2,V arg3){
		EventCenter.Broadcast(eventType,arg1,arg2,arg3);
	}
	protected void Broadcast<T,U,V,X>(Int32 eventType,T arg1,U arg2,V arg3,X arg4){
		EventCenter.Broadcast(eventType,arg1,arg2,arg3,arg4);
	}
    //解绑所有的事件监听
	private void UnmapAllEventListeners(){
		foreach (KeyValuePair<Int32,Delegate> pair in mCurPanelEventDic){
			UnmapEventListener(pair.Key,(Action)pair.Value);
			UnityEngine.Debug.Log("UnmapEventListener Key:" + pair.Key);
		}
	}
    //获取根物体上的Canvas Group组件设置其alpha值，1:显示 0：不显示
    //优点：相较于将界面元素移出屏幕，设置active状态和改变缩放的方式消耗更少，且物体挂载的脚本可以正常的接收事件和消息。
	private void SetPanelAlpha(int alpha)
	{
		if (canvasGroup == null)
		{
			this.gameObject.AddComponent<CanvasGroup>();
			canvasGroup = gameObject.GetComponent<CanvasGroup>();
		}
        if(canvasGroup.alpha != alpha)
		    canvasGroup.alpha = alpha;
    }
    //开销较大，弃用（未验证）
	private void SetPanelActive(bool isActive){
		if(isActive ^ gameObject.activeSelf){
			gameObject.SetActive(isActive);
		}
	}
	private void SetPanelInteractable(bool isInteractable){
		if (canvasGroup == null){
			this.gameObject.AddComponent<CanvasGroup>();
			canvasGroup = gameObject.GetComponent<CanvasGroup>();
		}
		if(isInteractable ^ canvasGroup.interactable){
			canvasGroup.interactable = isInteractable;
		}
	}
	private void SetPanelBlockRaycast(bool isBlockRaycast)
	{
		if (canvasGroup == null)
		{
			this.gameObject.AddComponent<CanvasGroup>();
			canvasGroup = gameObject.GetComponent<CanvasGroup>();
		}
		if (isBlockRaycast ^ canvasGroup.blocksRaycasts)
		{
			canvasGroup.blocksRaycasts = isBlockRaycast;
		}
	}
}
