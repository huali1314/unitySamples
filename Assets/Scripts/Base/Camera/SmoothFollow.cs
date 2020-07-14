using UnityEngine;

public class SmoothFollow : MonoBehaviour
{
	//摄像机跟随目标
	public Transform target;
	//在x-z平面上与目标的距离
	public float distance = 10.0f;
	//摄像机在目标上方的高度
	public float height = 5.0f;
	//高度跟随参数
	public float heightDamping = 2.0f;
	//旋转跟随参数
	public float rotationDamping = 3.0f;

	//当前帧数值，用来检测是否重复更新
	private int curFrame = -1;

	//是否优化动态建筑显示
	public bool useOptimizeDynamicBuild = true;
	//是否优化静态特效显示
	public bool useOptimizeStaticEffect = true;
	//是否优化动态模型显示
	public bool useOptimizeDynamicModel = true;

	//相机视截体
	public Plane[] planes;
	public Camera mainCamera = null;
	public bool CameraMoving;
	public Transform mTransform = null;
    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
    }

    public Transform GetTransform(){
    	if(mTransform == null){
    		mTransform = gameObject.transform;
    	}
    	return mTransform;
    }
    //创建视平截体
    public void RefreshFrustumPlanes(){
    	if(!useOptimizeDynamicBuild){
    		return;
    	}
    	if(mainCamera == null){
    		return;
    	}
    	planes = GeometryUtility.CalculateFrustumPlanes(mainCamera);
    }
    //每帧更新
    // void FixedUpdate(){
    // 	FixedUpdatePosition();
    // }
    //检测包围盒是否可见
    public bool IsInFrustum(Plane[] planes,Bounds bound){
    	return GeometryUtility.TestPlanesAABB(planes,bound);
    }
    public void FixedUpdatePosition(){
    	if(target == null || CameraMoving){
    		return;
    	}
    	//相机位置不随玩家位置高度，高度60固定
    	Vector3 targetPos = new Vector3(target.position.x,60,target.position.z);
    	//计算当前的旋转角度
    	float wantedRotationAngle = target.eulerAngles.y;
    	float wantedHeight = targetPos.y + height;

    	float currentRotationAngle = transform.eulerAngles.y;
    	float currentHeight = transform.position.y;

    	//y轴角度差值计算
    	currentHeight = Mathf.Lerp(currentHeight,wantedHeight,heightDamping * Time.deltaTime);
    	//转换角度到旋转中
    	Quaternion wantRotation = Quaternion.Euler(0,currentRotationAngle,0);
    	Vector3 wantPos = targetPos - wantRotation * Vector3.forward * distance;
    	wantPos.y = currentHeight;
    	Vector3 nowPos = GetTransform().position;
    	float mtoDis = Vector3.Distance(wantPos,nowPos);
    	GetTransform().position = wantPos;
    	Vector3 dir = targetPos = GetTransform().position;
    	dir.Normalize();
    	GetTransform().rotation = Quaternion.LookRotation(dir);

    	RefreshFrustumPlanes();
    	//检测是否有重复更新
    	if(curFrame == Time.frameCount){
    		UnityEngine.Debug.LogError("update more than once in one frame");
    	}
    	curFrame = Time.frameCount;
    }
}
