using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
enum TEST{
    TEST1 = 1,
    TEST2
};
public class LoginPanel : BasePanel
{
	private Button login_btn;
	private Button select_server_btn;
	protected override void Awake() {
        base.Awake();
        login_btn = GameObject.Find("login_btn").GetComponent<Button>();
		select_server_btn = GameObject.Find("server_bg").GetComponent<Button>();
    }
    // Start is called before the first frame update
    void Start()
    {
        login_btn.onClick.AddListener(()=>{
        	OnClickLogin();
        	});
        select_server_btn.onClick.AddListener(()=>{
        	OnClickSelectServer();
        	});
    }
    public override void OnEnter(){
    	base.OnEnter();
        MapEventListener((Int32)TEST.TEST1,SwitchScene);
    	UnityEngine.Debug.Log("=======LoginPanel OnEnter==========");
    }
    public override void OnResume(){
    	base.OnResume();
    	UnityEngine.Debug.Log("=======LoginPanel OnResume==========");
    }
    public override void OnPause(){
    	base.OnPause();
    	UnityEngine.Debug.Log("=======LoginPanel OnPause==========");
    }
    public override void OnExit(){
    	base.OnExit();
    	UnityEngine.Debug.Log("=======LoginPanel OnExit==========");
    }
    void OnClickLogin(){
    	UnityEngine.Debug.Log("=======OnClickLogin==========");
        Broadcast((Int32)TEST.TEST1);
    	// UIManager.Instance.PushPanel(UIPanelType.AlertPanel);
        // AlertPanel.Instance.ShowAlert(SwitchScene, "是否注销当前账号?");

    }
    void OnClickSelectServer(){
    	UnityEngine.Debug.Log("=======OnClickSelectServer==========");
    	UIManager.Instance.PushPanel(UIPanelType.SelectServerPanel);
    }
    void SwitchScene(){
    	UnityEngine.Debug.Log("=========SwitchScene========");
        UIManager.Instance.SwitchScene("Scene2");
    }
}