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

    private float moveHorizontal; // Variable to store the horizontal move input
    private float moveVertical; // Variable to store the vertical move input
    private SpriteRenderer spriteRenderer;

    public Animator anim;
    private bool isGrounded; // To check if the player is on the ground

    [SerializeField]
    private float MIN_Y = -3f;
    [SerializeField]
    private float MAX_Y = 3.75f;

    private Rigidbody2D rb;
    private Vector2 moveInput;

    [HideInInspector] public bool isFacingRight;

    public bool isGownForm;

    void Start()
    {
        // Get and store a reference to the Rigidbody2D component so that we can access it.
        rb = GetComponent<Rigidbody2D>();
        isFacingRight = true;
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        isGownForm = true;
        anim.SetBool("isGownForm", isGownForm);
    }

    void Update()
    {
        isGrounded = true;

        Move();
    }

    void FixedUpdate()
    {
        // Clamp both X and Y positions
        Vector3 clampedPosition = transform.position;
        clampedPosition.x = Mathf.Max(-45f, clampedPosition.x);  // Clamp minimum X to -45
        clampedPosition.y = Mathf.Clamp(clampedPosition.y, MIN_Y, MAX_Y);
        transform.position = clampedPosition;
    }

    // Debugging tool: Visualize the ground check radius in the Scene view
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }

    private void Move()
    {
        moveInput = UserInput.instance.moveInput;
        if(moveInput.x < 0 || moveInput.x > 0 || moveInput.y < 0 || moveInput.y > 0)
        {
            TurnCheck();
            anim.SetBool("isWalking", true);
        }
        else
        {
            anim.SetBool("isWalking", false);
        }

        float moveSpeed = isGownForm ? moveSpeedForm1 : moveSpeedForm2;
        rb.velocity = new Vector2(moveInput.x * moveSpeed, moveInput.y * moveSpeed);
    }

    private void StartDirectionCheck()
    {
        if(moveInput.x > 0)
        {
            isFacingRight = true;
        }
        else if(moveInput.x < 0)
        {
            isFacingRight = false;
        }
    }

    private void TurnCheck()
    {
        if(UserInput.instance.moveInput.x > 0 && !isFacingRight)
        {
            Turn();
        }
        else if(UserInput.instance.moveInput.x < 0 && isFacingRight)
        {
            Turn();
        }
    }

    private void Turn()
    {
        transform.Rotate(0f, 180f, 0f);
        isFacingRight = !isFacingRight;
    }
}
