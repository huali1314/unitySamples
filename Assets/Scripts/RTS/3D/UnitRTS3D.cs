using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitRTS3D : MonoBehaviour
{
    private GameObject selectedGameObject;
    private void Awake()
    {
        selectedGameObject = transform.Find("selected").gameObject;
        SetSelectedVisible(false);
    }
    public void SetSelectedVisible(bool visible)
    {
        selectedGameObject.SetActive(visible);
    }
    public void SetTargetPosition(Vector3 targetPosition)
    {
        GetComponent<MoveToTarget3D>().MoveToPosiiton(targetPosition);
    }
}
