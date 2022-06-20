using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Magiclab.InputHandler;

public class Movement : MonoBehaviour
{
    private float horizontalMove;
    private float verticalMove;
    [SerializeField] float swerveSpeed;
    [SerializeField] public float setZSpeed;
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

    }
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            horizontalMove = InputHandler.GetHorizontal() * swerveSpeed;
            verticalMove = InputHandler.GetVertical() * swerveSpeed;
            transform.position = new Vector3(horizontalMove, transform.position.y, transform.position.z);
            rb.AddForce(Vector3.forward * setZSpeed * Time.deltaTime);

        }
    }
}
