using UnityEngine;

public class RocketHand : MonoBehaviour
{
    public float moveSpeed = 8f;
    public float rotationSpeed = 10f;
    private RocketHandController rhc;
    private Rigidbody rb;
    private Vector3 rotationInput;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }
    private void Update()
    {
        GetInput();
    }
    private void FixedUpdate()
    {
        rb.linearVelocity = transform.forward * moveSpeed;
        Rotate();
    }
    public void GetInput()
    {
        rotationInput.x = -Input.GetAxis("Vertical");
        rotationInput.y = Input.GetAxis("Horizontal");
    }
    void Rotate()
    {
        if (rotationInput != Vector3.zero)
        {
            Vector3 rotationAmount = rotationInput * rotationSpeed * Time.deltaTime;
            transform.Rotate(rotationAmount);
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        rhc.LoseHand();
        this.gameObject.SetActive(false);
    }
    public void SetRHC(RocketHandController rhcontroller)
    {
        rhc = rhcontroller;
    }
}
