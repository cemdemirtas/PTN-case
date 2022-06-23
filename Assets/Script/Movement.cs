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
    void FixedUpdate()
    {
        if (Input.GetMouseButton(0))
        {
            horizontalMove = InputHandler.GetHorizontal() * swerveSpeed;
            verticalMove = InputHandler.GetVertical() * swerveSpeed;
            transform.position = new Vector3(horizontalMove, transform.position.y, transform.position.z);
            rb.AddForce(Vector3.forward * setZSpeed * Time.deltaTime);
            transform.gameObject.GetComponent<Animator>().SetBool("run", true);
            transform.gameObject.GetComponent<Animator>().SetBool("die", false);


        }
        transform.rotation = new Quaternion(0,0,0,0);

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "ObstacleRing") //fix rotation and x,y,z constrain when we move across the ring bridge

        {
            Debug.Log("begin");
            rb.constraints = RigidbodyConstraints.None;
            rb.constraints = RigidbodyConstraints.FreezeRotation;
        }
        if (other.gameObject.tag == "ObstacleRingEnd") //Rigidbody X,Y constrain

        {
            Debug.Log("end");
            rb.constraints = RigidbodyConstraints.FreezeRotation;
            rb.constraints = RigidbodyConstraints.FreezePositionX;
            rb.constraints = RigidbodyConstraints.FreezePositionX;
            rb.constraints = RigidbodyConstraints.FreezePositionY;
            rb.constraints = RigidbodyConstraints.FreezePositionX;



        }
    }
    private void Update()
    {
         swerveSpeed=0.25f;
        setZSpeed=3;
    }
}
