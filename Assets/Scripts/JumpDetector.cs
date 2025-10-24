using NUnit.Framework.Internal.Commands;
using UnityEngine;

public class JumpDetector : MonoBehaviour
{
    public JumpController jc;
    bool isGrounded;
    bool touchPlayer;

    void OnTriggerStay(Collider other)
    {
        if (other != this.GetComponentInParent<MoveController>())
        {
            isGrounded = true;
        }
        if (other.GetComponentInParent<MoveController>() && other != this.GetComponentInParent<MoveController>())
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
        if (other != this.GetComponentInParent<MoveController>())
        {
            isGrounded = false;
        }
        if (other.GetComponentInParent<MoveController>() && other != this.GetComponentInParent<MoveController>())
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
