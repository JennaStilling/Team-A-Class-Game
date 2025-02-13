using System.Collections.Generic;
using UnityEngine;
using Observations;

public class EnemyManager : MonoBehaviour
{
    private List<IObserver> _observers = new List<IObserver>();
    /*
     * Temporary manager - will merge with PlayerManager
     */
    public bool TakeDamage()
    {
        return false;
    }
    
    
}