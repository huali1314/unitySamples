using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pathFindUnit : MonoBehaviour
{
    private float startTime;
    // Start is called before the first frame update
    void Start()
    {
        AStarManager.Instance.InitWithMapInfo(50, 50);
    }
    void FindPathSuccess(Stack<string> result)
    {
        Debug.Log("endTime=======" + Time.realtimeSinceStartup);
        Debug.Log("diff=============" + (Time.realtimeSinceStartup - startTime));
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            startTime = Time.realtimeSinceStartup;
            Debug.Log("startTime=======" + startTime);
            AStarManager.Instance.FindPath(Vector2.zero, new Vector2(49, 49),FindPathSuccess);
        }
    }
}
