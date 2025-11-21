using UnityEngine;

public class JumpController : MonoBehaviour
{
    public float jumpValue;
    public bool canJump;
    public KeyCode jumpKey = KeyCode.Space;
    public bool isJumping { get; private set; }
    public JumpDetector jumpDetector;
    Rigidbody rb;
    RobotChangeParts robot;
    int jumpBuffer = 0;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        robot = GetComponent<RobotChangeParts>();
    }

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.isPaused)
        {
            rb.linearVelocity = Vector3.zero;
            return;
        }
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
        if (Input.GetKeyDown(jumpKey) && isJumping && !jumpDetector.IsGrounded() && RobotState.Instance.hasProyectileHand && RobotState.Instance.hasRocketHand && robot.canSeparate)
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
        if (!RobotState.Instance.isSeparate)
        {
            if (robot)
            {
                robot.ChangePart(robot.torso, true, true, robot.legs, robot.legsPoint, Vector3.down + (-transform.forward), robot.playerCam, this.gameObject);
                Jump();
            }
        }
    }
    void UnitOnJump()
    {
        if (robot)
        {
            transform.position = robot.legs.transform.position;
            robot.ChangePart(robot.robotWithRocket, false, false, robot.legs, robot.legsPoint, Vector3.zero, robot.playerCam, this.gameObject);
        }
    }
}
