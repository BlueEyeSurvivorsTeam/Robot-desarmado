using UnityEngine;

public class Animations : MonoBehaviour
{
    Animator anim;
    public MoveController mc;
    public JumpController jc;
    public RocketHandController rhc;
    public ProyectileHandController phc;
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (mc.canMove)
        {
            if (mc)
            {
                anim.SetFloat("Move", mc.currentAnimSpeed);
            }
        }
        if(jc.canJump)
        {
            if(jc)
            {
                anim.SetBool("IsJumping", jc.isJumping);
            }
        }
        if (rhc != null && rhc.canShoot && anim.runtimeAnimatorController == RobotState.Instance.robotWithRocket)
        {
            if(rhc)
            {
                anim.SetBool("IsAiming", rhc.isAiming);
            }
        }
        if (phc != null && phc.canShoot && anim.runtimeAnimatorController == RobotState.Instance.robotWithProyectile)
        {
            if (phc)
            {
                anim.SetBool("IsAiming", phc.isAiming);
            }
        }
    }
}