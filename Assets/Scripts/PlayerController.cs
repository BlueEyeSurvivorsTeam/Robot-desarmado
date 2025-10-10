using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movimiento")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;
    private float smoothTime = 0.1f;
    private Rigidbody rb;
    public Vector3 inputDirection { get; private set; }
    public float currentSpeed { get; private set; }
    private float velocityRef;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    void Update()
    {
        GetInput();
        RotatePlayer();
    }

    void FixedUpdate()
    {
        MovePlayer();
    }

    void GetInput()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        float targetSpeed = inputDirection.magnitude > 0.1f ? moveSpeed : 0f;
        currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref velocityRef, smoothTime);
    }

    void MovePlayer()
    {
        Vector3 moveVelocity = inputDirection * moveSpeed;
        rb.linearVelocity = new Vector3(moveVelocity.x, rb.linearVelocity.y, moveVelocity.z);
    }

    void RotatePlayer()
    {
        if (inputDirection.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(inputDirection);
            Quaternion smoothed = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Euler(0f, smoothed.eulerAngles.y, 0f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {

    }

    private void OnTriggerExit(Collider other)
    {

    }
}