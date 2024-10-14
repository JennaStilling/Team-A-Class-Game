using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayTrigger3 : MonoBehaviour
{
    public FlappyBird flappyBirdGame;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            flappyBirdGame.SetPlayerInRange(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            flappyBirdGame.SetPlayerInRange(false);
        }
    }
}
