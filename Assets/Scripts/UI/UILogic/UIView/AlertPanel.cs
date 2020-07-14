using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
public class AlertPanel : BasePanel
{
	public Text alertText;
	public Button confirmButton;
	public Button cancelButton;

	public static AlertPanel Instance{get;set;}

	protected override void Awake() {
        base.Awake();
        Instance = this;
        cancelButton.onClick.AddListener(ClosePanel);
    }
    public void ClosePanel(){
    	UIManager.Instance.BackToLastPanel();
    }
    public void ShowAlert(UnityAction confirmAction,string showText){
    	alertText.text = showText;
    	confirmButton.onClick.RemoveAllListeners();
    	confirmButton.onClick.AddListener(confirmAction);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public override void OnEnter(){
    	base.OnEnter();
    	UnityEngine.Debug.Log("==========AlertPanel OnEnter====");
    }
    public override void OnResume(){
    	base.OnResume();
    	UnityEngine.Debug.Log("==========AlertPanel OnResume====");
    }
    public override void OnPause(){
    	base.OnPause();
    	UnityEngine.Debug.Log("==========AlertPanel OnPause====");
    }
    public override void OnExit(){
    	base.OnExit();
    	UnityEngine.Debug.Log("==========AlertPanel OnExit====");
    }
}
