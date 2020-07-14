using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class Test : MonoBehaviour
{
    [SerializeField]
    GameObject gameObject;
    [SerializeField]
    Transform[] transform;
    [SerializeField]
    Transform terrain;
    [SerializeField]
    Transform hero;
    NavMeshAgent navMeshAgent;
    Vector3 velocity;
    private void Awake()
    {
        //for (int i = 0; i < transform.Length; i++)
        //{
        //    RVO.Vector2 pos = new RVO.Vector2(transform[i].position.x,transform[i].position.z);
        //    RVOManager.Instance.AddObstacles(pos);
        //}
        //navMeshAgent = hero.GetComponent<NavMeshAgent>();
    }
    private void Update()
    {
        //if (Input.GetMouseButtonDown(0))
        //{
        //    Vector3 targetPos = MyUtils.GetScreenToMapPosition();
        //    Vector3 curPos = hero.position;
        //    velocity = (targetPos - curPos).normalized * 5;
        //    navMeshAgent.SetDestination(targetPos);
        //    //navMeshAgent.Move(velocity * Time.deltaTime);
        //}
        
        //navMeshAgent.Move(velocity * Time.deltaTime);
    }
}
