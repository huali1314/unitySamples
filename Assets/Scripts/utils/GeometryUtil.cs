using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeometryUtil: MonoBehaviour
{
    
    Plane [] planes;
    public Camera mainCamera;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Collider collider = transform.GetChild(i).gameObject.GetComponent<Collider>();
            GeometryChild(collider);
        }
        //bounds.Expand(5.0f);
        
    }
    void GeometryChild(Collider collider) {
        if (collider == null) {
            Debug.LogError("warning:" + collider.gameObject.name + "do not have collider!");
            return;
        }
        planes = GeometryUtility.CalculateFrustumPlanes(mainCamera);
        if (planes == null)
        {
            Debug.LogError("=========planes can not be null===============");
            return;
        }
        Bounds bounds = collider.bounds;
        bool isVisible = IsInFrustum(planes, bounds);
        Debug.Log(collider.gameObject.name + ":" + isVisible);
        collider.enabled = isVisible;
        collider.gameObject.SetActive(isVisible);
    }
    //检测包围盒是否可见
    public bool IsInFrustum(Plane[] planes, Bounds bound)
    {
        return GeometryUtility.TestPlanesAABB(planes, bound);
    }
}
