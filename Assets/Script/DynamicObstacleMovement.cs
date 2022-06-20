using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class DynamicObstacleMovement : MonoBehaviour
{
    [SerializeField] float movementDuration = 1f;
    [SerializeField] float impactForce = 10f;
    [SerializeField] float leftBoundary = -3f, rightBoundary = 3f;

    private float characterStartPosZ = -231f;

    private void Start()
    {
        transform.DOMove(new Vector3(rightBoundary, transform.position.y, transform.position.z),
            movementDuration).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);

        transform.DOLocalRotate
            (new Vector3(0, 360, 0), movementDuration, RotateMode.FastBeyond360).SetEase
            (Ease.Linear).SetLoops(-1, LoopType.Restart);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent<Rigidbody>(out Rigidbody rb))
        {
            rb.AddForce(collision.gameObject.transform.forward * -1 * impactForce, ForceMode.Impulse);
            if (collision.gameObject.CompareTag("Player"))
            {
                Invoke("ReloadScene", .5f);
            }
            if (collision.gameObject.CompareTag("Character"))
            {
                float characterDeadPos = collision.gameObject.transform.position.z;
                characterDeadPos = characterStartPosZ;
            }
        }        
    }

    private void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
