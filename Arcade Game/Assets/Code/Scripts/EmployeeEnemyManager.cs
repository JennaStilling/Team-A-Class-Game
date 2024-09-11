using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EmployeeEnemyManager : MonoBehaviour
{
    [SerializeField] private int _healthpoints = 30;
    [SerializeField] private GameObject _token; // change comment pt 1 - eventually remove and replace below with asset path
    [SerializeField] private int maxTokens = 3;
    private int _tokensUponDeath;
    public bool isManager; // keep to have some employees be worth more, like managers? tbd
    private bool _isDead = false;
    private bool _callForHelp = false;
    [SerializeField] public bool isUnderAttack = false;
    private NavMeshAgent _agent;

    private void Awake()
    {
        _tokensUponDeath = Random.Range(1, maxTokens);

        if (Random.Range(1, 10) >= 8)
            isManager = true;
        _agent = GetComponent<NavMeshAgent>();
        if (_agent == null)
            Debug.Log("Nav mesh agent not found");
        _agent.updateRotation = false;
        _agent.stoppingDistance = 0f;
    }

    private void Update()
    {
        _isDead = _healthpoints <= 0;
        if (_isDead) Die();
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
        {
            Debug.Log("Spawning coin #" + i); // spawn coins at location of death
            Instantiate(_token, transform.position,
                transform.rotation); // change comment pt 2 - will eventually find path to prefab
        }

        Destroy(gameObject);
    }
}