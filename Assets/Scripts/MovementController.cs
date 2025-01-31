using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    public float moveSpeedForm1 = 5f; // Movement speed of the character
    public float moveSpeedForm2 = 8f; // Movement speed of the character
    public float jumpForce = 10f; // Force applied when the character jumps
    public Transform groundCheck; // A point at the bottom of the character to check if grounded
    public float groundCheckRadius = 0.2f; // Radius of the ground check
    public LayerMask groundMask; // Mask to specify what is considered ground

    private Rigidbody2D rb; // Reference to the Rigidbody2D component
    private float moveHorizontal; // Variable to store the horizontal move input
    private float moveVertical; // Variable to store the vertical move input
    private SpriteRenderer spriteRenderer;

    public AnimationController animationController;
    private bool isGrounded; // To check if the player is on the ground

    [SerializeField]
    private float MIN_Y = -3f;
    [SerializeField]
    private float MAX_Y = 3.75f;

    void Start()
    {
        // Get and store a reference to the Rigidbody2D component so that we can access it.
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animationController = GetComponent<AnimationController>();
    }

    void Update()
    {
        // Get the horizontal input (A, D, Left Arrow, Right Arrow for horizontal)
        moveHorizontal = Input.GetAxis("Horizontal");
        moveVertical = Input.GetAxis("Vertical");

        // Check if the player is on the ground using Physics2D to detect collision with ground layers
        //isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundMask);
        isGrounded = true;
        // Flip the sprite based on movement direction
        if (moveHorizontal > 0)
        {
            spriteRenderer.flipX = false;
        }
        else if (moveHorizontal < 0)
        {
            spriteRenderer.flipX = true;
        }

        // Handle jumping when the player presses the jump button (space bar)
        if (Input.GetKeyDown(KeyCode.J) && isGrounded)
        {
            Jump();
        }
    }

    void FixedUpdate()
    {
        // Create movement vector
        float moveSpeed = animationController.IsForm1() ? moveSpeedForm1 : moveSpeedForm2;
        Vector2 movement = new Vector2(moveHorizontal * moveSpeed, moveVertical * moveSpeed);

        // Apply the movement
        rb.velocity = movement;

        // Clamp both X and Y positions
        Vector3 clampedPosition = transform.position;
        clampedPosition.x = Mathf.Max(-45f, clampedPosition.x);  // Clamp minimum X to -45
        clampedPosition.y = Mathf.Clamp(clampedPosition.y, MIN_Y, MAX_Y);
        transform.position = clampedPosition;
    }

    // Function to handle jumping
    void Jump()
    {
        // Apply an upward force to the Rigidbody2D for jumping
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
    }

    // Debugging tool: Visualize the ground check radius in the Scene view
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}
