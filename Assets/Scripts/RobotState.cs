using UnityEditor.Animations;
using UnityEngine;

public class RobotState : MonoBehaviour
{
    public Animator animPlayer;
    [Header("Override Controllers")]
    public AnimatorOverrideController robot;
    public AnimatorOverrideController legs;
    public AnimatorOverrideController torso;

    [Header("Botones")]
    public KeyCode selectPart = KeyCode.Q;
    public KeyCode separate = KeyCode.Mouse0;

    private AnimatorOverrideController current;

    private void Update()
    {
        GetInput();
    }

    void GetInput()
    {
        if(Input.GetKey(selectPart))
        {
            if(Input.GetKeyDown(KeyCode.Alpha1))
            {
                current = robot;
            }
            else if(Input.GetKeyDown(KeyCode.Alpha2))
            {
                current = legs;
            }
            else if(Input.GetKeyDown(KeyCode.Alpha3))
            {
                current = torso;
            }
        }
        if(Input.GetKeyDown(separate) && current != null)
        {
            animPlayer.runtimeAnimatorController = current;
        }
    }
}
