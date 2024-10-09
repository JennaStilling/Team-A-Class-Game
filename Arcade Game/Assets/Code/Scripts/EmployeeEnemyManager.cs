using Observations;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;
using Observations;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR;

public class EmployeeEnemyManager : MonoBehaviour, IObserver
{
    [SerializeField] private float _healthpoints = 30f;
    [SerializeField] private GameObject _token; // change comment pt 1 - eventually remove and replace below with asset path
    [SerializeField] private int _maxTokens = 3;
    [SerializeField] private AudioSource _hitSound;
    [SerializeField] private AudioSource _tokenDrop;

     public int maxTokensProp {
         get { return _maxTokens; }
         set { _maxTokens = value; }
     }

     [SerializeField] private int _damage = 5;
     public int damage {
         get { return _damage; }
         set { _damage = value; }
     }
     
    private Transform[] _enemiesInScene;
    private int _tokensUponDeath;
    public bool isManager; // keep to have some employees be worth more, like managers? tbd
    private bool _isDead = false;
    private bool _callForHelp = false;
    [SerializeField] public bool isUnderAttack;
    private NavMeshAgent _agent;
    private float _damageTimer = 5f;
    private bool _canAttack = true;

    private void Awake()
    {
        if (Random.Range(1, 10) >= 8)
            isManager = true;
        _agent = GetComponent<NavMeshAgent>();
        if (_agent == null)
            Debug.Log("Nav mesh agent not found");
        _agent.updateRotation = false;
        _agent.stoppingDistance = 1.5f;
        _hitSound = GetComponent<AudioSource>();
        
    }

    private void Update()
    {
        if (Vector3.Distance(transform.position, GameObject.Find("Player").transform.position) <= _agent.stoppingDistance && isUnderAttack && _canAttack)
        {
            DealDamage();
        }
    }

    public bool TakeDamage(float amt)
    {
        _healthpoints -= amt;
        isUnderAttack = true;
        PlayAttackSound();

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
        if (_isDead)
        {
            _agent.speed = 0;
            // Play death sound only if the employee has just died
            if (_hitSound != null && !_hitSound.isPlaying)
            {
                _hitSound.Play(); // Play death sound
            }

            // Ensure tokens are dropped only once
            if (_tokensUponDeath == 0) // Only drop tokens if this is the first time dying
            {
                _tokensUponDeath = Random.Range(1, _maxTokens);
                if (isManager)
                {
                    _tokensUponDeath *= 2;
                }

                for (int i = 0; i < _tokensUponDeath; i++)
                {
                    Instantiate(_token, transform.position, transform.rotation);
                }
            }

            Destroy(gameObject, _hitSound.clip.length); // Delay destruction until sound finishes
            _tokenDrop.Play();
            Die();
        }

        return _isDead;
    }

    public void DealDamage()
    {
        //Debug.Log("Dealing damage");
        _canAttack = false;
        GameObject.Find("Player").GetComponent<PlayerMovement>().TakeDamage(_damage);
        StartCoroutine(HandleDamageTimer());
    }
    
    public IEnumerator HandleDamageTimer()
    {
        //yield return new WaitForSeconds(spawnTimer);
        yield return new WaitForSeconds(_damageTimer);
        _canAttack = true;
 
    }

    private void Die()
    {
        if (_tokenDrop != null)
        {
            _tokenDrop.Play();
        }

        _tokensUponDeath = Random.Range(1, _maxTokens);
        if (isManager)
            _tokensUponDeath *= 2;
        for (int i = 0; i < _tokensUponDeath; i++)
        {
            Instantiate(_token, transform.position,
                transform.rotation); // change comment pt 2 - will eventually find path to prefab
        }        
    }

    public void Kill()
    {
        if (_hitSound != null && !_hitSound.isPlaying)
        {
            _hitSound.Play(); // Play death sound
        }
        Destroy(gameObject, _hitSound.clip.length); // Delay destruction until sound finishes
        _agent.speed = 0;
        Die();
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

    private void PlayAttackSound()
    {
        if (_hitSound != null)
        {
            _hitSound.Play(); // Play the assigned audio clip
        }
    }

}