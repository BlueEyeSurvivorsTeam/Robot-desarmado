using UnityEngine;

public class Animations : MonoBehaviour
{
    Animator anim;
    MoveController mc;
    JumpController jc;
    void Start()
    {
        anim = GetComponent<Animator>();
        if (GetComponentInParent<MoveController>())
        {
            mc = GetComponentInParent<MoveController>();
        }
        if(GetComponentInParent<JumpController>())
        {
            jc = GetComponentInParent<JumpController>();
        }
    }

    void Update()
    {
        if (mc)
        {
            anim.SetFloat("Move", mc.currentAnimSpeed);
        }
        if(jc)
        {
            anim.SetBool("IsJumping", jc.isJumping);
        }
    }
}