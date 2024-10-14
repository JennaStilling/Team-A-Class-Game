using UnityEngine;

public class DDRProx : MonoBehaviour
{
    public DDRGame ddrGame;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ddrGame.SetPlayerInRange(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ddrGame.SetPlayerInRange(false);
        }
    }
}
