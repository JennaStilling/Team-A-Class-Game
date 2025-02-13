using System.Collections.Generic;
using UnityEngine;

namespace Observations
{
    public abstract class PlayerSubject : MonoBehaviour
    {
        private List<MeleeWeapon> _observers = new List<MeleeWeapon>();

        public void AddObserver(MeleeWeapon observer)
        {
            _observers.Add(observer);
        }

        public void RemoveObserver(MeleeWeapon observer)
        {
            _observers.Remove(observer);
        }

        protected void NotifyObservers(EmployeeEnemyManager enemy)
        {
            _observers.ForEach((_observer) =>
            {
                _observer.OnNotify(enemy);
            });
        }
    }
}