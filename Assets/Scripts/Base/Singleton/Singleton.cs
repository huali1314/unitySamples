using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Singleton<T> where T:new()
{
   private static T _instance;
   static object _lock = new Object();
   public static T Instance{
   		get{
   			if (_instance == null){
   				//加锁
   				lock(_lock){
   					if(_instance == null){
   						_instance = new T();
   					}
   				}
   			}
   			return _instance;
   		}
   }
}

public class UnitySingleton <T>: MonoBehaviour where T:Component
{
  	private static T _instance;
  	public static T Instance{
  		get{
  			if(_instance == null){
  				_instance = FindObjectOfType(typeof(T)) as T;
  				if(_instance == null){
  					GameObject obj = new GameObject();
  					obj.hideFlags = HideFlags.HideAndDontSave;
  					_instance = (T)obj.AddComponent<T>();
  				}
  			}
  			return _instance;
  		}
  	}
  	public virtual void Awake(){
  		DontDestroyOnLoad(this.gameObject);
  		if(_instance == null){
  			_instance = this as T;
  		}else{
  			Destroy(gameObject);
  		}
  	}
}
