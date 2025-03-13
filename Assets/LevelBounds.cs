using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelBounds : MonoBehaviour
{
    [SerializeField] private BoxCollider2D boundingBox;
    [SerializeField] private bool showDebugInfo = true;
    private EdgeCollider2D[] boundaryColliders;

    // Start is called before the first frame update
    void Start()
    {
        // Get or add the box collider
        if (boundingBox == null)
            boundingBox = GetComponent<BoxCollider2D>();

        CreateBoundaryColliders();
    }

    private void CreateBoundaryColliders()
    {
        boundaryColliders = new EdgeCollider2D[4]; // Top, Bottom, Left, Right boundaries

        string[] boundaryNames = { "TopBoundary", "BottomBoundary", "LeftBoundary", "RightBoundary" };
        for (int i = 0; i < 4; i++)
        {
            GameObject boundary = new GameObject(boundaryNames[i]);
            boundary.transform.parent = transform;
            boundary.transform.localPosition = Vector3.zero;
            boundary.layer = LayerMask.NameToLayer("Ground");
            
            EdgeCollider2D edgeCollider = boundary.AddComponent<EdgeCollider2D>();
            boundaryColliders[i] = edgeCollider;
        }

        UpdateBoundaryColliderPoints();
    }

    private void UpdateBoundaryColliderPoints()
    {
        if (boundingBox == null) return;

        // Get the local bounds and convert to world space points
        Bounds bounds = boundingBox.bounds;
        Vector2 size = boundingBox.size;
        Vector2 offset = boundingBox.offset;
        
        // Calculate corners in local space
        Vector2 bottomLeft = offset - size * 0.5f;
        Vector2 topRight = offset + size * 0.5f;

        // Top boundary
        boundaryColliders[0].points = new Vector2[]
        {
            new Vector2(bottomLeft.x, topRight.y),
            new Vector2(topRight.x, topRight.y)
        };

        // Bottom boundary
        boundaryColliders[1].points = new Vector2[]
        {
            new Vector2(bottomLeft.x, bottomLeft.y),
            new Vector2(topRight.x, bottomLeft.y)
        };

        // Left boundary
        boundaryColliders[2].points = new Vector2[]
        {
            new Vector2(bottomLeft.x, bottomLeft.y),
            new Vector2(bottomLeft.x, topRight.y)
        };

        // Right boundary
        boundaryColliders[3].points = new Vector2[]
        {
            new Vector2(topRight.x, bottomLeft.y),
            new Vector2(topRight.x, topRight.y)
        };

        if (showDebugInfo)
        {
            Debug.Log($"Created level boundaries at: MinX: {bottomLeft.x}, MaxX: {topRight.x}, MinY: {bottomLeft.y}, MaxY: {topRight.y}");
        }
    }

    // Optional: Add a method to visualize the bounds in the editor
    private void OnDrawGizmos()
    {
        if (boundingBox == null) return;

        Bounds bounds = boundingBox.bounds;
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(bounds.center, bounds.size);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
