using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraController : MonoBehaviour
{
    public GameObject player;
    private Transform playerTransform; // The player's transform
    public float smoothSpeed = 0.125f; // How smoothly the camera catches up with the player's movement
    public Vector3 offset; // The offset from the player position

    public float offsetY = 2f; // The Y position to lock the camera to

    public float minX = -100f; // Minimum X boundary
    public float maxX = 110f;  // Maximum X boundary

    private void Start()
    {
        playerTransform = player.transform;
    }

    void LateUpdate()
    {
        if (playerTransform == null) return;

        // Get the player's X position
        float playerX = playerTransform.position.x;
        
        // Clamp between boundaries
        float clampedX = Mathf.Min(playerX, maxX);  // First, clamp to max
        clampedX = Mathf.Max(minX, clampedX);       // Then, clamp to min
        
        // Calculate desired position with offset
        Vector3 desiredPosition = new Vector3(clampedX + offset.x, offsetY, -10);

        // Smoothly interpolate towards the desired position
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // Update the camera's position
        transform.position = smoothedPosition;
    }

    void Update()
    {
        // Check for 'M' key press to reload the scene
        if (Input.GetKeyDown(KeyCode.M))
        {
            ReloadScene();
        }
    }

    void ReloadScene()
    {
        // Get the current scene index
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        
        // Reload the current scene
        SceneManager.LoadScene(currentSceneIndex);
    }
    
        public void SetBoundaries(float min, float max)
    {
        minX = min;
        maxX = max;
    }

    public void ResetBoundaries()
    {
        // Reset to your default values
        minX = -float.MaxValue;
        maxX = float.MaxValue;
    }
}
