using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatorMovement : MonoBehaviour
{
    [SerializeField] float rotateAngle = 5f;
    private void Update()
    {
        transform.RotateAround(transform.position, Vector3.up, rotateAngle * Time.deltaTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Character") || collision.gameObject.CompareTag("Player"))
        {
            if (collision.gameObject.TryGetComponent<Rigidbody>(out Rigidbody rb))
            {
                rb.AddForce(transform.right * 30f, ForceMode.Impulse);
            }
        }
    }
}
