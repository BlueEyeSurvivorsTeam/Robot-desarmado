using NUnit.Framework;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEditor.Animations;
using UnityEngine;

public class RobotState : MonoBehaviour
{
    public static RobotState Instance;
    [Header("Override Controllers")]
    public AnimatorOverrideController robotWithRocket;
    public AnimatorOverrideController robotWithProyectile;
    public AnimatorOverrideController torso;

    [Header("Estados")]
    public bool hasProyectileHand = true;
    public bool hasRocketHand = true;
    public bool hasLegs = true;
    public bool isSeparate = false;

    public PhysicsMaterial staticMat;
    public PhysicsMaterial moveMat;
    public GameObject currentTarget;
    private List<GameObject> parts = new List<GameObject>();
    private void Awake()
    {
        Instance = this;
    }
    public void SetParts(GameObject legsPart, GameObject torsoPart)
    {
        parts.Add(legsPart); 
        parts.Add(torsoPart);  
    }
    public void SetCurrentPart(GameObject current)
    {
        currentTarget = current;
        OffOtherParts();
    }
    void OffOtherParts()
    {
        for (int i = 0; i < parts.Count; i++)
        {
            JumpController jc = parts[i].GetComponent<JumpController>();
            RotateController rc = parts[i].GetComponent<RotateController>();
            MoveController mc = parts[i].GetComponent<MoveController>();
            Rigidbody rb = parts[i].GetComponent<Rigidbody>();
            Collider c = parts[i].GetComponent<Collider>();
            if (parts[i] == currentTarget)
            {
                if (jc != null) jc.canJump = true;
                if (rc != null) rc.canRotate = true;
                if (mc != null) mc.canMove = true; mc.canRun = true;
                if (rb != null) rb.constraints = RigidbodyConstraints.FreezeRotation;
                if (c != null) c.material = moveMat;
            }
            else
            {
                if (jc != null) jc.canJump = false;
                if (rc != null) rc.canRotate = false;
                if (mc != null) mc.canMove = false; mc.canRun = false;
                if (rb != null) rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
                if (c != null) c.material = staticMat;
            }
        }
    }
}
