using UnityEngine;

public class ProyectileHandController : MonoBehaviour
{
    public float rotationSpeed = 100f;
    public float force = 10f;
    public bool canShoot;
    public RotateController rc;
    public GameObject originalHand;
    public ProyectileHand proyectileHand;
    public KeyCode aimKey = KeyCode.Mouse1;
    public KeyCode shootKey = KeyCode.Mouse0;
    public LineRenderer lineRenderer;
    [Range(10, 100)] public int linePoints = 175;
    [Range(0.01f, 0.25f)] public float timeIntervalInPoints = 0.01f;
    public LayerMask collisionMask;
    public bool isShooting { get; private set; }
    public bool isAiming { get; private set; }
    RobotChangeParts robot;
    RobotState state;

    void Start()
    {
        robot = GetComponent<RobotChangeParts>();
        state = RobotState.Instance;
    }

    private void Update()
    {
        if (canShoot && !isShooting && state.hasLeftArm && state.currentTarget == proyectileHand.gameObject && !state.isSeparate)
        {
            GetInput();
        }
    }

    void GetInput()
    {
        if (Input.GetKeyDown(aimKey) && !isAiming)
        {
            rc.ResetRotation();
            isAiming = true;
            lineRenderer.enabled = true;
            robot.ChangePart(robot.robotWithProyectile, false, false, robot.proyectileHand, robot.proyectileHandPoint, Vector3.zero, robot.playerCam, robot.proyectileHand);
        }
        else if (Input.GetKeyUp(aimKey) && isAiming)
        {
            isAiming = false;
            lineRenderer.enabled = false;
            robot.ChangePart(robot.robotWithProyectile, false, false, robot.proyectileHand, robot.proyectileHandPoint, Vector3.zero, robot.playerCam, this.gameObject);
            state.currentTarget = robot.proyectileHand;
        }
        if (Input.GetKeyDown(shootKey) && isAiming)
        {
            lineRenderer.enabled = false;
            isShooting = true;
            state.hasLeftArm = false;
            originalHand.SetActive(false);
            robot.ChangePart(robot.robotWithProyectile, false, true, robot.proyectileHand, robot.proyectileHandPoint, transform.forward * 0.5f, robot.proyectileCam, robot.proyectileHand);
            proyectileHand.Throw(this, force * robot.proyectileHandPoint.forward);
            isAiming = false;
        }
    }

    private void FixedUpdate()
    {
        if (isAiming)
        {
            DrawTrayectory();
        }
    }

    public void LoseHand()
    {
        isShooting = false;
        robot.ChangePart(robot.playerCam, this.gameObject);
    }

    void DrawTrayectory()
    { 
        lineRenderer.enabled = true;
        lineRenderer.positionCount = linePoints;
        Vector3 startPosition = robot.proyectileHand.transform.localPosition;
        Vector3 startVelocity = force * robot.proyectileHand.transform.forward / proyectileHand.GetComponent<Rigidbody>().mass;

        for (int i = 0; i < linePoints; i++)
        {
            float time = i * timeIntervalInPoints;
            Vector3 point = startPosition + startVelocity * time;
            point.y = startPosition.y + startVelocity.y * time + 0.5f * Physics.gravity.y * time * time;
            lineRenderer.SetPosition(i, point);

            if (i > 0)
            {
                Vector3 lastPos = lineRenderer.GetPosition(i - 1);
                if (Physics.Raycast(lastPos, (point - lastPos).normalized, out RaycastHit hit, (point - lastPos).magnitude, collisionMask))
                {
                    lineRenderer.SetPosition(i, hit.point);
                    lineRenderer.positionCount = i + 1;
                    break;
                }
            }
        }
    }
}
