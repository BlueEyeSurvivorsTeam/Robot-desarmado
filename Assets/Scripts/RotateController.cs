using Unity.IntegerTime;
using Unity.Mathematics;
using UnityEngine;

public class RotateController : MonoBehaviour
{
    public bool canRotate = true;
    public GameObject playerPivot;
    public GameObject cameraPivot;
    public KeyCode resetKey = KeyCode.R;
    public float rotationSpeed = 120;

    void Update()
    {
        if (canRotate)
        {
            GetInput();
            RotatePlayer();
        }
    }
    void GetInput()
    {
        if(Input.GetKeyDown(resetKey))
        {
            playerPivot.transform.localRotation = Quaternion.identity;
            cameraPivot.transform.rotation = playerPivot.transform.rotation;
        }
    }
    void RotatePlayer()
    {
        if (Input.mousePositionDelta != Vector3.zero)
        {
            float rotationAmount = Input.mousePositionDelta.x * rotationSpeed * Time.deltaTime;
            playerPivot.transform.Rotate(0f, 0f, rotationAmount);
            cameraPivot.transform.Rotate(0f, 0f, rotationAmount);
        }
    }
}
