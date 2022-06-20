using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingObstacleMovement : MonoBehaviour
{
    [SerializeField] Vector3 eulerAngleVelocity;
    
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    private void FixedUpdate()
    {       
        Quaternion deltaRotation = Quaternion.Euler(eulerAngleVelocity * Time.fixedDeltaTime);
        rb.MoveRotation(rb.rotation * deltaRotation);
    }
}
