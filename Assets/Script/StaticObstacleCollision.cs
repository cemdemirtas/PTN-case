using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class StaticObstacleCollision : MonoBehaviour
{
    [SerializeField] private GameObject startReference;
    [SerializeField] float countdown = 0.1f;
    private Vector3 startPos;

    private void Start()
    {
        startPos = startReference.transform.position;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("Player"))
        {
            //collision.transform.GetComponent<Movement>().enabled = false;

            collision.transform.gameObject.GetComponent<Animator>().SetBool("run", false);
            startPos.y = -0.0159f;
            collision.transform.position = startPos;

        }
        //if (collision.gameObject.TryGetComponent<Rigidbody>(out Rigidbody rb))
        //{
        //    rb.AddForce(transform.right * 30f, ForceMode.Impulse);
        //}
    }
}


