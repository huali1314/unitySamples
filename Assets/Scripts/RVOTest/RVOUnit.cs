using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RVO;
using System;

public class RVOUnit : MonoBehaviour
{
    
    int agentID;
    Vector3 vecolity;
    RVO.Vector2 goalPos;
    RVO.Vector2 agentVecolity;
    RVO.Vector2 prefVecolity;
    Rigidbody rigidbody;
    public int speed;
    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        speed = 5;
    }
    // Start is called before the first frame update
    void Start()
    {
        /*
         * Specify the default parameters for agents that are subsequently
         * added.
         */
        RVO.Vector2 position = new RVO.Vector2(transform.position.x, transform.position.z);
        goalPos = new RVO.Vector2(UnityEngine.Random.Range(100,190), UnityEngine.Random.Range(100, 190));
        prefVecolity = RVO.RVOMath.normalize(goalPos - position);
        agentID = RVOManager.Instance.AddAgent(position, goalPos);
        Simulator.Instance.setAgentPrefVelocity(agentID, prefVecolity);
        
    }
    private void FixedUpdate()
    {
        agentVecolity = Simulator.Instance.getAgentVelocity(agentID);
        vecolity.x = agentVecolity.x();
        vecolity.y = 0;
        vecolity.z = agentVecolity.y();
        rigidbody.velocity = vecolity * speed;
    }
}
