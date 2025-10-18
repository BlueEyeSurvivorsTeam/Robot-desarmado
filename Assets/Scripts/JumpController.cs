using UnityEngine;

public class JumpController : MonoBehaviour
{
    public float jumpValue;
    public bool canJump;
    public KeyCode jumpKey = KeyCode.Space;
    bool isGrounded;
    public bool isJumping { get; private set; }
    Rigidbody rb;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void IsGrounded(bool grounded)
    {
        isGrounded = grounded;
        if(isGrounded && isJumping)
        {
            isJumping = false;
        }
    }
    void Update()
    {
        if (canJump && isGrounded)
        {
            GetInput();
        }
    }
    void GetInput()
    {
        if (Input.GetKey(jumpKey))
        {
            Jump();
        }
    }
    void Jump()
    {
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpValue, rb.linearVelocity.z);
        isJumping = true;
    }
}
