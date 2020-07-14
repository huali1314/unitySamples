using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
public class EventCenter
{
	//存储事件数据的字典 key：事件类型，Value：对应的代理方法
	public static Dictionary<Int32,Delegate> mEventDic = new Dictionary<Int32,Delegate>();
	//存储永久性事件的列表
	public static List<Int32> mPermanentMessage = new List<Int32>();
	//添加事件 无参数
    public static void AddListener(Int32 eventType,Action handler){
    	OnListenerAdding(eventType,handler);
    	mEventDic[eventType] = (Action)mEventDic[eventType] + handler;
    }
    //添加事件 一个参数
    public static void AddListener<T>(Int32 eventType,Action<T> handler){
    	OnListenerAdding(eventType,handler);
    	mEventDic[eventType] = (Action<T>)mEventDic[eventType] + handler;
    }
    //添加事件 两个参数
    public static void AddListener<T,U>(Int32 eventType,Action<T,U> handler){
    	OnListenerAdding(eventType,handler);
    	mEventDic[eventType] = (Action<T,U>)mEventDic[eventType] + handler;
    }
    //添加事件 三个参数
    public static void AddListener<T,U,V>(Int32 eventType,Action<T,U,V> handler){
    	OnListenerAdding(eventType,handler);
    	mEventDic[eventType] = (Action<T,U,V>)mEventDic[eventType] + handler;
    }
    //添加事件 四个参数
    public static void AddListener<T,U,V,X>(Int32 eventType,Action<T,U,V,X> handler){
    	OnListenerAdding(eventType,handler);
    	mEventDic[eventType] = (Action<T,U,V,X>)mEventDic[eventType] + handler;
    }
    //删除事件 无参数
    public static void RemoveListener(Int32 eventType,Action handler){
    	OnListenerRemoving(eventType,handler);
    	mEventDic[eventType] = (Action)mEventDic[eventType] - handler;
    	OnListenerRemoved(eventType);
    }
    //删除事件 一个参数
     public static void RemoveListener<T>(Int32 eventType,Action<T> handler){
    	OnListenerRemoving(eventType,handler);
    	mEventDic[eventType] = (Action<T>)mEventDic[eventType] - handler;
    	OnListenerRemoved(eventType);
    }
    //删除事件 两个参数
     public static void RemoveListener<T,U>(Int32 eventType,Action<T,U> handler){
    	OnListenerRemoving(eventType,handler);
    	mEventDic[eventType] = (Action<T,U>)mEventDic[eventType] - handler;
    	OnListenerRemoved(eventType);
    }
    //删除事件 三个参数
     public static void RemoveListener<T,U,V>(Int32 eventType,Action<T,U,V> handler){
    	OnListenerRemoving(eventType,handler);
    	mEventDic[eventType] = (Action<T,U,V>)mEventDic[eventType] - handler;
    	OnListenerRemoved(eventType);
    }
    //删除事件 四个参数
    public static void RemoveListener<T,U,V,X>(Int32 eventType,Action<T,U,V,X> handler){
    	OnListenerRemoving(eventType,handler);
    	mEventDic[eventType] = (Action<T,U,V,X>)mEventDic[eventType] - handler;
    	OnListenerRemoved(eventType);
    }
    //广播事件 无参数
    public static void Broadcast(Int32 eventType){
    	Delegate d;
    	if(mEventDic.TryGetValue(eventType,out d)){
    		Action callback = d as Action;
    		if(callback != null){
    			callback();
    		}else{
    			throw createBoradcastSignatureException(eventType);
    		}
    	}
    }
    //广播事件 一个参数
    public static void Broadcast<T>(Int32 eventType,T arg1){
    	Delegate d;
    	if(mEventDic.TryGetValue(eventType,out d)){
    		Action<T> callback = d as Action<T>;
    		if(callback != null){
    			callback(arg1);
    		}else{
    			throw createBoradcastSignatureException(eventType);
    		}
    	}
    }
    //广播事件 两个参数
    public static void Broadcast<T,U>(Int32 eventType,T arg1,U arg2){
    	Delegate d;
    	if(mEventDic.TryGetValue(eventType,out d)){
    		Action<T,U> callback = d as Action<T,U>;
    		if(callback != null){
    			callback(arg1,arg2);
    		}else{
    			throw createBoradcastSignatureException(eventType);
    		}
    	}
    }
    //广播事件 三个参数
    public static void Broadcast<T,U,V>(Int32 eventType,T arg1,U arg2,V arg3){
    	Delegate d;
    	if(mEventDic.TryGetValue(eventType,out d)){
    		Action<T,U,V> callback = d as Action<T,U,V>;
    		if(callback != null){
    			callback(arg1,arg2,arg3);
    		}else{
    			throw createBoradcastSignatureException(eventType);
    		}
    	}
    }
    //广播事件 四个参数
    public static void Broadcast<T,U,V,X>(Int32 eventType,T arg1,U arg2,V arg3,X arg4){
    	Delegate d;
    	if(mEventDic.TryGetValue(eventType,out d)){
    		Action<T,U,V,X> callback = d as Action<T,U,V,X>;
    		if(callback != null){
    			callback(arg1,arg2,arg3,arg4);
    		}else{
    			throw createBoradcastSignatureException(eventType);
    		}
    	}
    }
    public static void MarkAsPermanent(Int32 eventType){
    	mPermanentMessage.Add(eventType);
    }
    public static void CleanUp(){
    	List<Int32> messageToRemove = new List<Int32>();
    	//原型：C# KeyValuePair<TKey,TValue> 定义一个可设置或检索的变量
    	foreach(KeyValuePair<Int32,Delegate> pair in mEventDic){
    		bool wasFound = false;
    		foreach(Int32 message in mPermanentMessage){
    			if(pair.Key == message){
    				wasFound = true;
    				break;
    			}
    		}
    		if(!wasFound){
    			messageToRemove.Add(pair.Key);
    		}
    	}
    	foreach(Int32 message in messageToRemove){
    		mEventDic.Remove(message);
    	}
    }
    public static void PreAllEvents(){
    	UnityEngine.Debug.Log("===========All Events===========");
    	foreach(KeyValuePair<Int32,Delegate> pair in mEventDic){
    		UnityEngine.Debug.Log("key:" + pair.Key + "\tvalue:" + pair.Value);
    	}
    }

    public static void OnListenerAdding(Int32 eventType,Delegate lisenerBeingAdd){
    	if(!mEventDic.ContainsKey(eventType)){
    		mEventDic.Add(eventType,null);
    	}
    	Delegate d = mEventDic[eventType];
    	if(d != null && d.GetType() != lisenerBeingAdd.GetType()){
    		throw new ListenerException(string.Format("Attempting to add listener with inconsistent signature for event type{0},Current listeners have type {1} and listener being added has type {2}",eventType,d.GetType().Name,lisenerBeingAdd.GetType().Name));
    	}
    }
    public static void OnListenerRemoving(Int32 eventType,Delegate lisenerBeingRemoved){
    	if(mEventDic.ContainsKey(eventType)){
    		Delegate d = mEventDic[eventType];
    		if(d == null){
    			throw new ListenerException(string.Format("Attempting to remove listener with for event type \"{0}\" but current listener is null.",eventType));
    		}else if(d.GetType() != lisenerBeingRemoved.GetType()){
    			throw new ListenerException(string.Format("Attempting to remove listener with inconsistent signature for event type {0},Current listeners have type {1} and listener being removed has type{2}",eventType,d.GetType().Name,lisenerBeingRemoved.GetType().Name));
    		}else{
    			throw new ListenerException(string.Format("Attempting to remove listener for type \"{0}\" but Message doesn't know about this event type",eventType));
    		}
    	}
    }
    // public internal static void Broadcast(object userEvent_NetMessage_NotifyMatchTeamSwitch){
    // 	throw new NotImplementedException();
    // }
    public static void OnListenerRemoved(Int32 eventType){
    	if(mEventDic.ContainsKey(eventType)){
    		mEventDic.Remove(eventType);
    	}
    }
    public static void OnBroadcasting(Int32 eventType){

    }
    public class ListenerException:Exception{
    	public ListenerException(string msg):base(msg){

    	}
    }
    public static ListenerException createBoradcastSignatureException(Int32 eventType){
    	return new ListenerException(string.Format("Broadcasting message \"{0}\" but listeners have a different signature than the broadcaster.",eventType));
    }





}
