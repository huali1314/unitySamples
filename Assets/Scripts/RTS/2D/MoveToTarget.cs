using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToTarget : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed;
    private Vector3 targetPositon;
    private Rigidbody2D rigidbody2D;
    // Start is called before the first frame update
    void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();   
    }
    private void FixedUpdate()
    {
        if (Vector2.Distance(transform.position, targetPositon) < .1f)
        {
            rigidbody2D.velocity = Vector3.zero;
        }
    }
    public void MoveToPosiiton(Vector3 targetPositon)
    {
        this.targetPositon = targetPositon;
        Vector3 moveDir = (targetPositon - transform.position).normalized;
        rigidbody2D.velocity = moveSpeed * moveDir;
    }
}
