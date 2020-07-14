using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameMethod
{
	private static Camera m_uiCamera = null;
    public static Camera UICamera{
    	get{
    		if(m_uiCamera == null){
    			m_uiCamera = UIManager.Instance.transform.Find("UICamera").GetComponent<Camera>();
    		}
    		return m_uiCamera;
    	}
    }
    private static SmoothFollow m_mainCamera = null;
    public static SmoothFollow MainCamera{
    	get{
    		if(m_mainCamera == null){
    			m_mainCamera = Camera.main.GetComponent<SmoothFollow>();
    		}
    		return m_mainCamera;
    	}
    }
}
