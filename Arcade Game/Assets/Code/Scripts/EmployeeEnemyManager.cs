using UnityEngine;

public class EmployeeEnemyManager : MonoBehaviour
{
    [SerializeField] private int _healthpoints = 30;
    private int _tokensUponDeath;
    public bool isManager; // keep to have some employees be worth more, like managers? tbd
    private bool _isDead = false;
    private bool _callForHelp = false;
    [SerializeField] public bool isUnderAttack = false;

    private void Awake()
    {
        _tokensUponDeath = Random.Range(1, 3);

        if (Random.Range(1, 10) >= 8)
            isManager = true;
    }

    public bool TakeDamage()
    {
        _healthpoints -= 10;
        if (!_callForHelp)
        {
            _callForHelp = true;
            //NotifyObservers(EnemyAlerts.Help);
        }
        
        _isDead = _healthpoints <= 0;
        if (_isDead) Die();
        return _isDead;
    }

    private void Die()
    {
        // drop tokens
        if (isManager)
            _tokensUponDeath *= 2;
        for (int i = 0; i < _tokensUponDeath; i++)
            Debug.Log("Spawning coin #"+i); // spawn coins at location of death
            //Instantiate(token, self.transform.position, self.transform.rotation);
        Destroy(gameObject);
    }
}