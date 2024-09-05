using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private GameObject employee;
    //[SerializeField] private SpawnDirections direction;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float spawnTimer = 5f;
    
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(HandleSpawnTimer());
    }

    private void SpawnEnemy()
    {
        Debug.Log("Spawned enemy");
        Instantiate(employee, spawnPoint);
        StartCoroutine(HandleSpawnTimer());
    }

    public IEnumerator HandleSpawnTimer()
    {
        yield return new WaitForSeconds(spawnTimer);
        SpawnEnemy();
    }
}
