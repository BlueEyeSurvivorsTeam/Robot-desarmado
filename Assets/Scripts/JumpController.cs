using UnityEngine;

public class JumpController : MonoBehaviour
{
    public float jumpValue;
    public bool canJump;
    public KeyCode jumpKey = KeyCode.Space;
    public bool isJumping { get; private set; }
    public JumpDetector jumpDetector;
    Rigidbody rb;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (jumpDetector.IsGrounded() && isJumping)
        {
            isJumping = false;
        }
        if (canJump)
        {
            GetInput();
        }
        if(RobotState.Instance.isSeparate && jumpDetector.TouchPlayer())
        {
            UnitOnJump();
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
    }
    void Separate()
    {
        if(!RobotState.Instance.isSeparate)
        {
            if(GetComponent<RobotChangeParts>())
            {
                RobotChangeParts robot = GetComponent<RobotChangeParts>();
                robot.ChangePart(robot.torso, true, true, robot.legs, 1, 0, this.gameObject);
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
            robot.ChangePart(robot.robot, false, false, robot.legs, 1, 0, this.gameObject);
        }
    }
}
