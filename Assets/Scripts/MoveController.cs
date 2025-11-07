using UnityEngine;

public class MoveController : MonoBehaviour
{
    [Header("Movimiento")]
    public float moveSpeed = 5f;
    public float runSpeed = 5f;
    public float rotationSpeed = 10f;
    public float acceleration = 8f;
    public float deceleration = 12f;
    public KeyCode runKey = KeyCode.LeftShift;

    [Header("Acciones")]
    public bool canMove;
    public bool canRun;
    private bool isMovingForward = false;
    private bool isMovingBackward = false;
    private Vector3 inputDirection;
    private float rotationInput;
    private float currentSpeed;
    private float currentMovementSpeed;
    public float currentAnimSpeed { get; private set; }
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    void Update()
    {
        GetInput();
    }

    void FixedUpdate()
    {
        if(canMove)
        {
            MovePlayer();
            RotatePlayer();
        }
    }

    void GetInput()
    {
        if(canRun)
        {
            if(Input.GetKey(runKey))
            {
                currentSpeed = runSpeed;
            }
            else
            {
                currentSpeed = moveSpeed;
            }
        }
        float rotateInput = Input.GetAxis("Horizontal");
        float moveInput = Input.GetAxis("Vertical");
        isMovingForward = moveInput > 0.1f;
        isMovingBackward = moveInput < -0.1f;
        float targetAnimSpeed = 0f;
        if (isMovingForward)
        {
            targetAnimSpeed = moveInput * currentSpeed;
            inputDirection = transform.forward;
            currentAnimSpeed = Mathf.MoveTowards(currentAnimSpeed, targetAnimSpeed, acceleration * Time.deltaTime);
        }
        else if (isMovingBackward)
        {
            targetAnimSpeed = moveInput * currentSpeed;
            inputDirection = transform.forward * -1f;
            currentAnimSpeed = Mathf.MoveTowards(currentAnimSpeed, targetAnimSpeed, acceleration * Time.deltaTime);
        }
        else
        {
            inputDirection = Vector3.zero;
            currentAnimSpeed = Mathf.MoveTowards(currentAnimSpeed, targetAnimSpeed, deceleration * Time.deltaTime);
        }

        float targetMovementSpeed = Mathf.Abs(moveInput) > 0.1f ? Mathf.Abs(moveSpeed) : 0f;
        currentMovementSpeed = Mathf.MoveTowards(currentSpeed, targetMovementSpeed, acceleration * Time.deltaTime);

        rotationInput = rotateInput;
    }

    void MovePlayer()
    {
        Vector3 moveVelocity = inputDirection * currentMovementSpeed;
        rb.linearVelocity = new Vector3(moveVelocity.x, rb.linearVelocity.y, moveVelocity.z);
    }

    void RotatePlayer()
    {
        if (Mathf.Abs(rotationInput) > 0.1f && currentAnimSpeed != 0)
        {
            float rotationAmount = rotationInput * rotationSpeed * Time.deltaTime;
            transform.Rotate(0f, rotationAmount, 0f);
        }
    }
}