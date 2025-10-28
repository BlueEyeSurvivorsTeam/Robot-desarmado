using Unity.Cinemachine;
using UnityEngine;

public class RobotChangeParts : MonoBehaviour
{
    [Header("Referencias")]
    public Animator animPlayer;
    public PartDetector partDetector;

    [Header("Override Controllers")]
    public AnimatorOverrideController robot;
    public AnimatorOverrideController torso;

    [Header("Parte")]
    public GameObject faceCamera;
    public GameObject legs;
    public Transform legsPoint;

    [Header("Botones")]
    public KeyCode changePartButton = KeyCode.Tab;

    [Header("C�maras")]
    public CinemachineCamera playerCam;
    public CinemachineCamera legsCam;

    private RobotState state;
    private void Start()
    {
        if(legs != null) legs.SetActive(false);
        state = RobotState.Instance;
        state.SetParts(legs, this.gameObject);
    }
    private void Update()
    {
        GetInput();
    }

    void GetInput()
    {
        if (Input.GetKey(changePartButton))
        {
            if(state.isSeparate)
            {
                if (Input.GetKeyDown(KeyCode.Alpha3) && state.isSeparate && partDetector.IsPartClose())
                {
                    ChangePart(robot,false,false,legs, Vector3.zero,1,0,this.gameObject);
                }
                else if(Input.GetKeyDown(KeyCode.Alpha1) && state.isSeparate)
                {
                    ChangePart(1, 0, this.gameObject);
                }
                else if(Input.GetKeyDown(KeyCode.Alpha2) && state.isSeparate)
                {
                    ChangePart(0, 1, legs);
                }
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.Alpha1) && !state.isSeparate)
                {
                    ChangePart(torso, true, true, legs, -transform.forward, 1, 0, this.gameObject);
                }
                else if (Input.GetKeyDown(KeyCode.Alpha2) && !state.isSeparate)
                {
                    ChangePart(torso, true, true, legs, -transform.forward, 0, 1, legs);
                }
            }
        }   
    }
    public void ChangePart(AnimatorOverrideController anim, bool isSeparate, bool dropping, GameObject drop, Vector3 direction, int playerPriority, int legsPriority, GameObject current)
    {
        animPlayer.runtimeAnimatorController = anim;
        state.isSeparate = isSeparate;
        if(dropping) Drop(drop, direction);
        else Take(drop);
        playerCam.Priority = playerPriority;
        legsCam.Priority = legsPriority;
        state.SetCurrentPart(current);     
    }
    public void ChangePart(int playerPriority, int legsPriority, GameObject current)
    {
        playerCam.Priority = playerPriority;
        legsCam.Priority = legsPriority;
        state.SetCurrentPart(current);
    }
    void Drop(GameObject part, Vector3 direction)
    {
        part.SetActive(true);
        part.transform.SetParent(null, false);
        part.transform.position = transform.position + (direction + (Vector3.up * 0.5f));
        faceCamera.SetActive(true);
    }
    void Take(GameObject part)
    {
        part.SetActive(false);
        part.transform.SetParent(legsPoint, false);
        part.transform.position = legsPoint.position;
        faceCamera.SetActive(false);
    }
}
