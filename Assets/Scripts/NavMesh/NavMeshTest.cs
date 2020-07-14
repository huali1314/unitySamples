using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class NavMeshTest : MonoBehaviour
{
    NavMeshAgent navMeshAgent;
    Vector3 destination;
    // Start is called before the first frame update
    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.speed = 5f;
        destination = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (!navMeshAgent.isStopped && Vector3.Distance(transform.position, destination) < 0.5f)
        {
            navMeshAgent.isStopped = true;

        }
        if (Input.GetMouseButtonDown(0))
        {
            navMeshAgent.isStopped = false;
            destination = MyUtils.GetScreenToMapPosition();
            navMeshAgent.SetDestination(destination);
        }
    }
    private void OnGUI()
    {
        if (GUI.Button(new Rect(0, 0, 100, 40), "ADD"))
        {
            navMeshAgent.speed += 1;
        }
        if (GUI.Button(new Rect(0, 40, 100, 40), "SUB"))
        {
            navMeshAgent.speed -= 1;
        }
    }
}
