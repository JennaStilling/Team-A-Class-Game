using System.Collections;
using UnityEngine;


public class PlayTrigger : MonoBehaviour
{
    public SpaceInvadersGame spaceInvaders;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            spaceInvaders.SetPlayerInRange(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            spaceInvaders.SetPlayerInRange(false);
        }
    }
}
