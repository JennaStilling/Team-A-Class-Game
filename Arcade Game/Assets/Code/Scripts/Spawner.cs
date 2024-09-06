using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private GameObject _employee;
    //[SerializeField] private SpawnDirections direction; // defunct, won't use
    [SerializeField] private Transform _spawnPoint;
    //[SerializeField] private float spawnTimer = 5f;
    [SerializeField] private float _minSpawnTime = 10f;
    [SerializeField] private float _maxSpawnTime = 15f;
    private Transform[] _tempWaypoints;
    [SerializeField] private int _employeeLimit = 5;
    
    // Start is called before the first frame update
    void Start()
    {
        _tempWaypoints= new Transform[GameObject.Find("Patrol Path").transform.childCount];
        GameObject originalGameObject = GameObject.Find("Patrol Path");

        for (int i = 0; i < _tempWaypoints.Length; i++)
            _tempWaypoints[i] = originalGameObject.transform.GetChild(i).transform;
        
        _employee.GetComponent<EmployeeBT>().waypoints = _tempWaypoints;
        StartCoroutine(HandleSpawnTimer());
    }

    private void SpawnEnemy()
    {
        Debug.Log("Spawned enemy");
        Instantiate(_employee, _spawnPoint);
    }

    /*
     * Change this code later!!!
     * There should be a way to know how many employees are in the area at a given time without trying to find using children of SpawnLocation
     * Instead of starting the coroutine after every HandleSpawnTimer() call since that uses up resources, when an employee is defeated, there should be an alert sent out when an employee dies that decrements the counter
     * ^ Where door is the subscriber to the notifier and 
     */
    public IEnumerator HandleSpawnTimer()
    {
        //yield return new WaitForSeconds(spawnTimer);
        yield return new WaitForSeconds(Random.Range(_minSpawnTime, _maxSpawnTime));
        if (GameObject.Find("SpawnLocation").transform.childCount < _employeeLimit)
            SpawnEnemy();
        StartCoroutine(HandleSpawnTimer());
    }
}