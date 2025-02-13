using UnityEngine;

public class Hazard : MonoBehaviour
{
    public int damageAmount = 25;  // How much damage the hazard deals

    void OnTriggerEnter(Collider other)
    {
        PlayerMovement playerHealth = other.GetComponent<PlayerMovement>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damageAmount);
        }
    }
}
