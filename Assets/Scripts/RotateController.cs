using Unity.IntegerTime;
using Unity.Mathematics;
using UnityEngine;

public class RotateController : MonoBehaviour
{
    public GameObject playerPivot;
    public GameObject cameraPivot;
    private float rotationSpeed;
    private float currentSpeed;
    private float rotationInput;

    public void SetValues(float rotation, float speed, float input)
    {
        rotationSpeed = rotation;
        currentSpeed = speed;
        rotationInput = input;
    }

    void Update()
    {;
        RotatePlayer();
    }
    void RotatePlayer()
    {
        if (Mathf.Abs(rotationInput) > 0.1f && currentSpeed == 0)
        {
            float rotationAmount = rotationInput * rotationSpeed * Time.deltaTime;
            playerPivot.transform.Rotate(0f, 0f, rotationAmount);
            cameraPivot.transform.Rotate(0f, 0f, rotationAmount);
        }
        else if(currentSpeed != 0)
        {
            playerPivot.transform.localRotation = Quaternion.identity;
            //cameraPivot.transform.rotation = playerPivot.transform.rotation;
            cameraPivot.transform.rotation = Quaternion.Euler(90 * Mathf.Rad2Deg, playerPivot.transform.rotation.eulerAngles.y, playerPivot.transform.rotation.eulerAngles.z);

        }
    }
}
