using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteMask))]  // This will automatically add SpriteMask if missing
public class CameraMask : MonoBehaviour
{
    private Camera mainCamera;
    private SpriteMask spriteMask;
    [SerializeField] private Sprite maskSprite; // Reference a white sprite for the mask

    void Start()
    {
        mainCamera = Camera.main;
        spriteMask = GetComponent<SpriteMask>();
        
        // Set up the sprite mask
        if (maskSprite != null)
            spriteMask.sprite = maskSprite;
            
        UpdateMaskSize();
    }

    void UpdateMaskSize()
    {
        // Calculate the visible area in world units
        float height = 2f * mainCamera.orthographicSize;
        float width = height * mainCamera.aspect;
        
        // Set the mask size to match the camera view
        transform.localScale = new Vector3(width, height, 1);
    }

    void LateUpdate()
    {
        // Keep the mask at camera position
        transform.position = new Vector3(
            mainCamera.transform.position.x,
            mainCamera.transform.position.y,
            transform.position.z
        );
    }
}
