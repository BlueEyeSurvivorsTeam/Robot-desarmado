using UnityEngine;
using System.Collections.Generic;

public class JumpDetector : MonoBehaviour
{
    public JumpController jc;

    bool touchPlayer;

    private HashSet<Collider> groundColliders = new HashSet<Collider>();

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ground"))
        {
            groundColliders.Add(other);
        }
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
        if (other.CompareTag("Ground"))
        {
            groundColliders.Remove(other);
        }

        if (other.GetComponentInParent<MoveController>())
        {
            touchPlayer = false;
        }
    }

    public bool IsGrounded()
    {
        return groundColliders.Count > 0;
    }

    public bool TouchPlayer()
    {
        return touchPlayer;
    }
}
