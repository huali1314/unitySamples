using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptimizeDynamicBuild : MonoBehaviour
{
	//保存碰撞体
	Collider buildCollider = null;
	//包围盒
	Bounds bounds;
	bool mVisible = true;
	//子GameObject
	List<GameObject> childObjs = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        buildCollider = GetComponent<Collider>();
        if(buildCollider == null){
        	UnityEngine.Debug.Log("Optimize dynamic building should have collider!");
        }
        //获取子object
        for(int i = 0;i<gameObject.transform.childCount;i++){
        	Transform ts = gameObject.transform.GetChild(i);
        	GameObject psObj = ts.gameObject;
        	if(psObj.tag != "posui" && psObj.name != "hitpoint" && psObj.name != "buffpoint" && psObj.name != "point"){
        		childObjs.Add(psObj);
        	}
        }
    }
    float mTotalTime = 0;
    // Update is called once per frame
    void Update()
    {
        mTotalTime += Time.deltaTime;
        if(mTotalTime <= 0.1){
        	return;
        }else{
        	mTotalTime = 0;
        }
        if(!GameMethod.MainCamera.useOptimizeDynamicBuild){
        	return;
        }
        Plane[] planes = GameMethod.MainCamera.planes;
        if(planes == null){
        	return;
        }
        bounds = buildCollider.bounds;
        //包围盒扩大5倍
        bounds.Expand(5.0f);
        bool isVisible = GameMethod.MainCamera.IsInFrustum(planes,bounds);
        if(isVisible != mVisible){
        	mVisible = isVisible;
        	if(mVisible){
        		setActive(true);
        	}else{
        		setActive(false);
        	}
        }
    }
    void setActive(bool flag){
    	buildCollider.enabled = flag;
    	foreach(GameObject obj in childObjs){
    		obj.SetActive(flag);
    	}
    }
}
