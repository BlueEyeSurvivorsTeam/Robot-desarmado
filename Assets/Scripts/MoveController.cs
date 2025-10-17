using UnityEngine;

public class MoveController : MonoBehaviour
{
    [Header("Movimiento")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;
    public float acceleration = 8f;
    public float deceleration = 12f;

    private bool isMovingForward = false;
    private bool isMovingBackward = false;
    private Vector3 inputDirection;
    private float rotationInput;
    private float currentSpeed;
    public float currentAnimSpeed { get; private set; }
    private Rigidbody rb;
    private RotateController rc;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        rc = GetComponent<RotateController>();
        rc.SetValues(rotationSpeed, currentSpeed);
    }

    void Update()
    {
        GetInput();
        rc.SetValues(rotationSpeed, currentSpeed);
    }

    void FixedUpdate()
    {
        MovePlayer();
        RotatePlayer();
    }

    void GetInput()
    {
        float rotateInput = Input.GetAxis("Horizontal");
        float moveInput = Input.GetAxis("Vertical");
        isMovingForward = moveInput > 0.1f;
        isMovingBackward = moveInput < -0.1f;
        float targetAnimSpeed = 0f;
        if (isMovingForward)
        {
            targetAnimSpeed = moveInput * moveSpeed;
            inputDirection = transform.forward;
            currentAnimSpeed = Mathf.MoveTowards(currentAnimSpeed, targetAnimSpeed, acceleration * Time.deltaTime);
        }
        else if (isMovingBackward)
        {
            targetAnimSpeed = moveInput * moveSpeed;
            inputDirection = transform.forward * -1f;
            currentAnimSpeed = Mathf.MoveTowards(currentAnimSpeed, targetAnimSpeed, acceleration * Time.deltaTime);
        }
        else
        {
            inputDirection = Vector3.zero;
            currentAnimSpeed = Mathf.MoveTowards(currentAnimSpeed, targetAnimSpeed, deceleration * Time.deltaTime);
        }

        float targetMovementSpeed = Mathf.Abs(moveInput) > 0.1f ? Mathf.Abs(moveSpeed) : 0f;
        currentSpeed = Mathf.MoveTowards(currentSpeed, targetMovementSpeed, acceleration * Time.deltaTime);

        rotationInput = rotateInput;
    }

    void MovePlayer()
    {
        Vector3 moveVelocity = inputDirection * currentSpeed;
        rb.linearVelocity = new Vector3(moveVelocity.x, rb.linearVelocity.y, moveVelocity.z);
    }

    void RotatePlayer()
    {
        if (Mathf.Abs(rotationInput) > 0.1f && currentSpeed != 0)
        {
            float rotationAmount = rotationInput * rotationSpeed * Time.deltaTime;
            transform.Rotate(0f, rotationAmount, 0f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {

    }

    private void OnTriggerExit(Collider other)
    {

    }
}