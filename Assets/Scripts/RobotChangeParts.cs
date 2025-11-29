using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class RobotChangeParts : MonoBehaviour
{
    public bool canSeparate = true;
    [Header("Referencias")]
    public Animator animPlayer;
    public Animator partsPanel;
    [Header("Override Controllers")]
    public AnimatorOverrideController robotWithRocket;
    public AnimatorOverrideController robotWithProyectile;
    public AnimatorOverrideController torso;

    [Header("Parte y puntos")]
    public GameObject faceCamera;
    public GameObject legs;
    public Transform legsPoint;
    public GameObject rocketHand;
    public Transform rocketHandPoint;
    public GameObject proyectileHand;
    public Transform proyectileHandPoint;

    [Header("Botones")]
    public KeyCode changePartButton = KeyCode.Tab;

    [Header("C�maras")]
    public CinemachineCamera playerCam;
    public CinemachineCamera legsCam;
    public CinemachineCamera rocketCam;
    public CinemachineCamera proyectileCam;
    private List<CinemachineCamera> cams = new List<CinemachineCamera>();
    private RobotState state;
    private void Start()
    {
        if(legs != null) legs.SetActive(false);
        state = RobotState.Instance;
        state.SetParts(legs, this.gameObject);
        cams.Add(playerCam);
        cams.Add(legsCam);
        cams.Add(rocketCam);
        if(GameManager.Instance.reloadScene)
        {
            GameManager.Instance.SetReload(false);
            transform.position = GameManager.Instance.GetPos();
        }
        else
        {
            GameManager.Instance.SetPos(gameObject);
        }
    }
    private void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.isPaused) return;
        GetInput();
    }

    void GetInput()
    {
        if(Input.GetKeyDown(changePartButton))
        {
            ChangePart(playerCam, this.gameObject);
        }
        if (Input.GetKey(changePartButton))
        {
            if(partsPanel != null)
            {
                partsPanel.SetBool("Open", true);
            }
            if(state.isSeparate)
            {
                if(Input.GetKeyDown(KeyCode.Alpha1) && state.isSeparate && state.hasLegs)
                {
                    ChangePart(legsCam, legs);
                }
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.Alpha1) && state.hasProyectileHand && state.hasRocketHand && canSeparate)
                {
                    ChangePart(torso, true, true, legs, legsPoint, -transform.forward + (Vector3.up * 0.5f), legsCam, legs);
                }
                else if (Input.GetKeyDown(KeyCode.Alpha3) && state.hasRocketHand)
                {
                    ChangePart(robotWithRocket, false, false, rocketHand, rocketHandPoint, Vector3.zero, playerCam, this.gameObject);
                    state.currentTarget = rocketHand;
                }
                else if (Input.GetKeyDown(KeyCode.Alpha2) && !proyectileHand.activeInHierarchy)
                {
                    ChangePart(robotWithProyectile, false, false, proyectileHand, proyectileHandPoint, Vector3.zero, playerCam, this.gameObject);
                    state.currentTarget = proyectileHand;
                }
                else if (Input.GetKeyDown(KeyCode.Alpha2) && proyectileHand.activeInHierarchy)
                {
                    ChangePart(proyectileCam, proyectileHand);
                }
            }
        }
        else
        {
            if (partsPanel != null)
            {
                partsPanel.SetBool("Open", false);
            }
        }
    }
    public void ChangePart(AnimatorOverrideController anim, bool isSeparate, bool dropping, GameObject drop, Transform parent, Vector3 direction, CinemachineCamera camPriority, GameObject current)
    {
        if (dropping) Drop(drop, direction);
        else Take(drop, parent);
        animPlayer.runtimeAnimatorController = anim;
        state.isSeparate = isSeparate;
        camPriority.Priority = 1;
        for (int i = 0; i < cams.Count; i++)
        {
            if (cams[i] != camPriority)
            {
                cams[i].Priority = 0;
            }
        }
        state.SetCurrentPart(current);     
    }
    public void ChangePart(CinemachineCamera camPriority, GameObject current)
    {
        camPriority.Priority = 1;
        for (int i = 0; i < cams.Count; i++)
        {
            if (cams[i] != camPriority)
            {
                cams[i].Priority = 0;
            }
        }
        state.SetCurrentPart(current);
    }
    void Drop(GameObject part, Vector3 direction)
    {
        Transform parent = part.transform.parent;
        part.transform.SetParent(null, true);
        part.SetActive(true);

        if (Physics.Raycast(part.transform.position, direction.normalized, out RaycastHit hit, direction.magnitude))
        {
            part.transform.position = hit.point + hit.normal * 0.05f;
        }
        else
        {
            part.transform.position += direction;
        }
        faceCamera.SetActive(true);
    }
    void Take(GameObject part, Transform parent)
    {
        part.transform.SetParent(parent, false);
        part.transform.localPosition = Vector3.zero;
        part.transform.localRotation = Quaternion.identity;
        part.transform.localScale = Vector3.one;
        part.SetActive(false);
        faceCamera.SetActive(false);
    }
    public void Repair(PieceType piece)
    {
        if(piece == PieceType.Legs)
        {
            state.hasLegs = true;
            ChangePart(torso, true, true, legs, legsPoint, -transform.forward + (Vector3.up * 0.5f), playerCam, this.gameObject);
        }
        else if(piece == PieceType.ProyectileHand)
        {
            ProyectileHandController phc = GetComponent<ProyectileHandController>();
            phc.originalHand.SetActive(true);
            state.hasProyectileHand = true;
            Take(proyectileHand, proyectileHandPoint);
        }
        else
        {
            if(GetComponent<RocketHandController>())
            {
                RocketHandController rhc = GetComponent<RocketHandController>();
                rhc.originalHand.SetActive(true);
                state.hasRocketHand = true;
            }
        }
    }
    public bool NeedRepair(PieceType piece)
    {
        if (piece == PieceType.Legs && !state.hasLegs)
        {
            return true;
        }
        else if (piece == PieceType.ProyectileHand && !state.hasProyectileHand)
        {
            return true;
        }
        else if (piece == PieceType.RocketHand && !state.hasRocketHand)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public void SetCanSeparate(bool boolean)
    {
        canSeparate = boolean;
    }
}
