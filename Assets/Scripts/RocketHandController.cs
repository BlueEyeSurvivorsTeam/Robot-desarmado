using UnityEngine;

public class RocketHandController : MonoBehaviour
{
    public float rotationSpeed = 10f;
    public bool canShoot;
    public GameObject playerPivot;
    public GameObject cameraPivot;
    public GameObject originalHand;
    public KeyCode aimKey = KeyCode.Mouse1;
    public KeyCode shootKey = KeyCode.Mouse0;
    public bool isShooting { get; private set; }
    public bool isAiming { get; private set; }
    public RocketHand rocketHand;
    RobotChangeParts robot;
    RobotState state;
    void Start()
    {
        robot = GetComponent<RobotChangeParts>();
        state = RobotState.Instance;
    }
    private void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.isPaused) return;
        if (canShoot && !isShooting && state.hasRocketHand && state.currentTarget == rocketHand.gameObject && !state.isSeparate)
        {
            GetInput();
        }
    }
    void GetInput()
    {
        if (Input.GetKeyDown(aimKey) && !isAiming)
        {
            isAiming = true;
            robot.ChangePart(robot.robotWithRocket, false, false, robot.rocketHand, robot.rocketHandPoint, Vector3.zero, robot.playerCam, robot.rocketHand);
        }
        else if(Input.GetKeyUp(aimKey) && isAiming)
        {
            isAiming = false;
            robot.ChangePart(robot.robotWithRocket, false, false, robot.rocketHand, robot.rocketHandPoint, Vector3.zero, robot.playerCam, this.gameObject);
            state.currentTarget = robot.rocketHand;
        }
        if (Input.GetKeyDown(shootKey) && isAiming)
        {
            isShooting = true;
            state.hasRocketHand = false;
            originalHand.SetActive(false);
            rocketHand.SetRHC(this);
            robot.ChangePart(robot.robotWithRocket, false, true, robot.rocketHand, robot.rocketHandPoint, transform.forward * 0.5f, robot.rocketCam, robot.rocketHand);
            isAiming = false;
        }
    }
    private void FixedUpdate()
    {
        if(isAiming)
        {
            RotatePlayer();
        }
    }
    void RotatePlayer()
    {
        if (Input.mousePositionDelta != Vector3.zero)
        {
            float rotationAmount = Input.mousePositionDelta.x * rotationSpeed * Time.deltaTime;
            playerPivot.transform.Rotate(0f, 0f, rotationAmount);
            cameraPivot.transform.Rotate(0f, 0f, rotationAmount);
        }
    }
    public void LoseHand()
    {
        isShooting = false;
        robot.ChangePart(robot.robotWithRocket, false, false, robot.rocketHand, robot.rocketHandPoint, Vector3.zero, robot.playerCam, this.gameObject);
    }
    public void SetCanShoot(bool boolean)
    {
        canShoot = boolean;
    }
}
