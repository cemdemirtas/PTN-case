using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCollision : MonoBehaviour
{
    [SerializeField] private GameObject startReference;
    [SerializeField] private Vector3 moveDirection;
    [SerializeField] private int thrust;
    public OpponentNav nav;
    //public ScoreBoard scoreBoard;
    private Rigidbody rb;
    private Animator animator;
    private Vector3 startPos;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        nav = GetComponent<OpponentNav>();
        animator = GetComponent<Animator>();
        startPos = startReference.transform.position;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            rb.AddRelativeForce(moveDirection.normalized * Time.deltaTime * thrust, ForceMode.Impulse);

            gameObject.transform.position = startPos;
        }
        else if (collision.gameObject.CompareTag("ObstacleRing"))
        {
            gameObject.transform.parent = collision.gameObject.transform;
        }
        else if (collision.gameObject.CompareTag("Donut"))
        {
            rb.AddRelativeForce(moveDirection.normalized * Time.deltaTime * thrust, ForceMode.Impulse);

            gameObject.transform.parent = collision.gameObject.transform;
        }
        else if(collision.gameObject.CompareTag("Rotator"))
        {
            //moveDirection = collision.gameObject.transform.parent.GetComponent<RotatingStick>().direction*collision.transform.right;
            //moveDirection.y = 0;
            rb.AddRelativeForce( moveDirection.normalized *Time.deltaTime*thrust,ForceMode.Impulse);
        }
        else if (collision.gameObject.CompareTag("Ground"))
        {
            transform.parent = null;
        }
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("FinishLine") && other.gameObject.GetComponent<MeshRenderer>().enabled==false)
        {
            nav.isFinished = true;
            GetComponent<CapsuleCollider>().enabled = false;
            //if (scoreBoard.FindPlacement(this.gameObject)==1)
            //{
            //    animator.SetBool("hasWon",true);
            //}
            //else
            //{
            //    animator.SetBool("hasLost",true);
            //}
        }
    }
}
