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

    public float offsetY = 0f; // The Y position to lock the camera to

    public float minX = -33f; // Minimum X boundary
    public float maxX = 110f;  // Maximum X boundary

    private void Start()
    {
        playerTransform = player.transform;
    }

    void LateUpdate()
    {
        // Get the player's X position
        float playerX = playerTransform.position.x;
        
        // Clamp between -22 and player's position
        float clampedX = Mathf.Min(playerX, maxX);  // First, clamp to max
        clampedX = Mathf.Max(-16f, clampedX);       // Then, clamp to -22
        
        Vector3 desiredPosition = new Vector3(clampedX, offsetY, transform.position.z);

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

    void FixedUpdate()
    {
        /**
        playerTransform = player.transform;

        if (playerTransform != null)
        {
            // Get the player's position with offset
            Vector3 targetPosition = new Vector3(playerTransform.position.x + offset.x, transform.position.y, -10);

            // Smoothly move the camera towards the player's position on the X axis
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, targetPosition, smoothSpeed);
            transform.position = smoothedPosition;
        }
        **/
    }

    
}
