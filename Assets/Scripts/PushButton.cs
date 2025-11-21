using UnityEngine;

public class PushButton : MonoBehaviour
{
    public MoveController mc;
    public RotateController rc;
    public JumpController jc;

    public void Push()
    {
        if(mc != null) mc.canMove = false;
        if(rc != null) rc.canRotate = false;
        if(jc != null) jc.canJump = false;
    }
    public void EndPush()
    {
        if (mc != null) mc.canMove = true;
        if (rc != null) rc.canRotate = true;
        if (jc != null) jc.canJump = true;
    }
}
