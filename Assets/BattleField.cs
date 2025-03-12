using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleField : MonoBehaviour
{
    [Header("Battle Settings")]
    public bool isBattleActive = false;
    [SerializeField] private Transform[] enemySpawnPoints;
    [SerializeField] private GameObject[] enemyPrefabs;
    [SerializeField] private BoxCollider2D battleBounds;  // Reference to the bounds child object's collider
    [SerializeField] private BoxCollider2D battleTrigger; // Reference to the trigger child object's collider
    
    [Header("Camera Settings")]
    [SerializeField] private float cameraMinX;
    [SerializeField] private float cameraMaxX;
    
    private CameraController cameraController;
    private EdgeCollider2D[] boundaryColliders;
    private int remainingEnemies;

    // Start is called before the first frame update
    void Start()
    {
        // Get references
        cameraController = Camera.main.GetComponent<CameraController>();
        
        // Make sure the trigger is set as a trigger
        if (battleTrigger != null)
            battleTrigger.isTrigger = true;
            
        // Create boundary colliders but keep them disabled initially
        CreateBoundaryColliders();
        SetBoundariesActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player") && !isBattleActive)
        {
            StartBattle();
        }
    }

    public void StartBattle()
    {
        isBattleActive = true;
        
        // Lock camera
        if (cameraController != null)
        {
            cameraController.SetBoundaries(
                transform.position.x + cameraMinX,
                transform.position.x + cameraMaxX
            );
        }

        // Enable boundaries
        SetBoundariesActive(true);

        // Spawn enemies
        SpawnEnemies();
    }

    private void CreateBoundaryColliders()
    {
        boundaryColliders = new EdgeCollider2D[2]; // Left and right boundaries

        // Create left boundary
        GameObject leftBoundary = new GameObject("LeftBoundary");
        leftBoundary.transform.parent = transform;
        EdgeCollider2D leftCollider = leftBoundary.AddComponent<EdgeCollider2D>();
        boundaryColliders[0] = leftCollider;

        // Create right boundary
        GameObject rightBoundary = new GameObject("RightBoundary");
        rightBoundary.transform.parent = transform;
        EdgeCollider2D rightCollider = rightBoundary.AddComponent<EdgeCollider2D>();
        boundaryColliders[1] = rightCollider;

        // Set up the collider points based on the battle bounds
        UpdateBoundaryColliderPoints();
    }

    private void UpdateBoundaryColliderPoints()
    {
        if (battleBounds == null) return;

        Bounds bounds = battleBounds.bounds;
        float height = bounds.size.y;
        
        // Set points for left boundary
        boundaryColliders[0].points = new Vector2[]
        {
            new Vector2(bounds.min.x, bounds.min.y),
            new Vector2(bounds.min.x, bounds.min.y + height)
        };

        // Set points for right boundary
        boundaryColliders[1].points = new Vector2[]
        {
            new Vector2(bounds.max.x, bounds.min.y),
            new Vector2(bounds.max.x, bounds.min.y + height)
        };
    }

    private void SetBoundariesActive(bool active)
    {
        foreach (var collider in boundaryColliders)
        {
            if (collider != null)
                collider.enabled = active;
        }
    }

    private void SpawnEnemies()
    {
        remainingEnemies = enemySpawnPoints.Length;
        
        for (int i = 0; i < enemySpawnPoints.Length; i++)
        {
            if (enemySpawnPoints[i] != null)
            {
                int randomEnemyIndex = Random.Range(0, enemyPrefabs.Length);
                GameObject enemy = Instantiate(
                    enemyPrefabs[randomEnemyIndex], 
                    enemySpawnPoints[i].position, 
                    Quaternion.identity
                );

                // Set the enemy's battle state to true
                if (enemy.TryGetComponent<EnemyAI>(out var enemyAI))
                {
                    enemyAI.SetBattleState(true);
                }
                
                // Subscribe to enemy death event
                if (enemy.TryGetComponent<EnemyHealth>(out var enemyHealth))
                {
                    enemyHealth.OnEnemyDeath += HandleEnemyDeath;
                }
            }
        }
    }

    private void HandleEnemyDeath()
    {
        remainingEnemies--;
        
        if (remainingEnemies <= 0)
        {
            EndBattle();
        }
    }

    private void EndBattle()
    {
        isBattleActive = false;
        
        // Release camera
        if (cameraController != null)
        {
            cameraController.ResetBoundaries();
        }

        // Disable boundaries
        SetBoundariesActive(false);
    }
}
