using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToTarget3D : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed;
    private Vector3 targetPositon;
    private Rigidbody rigidbody;
    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }
    private void FixedUpdate()
    {
        if (Vector3.Distance(transform.position, targetPositon) < 5f)
        {
            rigidbody.velocity = Vector3.zero;
        }
    }
    public void MoveToPosiiton(Vector3 targetPositon)
    {
        this.targetPositon = targetPositon;
        this.targetPositon.y = transform.position.y;
        Vector3 startPosition = transform.position;
        Vector3 direction = (this.targetPositon - startPosition).normalized;
        rigidbody.velocity = moveSpeed * direction;
    }
}
