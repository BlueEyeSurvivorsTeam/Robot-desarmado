using UnityEngine;

public class Animations : MonoBehaviour
{
    Animator anim;
    PlayerController pc;
    void Start()
    {
        anim = GetComponent<Animator>();
        if (GetComponentInParent<PlayerController>())
        {
            pc = GetComponentInParent<PlayerController>();
        }
    }

    void Update()
    {
        if (pc)
        {
            anim.SetFloat("Move", pc.currentSpeed);
        }
    }
}