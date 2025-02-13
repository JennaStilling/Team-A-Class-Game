using System.Collections;
using System.Collections.Generic;
using Observations;
using Unity.VisualScripting;
using UnityEngine;

public class Spawner : PlayerSubject
{
    [SerializeField] private GameObject _employee;
    //[SerializeField] private SpawnDirections direction; // defunct, won't use
    [SerializeField] private Transform _spawnPoint;
    //[SerializeField] private float spawnTimer = 5f;
    [SerializeField] private float _minSpawnTime = 10f;
    [SerializeField] private float _maxSpawnTime = 15f;
    private Transform[] _tempWaypoints;
    [SerializeField] private int _employeeLimit = 5;
    private GameObject _observer;
    
    // Start is called before the first frame update
    void Start()
    {
        _employee.GetComponent<EmployeeEnemyManager>().maxTokensProp = 1;

        _tempWaypoints = new Transform[GameObject.Find("Patrol Path").transform.childCount];
        GameObject originalGameObject = GameObject.Find("Patrol Path");

        for (int i = 0; i < _tempWaypoints.Length; i++)
            _tempWaypoints[i] = originalGameObject.transform.GetChild(i).transform;
        
        _employee.GetComponent<EmployeeBT>().waypoints = _tempWaypoints;
        AddObserver(FindObjectOfType<MeleeWeapon>());
        
        StartCoroutine(HandleSpawnTimer());
    }

    private void SpawnEnemy()
    {
        Debug.Log("Spawned enemy");
        GameObject temp = Instantiate(_employee, _spawnPoint);
        NotifyObservers(temp.GetComponent<EmployeeEnemyManager>());
    }
    
    public IEnumerator HandleSpawnTimer()
    {
        //yield return new WaitForSeconds(spawnTimer);
        yield return new WaitForSeconds(Random.Range(_minSpawnTime, _maxSpawnTime));
        if (GameObject.Find("SpawnLocation").transform.childCount < _employeeLimit)
            SpawnEnemy();
        StartCoroutine(HandleSpawnTimer());
    }

    public void setMaxCoins(int max)
    {
        _employee.GetComponent<EmployeeEnemyManager>().maxTokensProp = max;
        foreach (EmployeeEnemyManager enemy in FindObjectsOfType<EmployeeEnemyManager>())
        {
            enemy.maxTokensProp = max;
        }
    }
    public void incrementMaxCoins() 
    {
        _employee.GetComponent<EmployeeEnemyManager>().maxTokensProp++;
        foreach (EmployeeEnemyManager enemy in FindObjectsOfType<EmployeeEnemyManager>())
        {
            enemy.maxTokensProp++;
        }

    }
}