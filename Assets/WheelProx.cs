using UnityEngine;

public class WheelProx : MonoBehaviour
{
    public WheelGame wheelGame;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            wheelGame.SetPlayerInRange(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            wheelGame.SetPlayerInRange(false);
        }
    }
}
