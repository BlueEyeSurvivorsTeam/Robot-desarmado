using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.WSA;

public class RobotChangeParts : MonoBehaviour
{
    [Header("Referencias")]
    public Animator animPlayer;
    public PartDetector partDetector;

    [Header("Override Controllers")]
    public AnimatorOverrideController robot;
    public AnimatorOverrideController torso;

    [Header("Parte")]
    public GameObject legs;
    public Transform legsPoint;

    [Header("Botones")]
    public KeyCode changePartButton = KeyCode.Tab;

    [Header("Cámaras")]
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
                    ChangePart(robot,false,false,legs,1,0,this.gameObject);
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
                    ChangePart(torso, true, true, legs, 1, 0, this.gameObject);
                }
                else if (Input.GetKeyDown(KeyCode.Alpha2) && !state.isSeparate)
                {
                    ChangePart(torso, true, true, legs, 0, 1, legs);
                }
            }
        }   
    }
    public void ChangePart(AnimatorOverrideController anim, bool isSeparate, bool dropping, GameObject drop, int playerPriority, int legsPriority, GameObject current)
    {
        animPlayer.runtimeAnimatorController = anim;
        state.isSeparate = isSeparate;
        if(dropping) Drop(drop);
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
    void Drop(GameObject part)
    {
        part.SetActive(true);
        part.transform.SetParent(null, false);
        part.transform.position = transform.position + Vector3.down;
    }
    void Take(GameObject part)
    {
        part.SetActive(false);
        part.transform.SetParent(legsPoint, false);
        part.transform.position = legsPoint.position;
    }
}
