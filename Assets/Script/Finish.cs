using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Finish : MonoBehaviour
{
    [SerializeField] GameObject board;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("FinishLine"))
        {
            gameObject.SetActive(false); //Player does not move around.
            board.SetActive(true); // Paint frame
            transform.gameObject.GetComponent<Animator>().SetBool("victory", true);

        }
    }
}
