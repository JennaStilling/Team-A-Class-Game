using Observations;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EmployeeEnemyManager : MonoBehaviour, IObserver
{
    [SerializeField] private float _healthpoints = 30f;
    [SerializeField] private GameObject _token; // change comment pt 1 - eventually remove and replace below with asset path
    [SerializeField] private int _maxTokens = 3;
     public int maxTokensProp {
         get { return _maxTokens; }
         set { _maxTokens = value; }
     }
     
    private Transform[] _enemiesInScene;
    private int _tokensUponDeath;
    public bool isManager; // keep to have some employees be worth more, like managers? tbd
    private bool _isDead = false;
    private bool _callForHelp = false;
    [SerializeField] public bool isUnderAttack;
    private NavMeshAgent _agent;

    private void Awake()
    {
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

    public bool TakeDamage(float amt)
    {
        _healthpoints -= amt;
        isUnderAttack = true;
        Debug.Log(isUnderAttack);
        if (!_callForHelp)
        {
            _callForHelp = true;
            //NotifyObservers(EnemyAlerts.Help);
             _enemiesInScene= new Transform[GameObject.Find("SpawnLocation").transform.childCount];
             GameObject originalGameObject = GameObject.Find("SpawnLocation");
            
             for (int i = 0; i < _enemiesInScene.Length; i++)
             {
                 _enemiesInScene[i] = originalGameObject.transform.GetChild(i).transform;
                 _enemiesInScene[i].GetComponent<EmployeeEnemyManager>().isUnderAttack = true;
                 _enemiesInScene[i].GetComponent<EmployeeEnemyManager>()._callForHelp = true;
                 _enemiesInScene[i].GetComponent<NavMeshAgent>().stoppingDistance = 2f;
             }
        }
        
        _isDead = _healthpoints <= 0;
        if (_isDead) Die();
        return _isDead;
    }

    private void Die()
    {
        _tokensUponDeath = Random.Range(1, _maxTokens);
        if (isManager)
            _tokensUponDeath *= 2;
        for (int i = 0; i < _tokensUponDeath; i++)
        {
            Instantiate(_token, transform.position,
                transform.rotation); // change comment pt 2 - will eventually find path to prefab
        }

        Destroy(gameObject);
    }

    public void OnNotify(EnemyAlerts alert)
    {
        if (alert == EnemyAlerts.Help)
        {
            isUnderAttack = true;
        }
        else
        {
            isUnderAttack = false;
        }
    }
}