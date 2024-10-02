using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playTrigger2 : MonoBehaviour
{
    public Pong pongGame;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            pongGame.SetPlayerInRange(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            pongGame.SetPlayerInRange(false);
        }
    }
}
