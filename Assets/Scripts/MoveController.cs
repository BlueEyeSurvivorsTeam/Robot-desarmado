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

    public RotateController rotateController;
    private bool isMovingForward = false;
    private bool isMovingBackward = false;
    private Vector3 inputDirection;
    private float currentSpeed;
    private float currentMovementSpeed;
    private bool initialMove;
    public float currentAnimSpeed { get; private set; }
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.isPaused)
        {
            rb.linearVelocity = Vector3.zero;
            return;
        }
        GetInput();
    }

    void FixedUpdate()
    {
        if(canMove)
        {
            MovePlayer();
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
    }

    void MovePlayer()
    {
        Vector3 moveVelocity = inputDirection * currentMovementSpeed;
        if(moveVelocity != Vector3.zero)
        {
            if(initialMove == false)
            {
                initialMove = true;
                Vector3 current = rotateController.playerPivot.transform.localEulerAngles;
                rotateController.ResetRotation();
                transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y + current.z, transform.eulerAngles.z);
            }
        }
        else
        {
            initialMove = false;
        }
        rb.linearVelocity = new Vector3(moveVelocity.x, rb.linearVelocity.y, moveVelocity.z);
    }
}