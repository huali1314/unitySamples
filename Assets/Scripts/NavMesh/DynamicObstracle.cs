using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class DynamicObstracle : MonoBehaviour
{
    NavMeshAgent navMeshAgent;
    Vector3 startPos;
    Vector3 endPos;
    Vector3 curDestiation;
    // Start is called before the first frame update
    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        startPos = transform.position;
        endPos = new Vector3(Random.Range(50, 450), 0.5f, Random.Range(50, 450));
        curDestiation = endPos;
        navMeshAgent.SetDestination(curDestiation);
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(transform.position, curDestiation) < 0.5f)
        {
            if (curDestiation != startPos)
            {
                curDestiation = startPos;
            }
            else
            {
                curDestiation = endPos;
            }

                
            navMeshAgent.SetDestination(curDestiation);
        }
        
    }
}
