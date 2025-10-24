using UnityEngine;

public class Animations : MonoBehaviour
{
    Animator anim;
    public MoveController mc;
    public JumpController jc;
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (!mc.canMove || !jc.canJump) return;
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