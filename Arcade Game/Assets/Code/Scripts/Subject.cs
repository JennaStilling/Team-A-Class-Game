using System.Collections.Generic;
using UnityEngine;

namespace Observations
{
    public abstract class Subject : MonoBehaviour
    {
        private List<IObserver> _observers = new List<IObserver>();

        public void AddObserver(IObserver observer)
        {
            _observers.Add(observer);
        }

        public void RemoveObserver(IObserver observer)
        {
            _observers.Remove(observer);
        }

        protected void NotifyObservers(EnemyAlerts alert)
        {
            _observers.ForEach((_observer) =>
            {
                _observer.OnNotify(alert);
            });
        }
    }
}