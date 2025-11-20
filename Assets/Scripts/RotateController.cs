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
        if (GameManager.Instance != null && GameManager.Instance.isPaused) return;
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
            ResetRotation();
        }
    }
    void RotatePlayer()
    {
        Vector2 mouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        Vector2 moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        if (mouseDelta != Vector2.zero)
        {
            float rotationAmount = mouseDelta.x * rotationSpeed * Time.deltaTime;
            if(moveInput != Vector2.zero)
            {
                transform.Rotate(0f, rotationAmount, 0f);
            }
            else
            {
                playerPivot.transform.Rotate(0f, 0f, rotationAmount);
                cameraPivot.transform.Rotate(0f, 0f, rotationAmount);
            }
        }
    }
    public void ResetRotation()
    {
        playerPivot.transform.localRotation = Quaternion.identity;
        cameraPivot.transform.rotation = playerPivot.transform.rotation;
    }
}
