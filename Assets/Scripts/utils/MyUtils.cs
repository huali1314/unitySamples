using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MyUtils
{
    public static Vector3 GetMouseWorldPosition()
    {
        return Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    public static Vector3 GetScreenToMapPosition()
    {

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit raycastHit;
        bool result = Physics.Raycast(ray, out raycastHit,1000f);
        if (result)
        {
            //Debug.Log(" ============name=====" + raycastHit.collider.gameObject.name);
            return raycastHit.point;   
        }
        return Vector3.zero;
    }
}
