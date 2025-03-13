using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public GameObject enemyPrefab;
    public float spawnInterval = 8f;
    public float minX = -20f;
    public float maxX = 30f;
    public float spawnY = -4f;  // Adjust this based on your terrain
    public float spawnZ = 0f;  // Adjust this based on your desired spawn depth

    private void Start()
    {
        //StartCoroutine(SpawnEnemyRoutine());
        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    private IEnumerator SpawnEnemyRoutine()
    {
        while (true)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnEnemy()
    {
        float randomX = Random.Range(minX, maxX);
        Vector3 spawnPosition = new Vector3(randomX, spawnY, spawnZ);
        
        Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
    }


}

