using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCollision : MonoBehaviour
{
    [SerializeField] private GameObject startReference;
    [SerializeField] private Vector3 moveDirection;
    [SerializeField] private int thrust;
    public EnemyNav nav;
    //public ScoreBoard scoreBoard;
    private Rigidbody rb;
    private Animator animator;
    private Vector3 startPos;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        nav = GetComponent<EnemyNav>();
        animator = GetComponent<Animator>();
        startPos = startReference.transform.position;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            gameObject.transform.position = startPos;
        }
        else if (collision.gameObject.CompareTag("RotPlat"))
        {
            gameObject.transform.parent = collision.gameObject.transform;
        }
        else if(collision.gameObject.CompareTag("RotStick"))
        {
            moveDirection = collision.gameObject.transform.parent.GetComponent<RotatingStick>().direction*collision.transform.right;
            moveDirection.y = 0;
            rb.AddRelativeForce( moveDirection.normalized *Time.deltaTime*thrust,ForceMode.Impulse);
        }
        else if (collision.gameObject.CompareTag("Floor"))
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
            if (scoreBoard.FindPlacement(this.gameObject)==1)
            {
                animator.SetBool("hasWon",true);
            }
            else
            {
                animator.SetBool("hasLost",true);
            }
        }
    }
}
