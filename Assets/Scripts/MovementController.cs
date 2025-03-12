using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    [SerializeField] private bool canMove = true;
    [Tooltip(("If your character does not jump, ignore all below 'Jumping' Character"))]
    [SerializeField] private bool doesCharacterJump = false;

    [Header("Base / Root")]
    [SerializeField] private Rigidbody2D baseRB;
    [SerializeField] private float hSpeed = 10f;
    [SerializeField] private float vSpeed = 6f;
    [Range(0, 1.0f)]
    [SerializeField] float movementSmooth = 0.5f;

    [Header("'Jumping' Character")]
    [SerializeField] private Rigidbody2D charRB;
    [SerializeField] private float jumpVal = 10f;
    [SerializeField] private int possibleJumps = 1;
    [SerializeField] private int currentJumps = 0;
    [SerializeField] private bool onBase = false;
    [SerializeField] private Transform jumpDetector;
    [SerializeField] private float detectionDistance;
    [SerializeField] private LayerMask detectLayer;
    [SerializeField] private float jumpingGravityScale;
    [SerializeField] private float fallingGravityScale;
    private bool jump;

    private bool facingRight = true;
    private Vector3 velocity = Vector3.zero;

    private Vector2 moveInput;

    private float gownMoveSpeed = 5f;
    private float suitMoveSpeed = 8f;

    private float moveSpeed;

    private bool isGownForm = true;

    private Animator animator;
    
    // 
    private Vector3 charDefaultRelPos, baseDefPos; 

    // Start is called before the first frame update
    private void Awake()
    {

    }

    private void Start()
    {
        charDefaultRelPos = charRB.transform.localPosition;
        animator = GetComponentInChildren<Animator>();
    }
    
    private void Update()
    {
        moveInput = UserInput.instance.moveInput;
        jump = UserInput.instance.jumpInput;
        /*
        if (controls.JumpState && currentJumps < possibleJumps)
        {
            jump = true;
        }
        */
    }

    private void FixedUpdate()
    {
        Move();
    }
    
    private void Move()
    {
        moveInput = UserInput.instance.moveInput;
        moveSpeed = isGownForm ? gownMoveSpeed : suitMoveSpeed;

        if(moveInput.x != 0 || moveInput.y != 0){
            animator.SetBool("isWalking", true);
        } else {
            animator.SetBool("isWalking", false);
        }

        if (!onBase && doesCharacterJump && charRB.velocity.y < 0)
        {
            detectBase();
        }

        if (canMove)
        {
            Vector3 targetVelocity = new Vector2(moveInput.x * moveSpeed, moveInput.y * moveSpeed);

            Vector2 _velocity = Vector3.SmoothDamp(baseRB.velocity, targetVelocity, ref velocity, movementSmooth);
            baseRB.velocity = _velocity;



            
            //----- 
            if (doesCharacterJump)
            {
                
                if (onBase)
                {
                    
                    // charRB.velocity = baseRB.velocity;
                    charRB.velocity = Vector2.zero;
                    
                    // vertical check
                    if (charRB.transform.localPosition != charDefaultRelPos)
                    {
                        var charTransform = charRB.transform;
                        charTransform.localPosition = new Vector2(charTransform.localPosition.x,
                            charDefaultRelPos.y);
                    }
                }


                else
                {
                    // falling
                    // if (charRB.velocity.y < 0)
                    // {
                    //     // charRB.gravityScale = fallingGravityScale;
                    // }
                    // else
                    // { // moving upward from jump
                    //     // charRB.gravityScale = jumpingGravityScale;
                    // }
                
                    charRB.velocity = new Vector2(_velocity.x, charRB.velocity.y);
                }

                if (jump)
                {
                    charRB.isKinematic = false;
                    charRB.AddForce(Vector2.up * jumpVal, ForceMode2D.Impulse);
                    charRB.gravityScale = jumpingGravityScale;
                    jump = false;
                    currentJumps++;
                    onBase = false;
                }
                
                
                // --- horizontal position check
                if (charRB.transform.localPosition != charDefaultRelPos)
                {
                    //print("pos diff- local: " + charRB.transform.localPosition + "  --default: " + charDefaultRelPos );
                    var charTransform = charRB.transform;
                    charTransform.localPosition = new Vector2(charDefaultRelPos.x,
                        charTransform.localPosition.y);
                }
                
            }
            // --- 

            

            // rotate if we're facing the wrong way
            if (moveInput.x > 0 && !facingRight)
            {
                flip();
            } else if(moveInput.x < 0 && facingRight)
            {
                flip();
            }
        }
    }

    private void flip()
    {
        facingRight = !facingRight;
        transform.Rotate(0, 180, 0);
    }

    private void detectBase()
    {
        RaycastHit2D hit = Physics2D.Raycast(jumpDetector.position, -Vector2.up, detectionDistance, detectLayer);
        if(hit.collider != null)
        {
            onBase = true;
            charRB.isKinematic = true;
            currentJumps = 0;
            // charRB.velocity = Vector2.zero;
            // baseRB.velocity = Vector2.zero;
            Debug.Log("setting velocity to zero");
        }
    }

    private void OnDrawGizmos()
    {
        if (doesCharacterJump)
        {
            Gizmos.DrawRay(jumpDetector.transform.position, -Vector3.up * detectionDistance);
        }
    }   
}
