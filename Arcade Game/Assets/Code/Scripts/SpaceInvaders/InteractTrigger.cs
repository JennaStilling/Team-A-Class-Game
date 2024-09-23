using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractTrigger : MonoBehaviour
{
    public IGame game; // Use the interface instead of a specific game script

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            game?.SetPlayerInRange(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            game?.SetPlayerInRange(false);
        }
    }
}
