using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class Ranking : MonoBehaviour
{

    public GameObject[] players;
    public Movement playerController;
    public Text text;
    public int placement;
    private GameObject player;
    void Start()
    {
        player = playerController.gameObject;
    }
    void Update()
    {
        
            players = players.OrderBy(x => x.transform.position.z).ToArray();
            text.text = "Rank :" + (players.Length - Array.IndexOf(players, player)) + "/" + players.Length + "";
        
    }

    public int FindPlacement(GameObject gameObject)
    {
        return (players.Length - Array.IndexOf(players, gameObject));
    }
}


