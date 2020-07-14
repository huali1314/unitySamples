using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitRTS : MonoBehaviour
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
        GetComponent<MoveToTarget>().MoveToPosiiton(targetPosition);
    }
}
