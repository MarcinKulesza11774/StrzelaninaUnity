using UnityEngine;

public class CameraControll : MonoBehaviour
{
    [Header("Referencje")]
    public Transform player;
    public PlayerMovement playerMovement;

    [Header("Mysz")]
    public float mouseSensitivity = 120f;

    [Header("Celowanie – pozycja (over-the-shoulder)")]
    [Tooltip("Offset pozycji kamery podczas celowania w lokalnym układzie gracza.\n" +
             "Przykład: (0.3, 0, 0.5) = w prawo i do przodu")]
    public Vector3 aimPositionOffset = new Vector3(0.3f, 0f, 0.5f);

    [Header("Celowanie – rotacja")]
    [Tooltip("Offset rotacji kamery podczas celowania (Euler, lokalny).\n" +
             "Przykład: (5, 0, 0) = lekko w dół")]
    public Vector3 aimRotationOffset = new Vector3(5f, 0f, 0f);

    [Header("Płynność przejścia")]
    [Range(1f, 30f)]
    public float aimSpeed = 8f;

    private Vector3 defaultLocalPosition;
    private Quaternion defaultLocalRotation;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        defaultLocalPosition = transform.localPosition;
        defaultLocalRotation = transform.localRotation;
    }

    void LateUpdate()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * mouseSensitivity * Time.deltaTime;
        player.Rotate(Vector3.up * mouseX);

        bool isAiming = playerMovement != null && playerMovement.IsAiming;

        // przybliżenie przy celowaniu
        Vector3 targetPos = defaultLocalPosition +
                               (isAiming ? aimPositionOffset : Vector3.zero);
        Quaternion targetRot = defaultLocalRotation *
                               (isAiming ? Quaternion.Euler(aimRotationOffset) : Quaternion.identity);

        // Płynne przejście
        float t = aimSpeed * Time.deltaTime;
        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPos, t);
        transform.localRotation = Quaternion.Lerp(transform.localRotation, targetRot, t);
    }
}