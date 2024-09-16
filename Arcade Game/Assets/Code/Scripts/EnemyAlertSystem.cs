using UnityEngine;

namespace Observations
{
    public class EnemyAlertSystem : Subject
    {
        // where subject is the name of the abstract class!!
        [SerializeField] private Subject _playerSubject;
        private Transform[] _enemiesInScene;

        public void CallForHelp(EnemyAlerts alert)
        {
            if (alert == EnemyAlerts.Help)
            {
                _enemiesInScene= new Transform[GameObject.Find("SpawnLocation").transform.childCount];
                GameObject originalGameObject = GameObject.Find("SpawnLocation");

                for (int i = 0; i < _enemiesInScene.Length; i++)
                {
                    _enemiesInScene[i] = originalGameObject.transform.GetChild(i).transform;
                    _enemiesInScene[i].GetComponent<EmployeeEnemyManager>().isUnderAttack = true;
                }
            }

            if (alert == EnemyAlerts.Safe)
            {
                for (int i = 0; i < _enemiesInScene.Length; i++)
                    _enemiesInScene[i].GetComponent<EmployeeEnemyManager>().isUnderAttack = false;
            }
        }

        protected void NotifyObservers(EmployeeEnemyManager alert)
        {
            
        }
    }
}


