using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private GameObject employee;
    //[SerializeField] private SpawnDirections direction; // defunct, won't use
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float spawnTimer = 5f;
    private Transform[] tempWaypoints;
    [SerializeField] private int employeeLimit = 5;
    
    // Start is called before the first frame update
    void Start()
    {
        tempWaypoints= new Transform[GameObject.Find("Patrol Path").transform.childCount];
        GameObject originalGameObject = GameObject.Find("Patrol Path");

        for (int i = 0; i < tempWaypoints.Length; i++)
            tempWaypoints[i] = originalGameObject.transform.GetChild(i).transform;
        
        employee.GetComponent<EmployeeBT>().waypoints = tempWaypoints;
        StartCoroutine(HandleSpawnTimer());
    }

    private void SpawnEnemy()
    {
        Debug.Log("Spawned enemy");
        Instantiate(employee, spawnPoint);
    }

    /*
     * Change this code later!!!
     * There should be a way to know how many employees are in the area at a given time without trying to find using children of SpawnLocation
     * Instead of starting the coroutine after every HandleSpawnTimer() call since that uses up resources, when an employee is defeated, there should be an alert sent out when an employee dies that decrements the counter
     * ^ Where door is the subscriber to the notifier and 
     */
    public IEnumerator HandleSpawnTimer()
    {
        yield return new WaitForSeconds(spawnTimer);
        if (GameObject.Find("SpawnLocation").transform.childCount < employeeLimit)
            SpawnEnemy();
        StartCoroutine(HandleSpawnTimer());
    }
}