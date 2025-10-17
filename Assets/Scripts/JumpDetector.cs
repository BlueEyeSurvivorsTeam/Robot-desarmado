using UnityEngine;

public class JumpDetector : MonoBehaviour
{
    JumpController jc;

    void Start()
    {
        jc = GetComponentInParent<JumpController>();
    }

    void OnTriggerEnter(Collider other)
    {
        jc.IsGrounded(true);
    }
    void OnTriggerExit(Collider other)
    {
        jc.IsGrounded(false);
    }

}
