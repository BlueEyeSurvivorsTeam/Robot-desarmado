using UnityEngine;

public class ProyectileHand : MonoBehaviour
{
    Rigidbody rb;
    ProyectileHandController phc;
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.CompareTag("PressurePlate"))
        {
            phc.LoseHand();
            rb.linearVelocity = Vector3.zero;
        }
        if(collision.collider.CompareTag("Ground"))
        {
            phc.LoseHand();
        }
    }
    public void Throw(ProyectileHandController phcontroller, Vector3 speed)
    {
        phc = phcontroller;
        rb = GetComponent<Rigidbody>();
        rb.AddForce(speed, ForceMode.Impulse);
    }
}
