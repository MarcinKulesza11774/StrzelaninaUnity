using UnityEngine;

public class CameraControll : MonoBehaviour
{
    public Transform player;
    public float mouseSensitivity = 120f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void LateUpdate()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * mouseSensitivity * Time.deltaTime;
        player.Rotate(Vector3.up * mouseX);
    }
}
