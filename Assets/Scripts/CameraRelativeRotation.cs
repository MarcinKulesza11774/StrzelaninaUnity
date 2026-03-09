using UnityEngine;

public class CameraRelativeRotation : MonoBehaviour
{
    public Transform cameraTransform;
    public PlayerMovement movement;
    public float rotationSpeed = 10f;

    void Update()
    {
        if (!movement.IsMoving())
            return;

        Vector3 awayFromCamera = movement.MoveDirection();
        awayFromCamera.y = 0;

        if (awayFromCamera.sqrMagnitude < 0.001f)
            return;

        Quaternion targetRotation = Quaternion.LookRotation(awayFromCamera);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }
}
