using System;
using System.Collections;
using System.Collections.Generic;
//using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;
using Random = System.Random;

public class OpponentNav : MonoBehaviour
{
    [SerializeField] private float rotateSpeed;
    [SerializeField] private float horizontalSpeed;
    [SerializeField] private float forwardSpeed;
    [SerializeField] private Vector3 agentVelocity;
    [SerializeField] private Vector3 move;
    public GameObject finishline;
    public Rigidbody rb;
    public bool isFinished;
    private NavMeshAgent agent;
    private bool blocked = false;
    private bool blockedCrossRight = false;
    private bool blockedCrossLeft = false;
    private bool blockedRight = false;
    private bool blockedLeft = false;
    private NavMeshHit hit;
    private Vector3 dest;
    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
        dest = finishline.transform.position;
        dest.y = transform.position.y;
        agent.destination = dest;
    }

    // Update is called once per frame
    void Update()
    {
        blocked = NavMesh.Raycast(transform.position, transform.position + 2 * Vector3.forward, out hit, NavMesh.AllAreas);
        blockedCrossLeft = NavMesh.Raycast(transform.position, transform.position + Vector3.forward + Vector3.left, out hit, NavMesh.AllAreas);
        blockedCrossRight = NavMesh.Raycast(transform.position, transform.position + Vector3.forward + Vector3.right, out hit, NavMesh.AllAreas);
        blockedLeft = NavMesh.Raycast(transform.position, transform.position + Vector3.left, out hit, NavMesh.AllAreas);
        blockedRight = NavMesh.Raycast(transform.position, transform.position + Vector3.right, out hit, NavMesh.AllAreas);
        Debug.DrawLine(transform.position, transform.position + Vector3.forward * 2 + Vector3.left, blockedCrossLeft ? Color.red : Color.green);
        Debug.DrawLine(transform.position, transform.position + Vector3.forward * 2 + Vector3.right, blockedCrossRight ? Color.red : Color.green);
        Debug.DrawLine(transform.position, transform.position + 2 * Vector3.forward, blocked ? Color.red : Color.green);
        Debug.DrawLine(transform.position, transform.position + Vector3.left, blockedLeft ? Color.red : Color.green);
        Debug.DrawLine(transform.position, transform.position + Vector3.right, blockedRight ? Color.red : Color.green);
    }   

    private void FixedUpdate()
    {
        //Could add more cases but this allows a range of difficulties for bots which makes them more natural
        if (!isFinished)
        {
            agentVelocity.z = forwardSpeed;
            agentVelocity.y = 0;
            if (blocked&& blockedCrossLeft)
            {
                agentVelocity.x = horizontalSpeed;
            }
            else if (blocked&& blockedCrossRight)
            {
                agentVelocity.x = -horizontalSpeed;
            }
            else if (blockedCrossRight&&blockedCrossLeft)
            {
                agentVelocity.x = 0;
            }
            else if (blockedRight)
            {
                agentVelocity.x = -horizontalSpeed;
            }
            else if (blockedLeft)
            {
                agentVelocity.x = horizontalSpeed;
            }
            else
            {
                agentVelocity.x = 0;
            }
            rb.velocity = agentVelocity * Time.deltaTime;
        }
        else
        {
            rb.velocity = Vector3.zero;
        }

    }
    private void LateUpdate()
    {
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(agentVelocity), Time.deltaTime * rotateSpeed);
    }
}
