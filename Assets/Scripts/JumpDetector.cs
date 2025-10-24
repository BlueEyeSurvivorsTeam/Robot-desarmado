using NUnit.Framework.Internal.Commands;
using UnityEngine;

public class JumpDetector : MonoBehaviour
{
    public JumpController jc;
    bool isGrounded;
    bool touchPlayer;

    void OnTriggerEnter(Collider other)
    {
        isGrounded = true;
    }
    void OnTriggerStay(Collider other)
    {
        if (other.GetComponentInParent<MoveController>())
        {
            touchPlayer = true;
        }
        else
        {
            touchPlayer = false;
        }
    }
    void OnTriggerExit(Collider other)
    {
        isGrounded = false;
        if (other.GetComponentInParent<MoveController>())
        {
            touchPlayer = false;
        }
    }
    public bool IsGrounded()
    {
        return isGrounded;
    }
    public bool TouchPlayer()
    {
        return touchPlayer;
    }
}
