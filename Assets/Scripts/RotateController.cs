using Unity.IntegerTime;
using Unity.Mathematics;
using UnityEngine;

public class RotateController : MonoBehaviour
{
    public GameObject pivot;
    private float rotationSpeed;
    private float currentSpeed;
    private float rotationInput;
    public void SetValues(float rotation, float speed)
    {
        rotationSpeed = rotation;
        currentSpeed = speed;
    }
    void Update()
    {
        rotationInput = Input.mousePositionDelta.x;
        RotatePlayer();
    }
    void RotatePlayer()
    {
        if (Mathf.Abs(rotationInput) > 0.1f && currentSpeed == 0)
        {
            float rotationAmount = rotationInput * rotationSpeed * Time.deltaTime;
            pivot.transform.Rotate(0f, 0f, rotationAmount);
        }
        else if(currentSpeed != 0)
        {
            pivot.transform.localRotation = Quaternion.identity;
        }
    }
}
