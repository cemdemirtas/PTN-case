using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class DonutForwarder : MonoBehaviour
{
    [SerializeField] float thrownPosition = -0.1f;
    [SerializeField] float throwInterval = 0.5f;
    [SerializeField] float pulledPosition = 0.1f;
    [SerializeField] Transform halfDonutModel;

    private bool isStick;
    private float characterStartPosZ = -231f;
    private void Start()
    {
        var sequence = DOTween.Sequence().SetLoops(-1);
        sequence.Append(halfDonutModel.DOLocalMoveX(thrownPosition, throwInterval));
        sequence.AppendInterval(throwInterval);
        sequence.Append(halfDonutModel.DOLocalMoveX(pulledPosition, 1));        
    }

    private void Update()
    {
        if (isStick == true) return;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("Player"))
        {            
            if (collision.gameObject.TryGetComponent<Rigidbody>(out Rigidbody rb))
            {               
                rb.AddForce(transform.forward * -1, ForceMode.Impulse);
            }          
        }
    }

}
