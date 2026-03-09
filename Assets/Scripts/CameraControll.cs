using UnityEngine;

public class CameraControll : MonoBehaviour
{
    public Transform player;
    public float mouseSensitivity = 120f;
    public Vector3 aimingCameraRotation;
    public Vector3 aimingCameraPosition;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void LateUpdate()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * mouseSensitivity * Time.deltaTime;
        player.Rotate(Vector3.up * mouseX);

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            transform.localPosition = transform.localPosition + aimingCameraPosition;
            transform.Rotate(aimingCameraRotation);
        }

        if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            transform.localPosition = transform.localPosition - aimingCameraPosition;
            transform.Rotate(-aimingCameraRotation);
        }
    }
}
