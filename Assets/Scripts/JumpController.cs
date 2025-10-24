using UnityEngine;

public class JumpController : MonoBehaviour
{
    public float jumpValue;
    public bool canJump;
    public KeyCode jumpKey = KeyCode.Space;
    public bool isJumping { get; private set; }
    public JumpDetector jumpDetector;
    Rigidbody rb;
    int jumpBuffer = 0;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (canJump)
        {
            GetInput();
        }
        if (RobotState.Instance.isSeparate && jumpDetector.TouchPlayer())
        {
            UnitOnJump();
        }
    }
    void FixedUpdate()
    {
        if (jumpBuffer > 0)
        {
            jumpBuffer -= 1;
        }
        else if (jumpDetector.IsGrounded() && isJumping)
        {
            isJumping = false;
            jumpBuffer = 3;
        }
    }
    void GetInput()
    {
        if (Input.GetKeyDown(jumpKey) && isJumping && !jumpDetector.IsGrounded())
        {
            Separate();
        }
        else if (Input.GetKeyDown(jumpKey) && jumpDetector.IsGrounded())
        {
            Jump();
        }
    }
    void Jump()
    {
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpValue, rb.linearVelocity.z);
        isJumping = true;
        jumpBuffer = 3;
    }
    void Separate()
    {
        if(!RobotState.Instance.isSeparate)
        {
            if(GetComponent<RobotChangeParts>())
            {
                RobotChangeParts robot = GetComponent<RobotChangeParts>();
                robot.ChangePart(robot.torso, true, true, robot.legs, Vector3.down - Vector3.left, 1, 0, this.gameObject);
                Jump();
            }
        }
    }
    void UnitOnJump()
    {
        if (GetComponent<RobotChangeParts>())
        {
            RobotChangeParts robot = GetComponent<RobotChangeParts>();
            transform.position = robot.legs.transform.position;
            robot.ChangePart(robot.robot, false, false, robot.legs, Vector3.zero, 1, 0, this.gameObject);
        }
    }
}
