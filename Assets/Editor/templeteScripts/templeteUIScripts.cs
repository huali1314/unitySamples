using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class TempleteScript : BasePanel
{
	
	protected override void Awake() {
        base.Awake();
    }
    // Start is called before the first frame update
    void Start()
    {
       
    }
    public override void OnEnter(){
    	base.OnEnter();
    	UnityEngine.Debug.Log("======= OnEnter==========");
    }
    public override void OnResume(){
    	base.OnResume();
    	UnityEngine.Debug.Log("======= OnResume==========");
    }
    public override void OnPause(){
    	base.OnPause();
    	UnityEngine.Debug.Log("======= OnPause==========");
    }
    public override void OnExit(){
    	base.OnExit();
    	UnityEngine.Debug.Log("======= OnExit==========");
    }
}