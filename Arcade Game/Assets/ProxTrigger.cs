using UnityEngine;

public class MachineTrigger : MonoBehaviour
{
    public StackerGame stackerGame; 

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            stackerGame.SetPlayerInRange(true); 
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            stackerGame.SetPlayerInRange(false); 
        }
    }
}
