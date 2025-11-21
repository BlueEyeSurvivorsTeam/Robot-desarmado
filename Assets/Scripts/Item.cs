using UnityEngine;
using System.Collections;
public class Item : MonoBehaviour
{
    public PieceType type;
    public GameObject model;
    [Header("Item rotativo")]
    public bool isRotating = false;
    public bool rotateX = false;
    public bool rotateY = false;
    public bool rotateZ = false;
    public float rotationSpeed = 90f;

    [Header("Item flotante")]
    public bool isFloating = false;
    public bool useEasingForFloating = false;
    public float floatHeight = 1f;
    public float floatSpeed = 1f;
    private Vector3 initialPosition;
    private float floatTimer;


    [Header("Item palpitante")]
    public bool isScaling = false;
    public bool useEasingForScaling = false;
    public float scaleLerpSpeed = 1f;
    private float scaleTimer;
    private Vector3 initialScale;
    public Vector3 startScale;
    public Vector3 endScale;

    void Start()
    {
        initialScale = transform.localScale;
        initialPosition = transform.position;

        startScale = initialScale;
        endScale = initialScale * (endScale.magnitude / startScale.magnitude);
    }

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.isPaused)
        {
            return;
        }
        if (isRotating)
        {
            Vector3 rotationVector = new Vector3(
                rotateX ? 1 : 0,
                rotateY ? 1 : 0,
                rotateZ ? 1 : 0
            );
            transform.Rotate(rotationVector * rotationSpeed * Time.deltaTime);
        }

        if (isFloating)
        {
            floatTimer += Time.deltaTime * floatSpeed;
            float t = Mathf.PingPong(floatTimer, 1f);
            if (useEasingForFloating) t = EaseInOutQuad(t);

            transform.position = initialPosition + new Vector3(0, t * floatHeight, 0);
        }

        if (isScaling)
        {
            scaleTimer += Time.deltaTime * scaleLerpSpeed;
            float t = Mathf.PingPong(scaleTimer, 1f);

            if (useEasingForScaling)
            {
                t = EaseInOutQuad(t);
            }

            transform.localScale = Vector3.Lerp(startScale, endScale, t);
        }
    }

    float EaseInOutQuad(float t)
    {
        return t < 0.5f ? 2 * t * t : 1 - Mathf.Pow(-2 * t + 2, 2) / 2;
    }
    public void OnTriggerEnter(Collider other)
    {
        if(other.GetComponentInParent<RobotChangeParts>() && other.GetComponentInParent<RobotChangeParts>().NeedRepair(type)) 
        {
            other.GetComponentInParent<RobotChangeParts>().Repair(type);
            model.SetActive(false);
            enabled = false;
        }
    }
}
