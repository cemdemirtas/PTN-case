using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatorMovement : MonoBehaviour
{
    Animator anim;
    [SerializeField] float rotateAngle = 5f;

    private void Start()
    {
      
    }
    private void Update()
    {
        transform.RotateAround(transform.position, Vector3.up, rotateAngle * Time.deltaTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Character") || collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<Animator>().SetBool("die", true);
            if (collision.gameObject.TryGetComponent<Rigidbody>(out Rigidbody rb))
            {
                rb.AddForce(transform.right * 30f, ForceMode.Impulse);
            }
        }
    }
}
