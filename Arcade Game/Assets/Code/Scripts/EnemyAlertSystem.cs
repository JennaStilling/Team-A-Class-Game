using UnityEngine;

namespace Observations
{
    public class EnemyAlertSystem : MonoBehaviour, IObserver
    {
        // where subject is the name of the abstract class!!
        [SerializeField] private Subject _playerSubject;
        public void OnNotify(EnemyAlerts alert)
        {
            if (alert == EnemyAlerts.Help)
            {
                //call other guys
            }
        }

        private void OnEnable()
        {
            _playerSubject.AddObserver(this);
        }
    }
}