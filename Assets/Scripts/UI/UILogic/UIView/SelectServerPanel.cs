// using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using CircularScrollView;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Events;
public class SelectServerPanel : BasePanel
{
	public UICircularScrollView scrollView;
    public ToggleGroup toggleGroup;
    public Button back_btn;
	private Toggle [] toggles;
    private int m_CurSelectCellIndex = -1;
    // private int m_LastSelectCellIndex = -1;
    private Sprite sp_normal;
    private Sprite sp_press;
    private Animation m_Animation;
    // Start is called before the first frame update
    void Start()
    {
    	UnityEngine.Debug.Log("==========SelectServerPanel Start=====");
    	toggles = transform.GetComponentsInChildren<Toggle>();
    	for(int i = 0;i< toggles.Length;i++){
    		Toggle toggle = toggles[i];
    		toggle.onValueChanged.AddListener((bool isSelected)=>{
    			if (isSelected){
    				OnValueChanged(toggle);
    			}
    		});
    	}
    	back_btn.onClick.AddListener(()=>{
    		OnBackBtnClicked();
    		});
    	sp_normal = Resources.Load("Images/SelectServer/btn_fuwuqi_common1_normal",typeof(Sprite)) as Sprite;
        sp_press = Resources.Load("Images/SelectServer/btn_fuwuqi_common1_press",typeof(Sprite)) as Sprite;
        scrollView.Init(NormalCallback,NormalOnClickCallBack);
        scrollView.ShowList(50);
        scrollView.UpdateList();
    }
    public override void OnEnter(){
    	base.OnEnter();
        UnityEngine.Debug.Log("==========SelectServerPanel OnEnter====");
    }
    public override void OnResume(){
    	base.OnResume();
    	UnityEngine.Debug.Log("==========SelectServerPanel OnResume====");
    }
    public override void OnPause(){
    	base.OnPause();
    	UnityEngine.Debug.Log("==========SelectServerPanel OnPause====");
    }
    public override void OnExit(){
    	base.OnExit();
    	Resources.UnloadAsset(sp_normal);
    	Resources.UnloadAsset(sp_press);
    	Resources.UnloadUnusedAssets();
    	UnityEngine.Debug.Log("==========SelectServerPanel OnExit====");
    }
    private void OnValueChanged(Toggle toggle){
    	// if(toggleGroup.allowSwitchOff == false){
    	// 	toggleGroup.allowSwitchOff = true;
    	// }
    	UnityEngine.Debug.Log("toggle name:" + toggle.name);
    	UIManager.Instance.BackToLastPanel();
    }
    private void OnBackBtnClicked(){
    	UIManager.Instance.BackToLastPanel();
    }
    private void NormalCallback(GameObject cell,int index){
    	Transform server_name_txt = cell.transform.Find("server_name_txt");
    	Transform server_bg = cell.transform.Find("bg");
    	server_name_txt.GetComponent<Text>().text = "Server" + index;
    	if (m_CurSelectCellIndex !=-1 && index == m_CurSelectCellIndex){
    		server_bg.GetComponent<Image>().sprite = sp_press;
    	}else{
    		server_bg.GetComponent<Image>().sprite = sp_normal;
    	}

    }
    private void NormalOnClickCallBack(GameObject cell, int index)
    {
       UnityEngine.Debug.Log("index==========" + index);
       m_CurSelectCellIndex = index;
       scrollView.UpdateList();
       UIManager.Instance.BackToLastPanel();
       // scrollView.UpdateCell(index);
       // if (m_LastSelectCellIndex != -1){
       // 		scrollView.UpdateCell(m_LastSelectCellIndex);
       // }
       // m_LastSelectCellIndex = m_CurSelectCellIndex;
    }
}
