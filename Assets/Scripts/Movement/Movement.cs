using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{

    public float acceleration = 4f;
    public float moveSpeed = 10f;
    public float jumpHeight = 250f;

    public float horizontal;
    public float vertical;

    Rigidbody rb;

    // Use this for initialization
    public virtual void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    public virtual void Update()
    {
        Move(new Vector3(horizontal, 0f, vertical) * moveSpeed);
    }

    void Move(Vector3 translation)
    {
        Vector3 localVel = transform.InverseTransformDirection(rb.velocity);
        rb.AddRelativeForce((translation - localVel) * acceleration * Time.deltaTime);
    }

    public void Jump()
    {
        if (IsGrounded())
        {
            rb.AddForce(Vector3.up * jumpHeight);
        }
    }

    bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, 1.1f);
    }
}
