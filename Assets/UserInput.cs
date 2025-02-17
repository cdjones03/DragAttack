using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserInput : MonoBehaviour
{

    public static UserInput instance;
    [HideInInspector] public Controls controls;
    [HideInInspector] public Vector2 moveInput;
    [HideInInspector] public bool jumpInput;
    [HideInInspector] public bool attackInput;



    public void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        controls = new Controls();

        controls.Movement.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();

        controls.Movement.Jump.performed += ctx => jumpInput = true;
        controls.Movement.Jump.canceled += ctx => jumpInput = false;
        
        controls.Attack.Punch.performed += ctx => attackInput = true;
        controls.Attack.Punch.canceled += ctx => attackInput = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }
}
