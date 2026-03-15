using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Ruch")]
    public float speed = 5f;
    public float sprintMultiplier = 2f;

    [Header("Referencje")]
    public Transform cameraTransform;
    public Animator animator;

    private CharacterController cc;
    private Vector3 moveInput;
    private bool isSprinting;

    // Dostêp dla kamery – czy gracz celuje
    public bool IsAiming { get; private set; }
    public bool Bang { get; private set; }

    void Start()
    {
        cc = GetComponent<CharacterController>();
    }

    void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;
        forward.y = 0; forward.Normalize();
        right.y = 0; right.Normalize();

        moveInput = (forward * v + right * h).normalized;
        bool czyIdzie = moveInput.sqrMagnitude > 0.01f;

        // Celowanie – zablokowane podczas ruchu
        if (Input.GetKeyDown(KeyCode.Mouse1) && !czyIdzie)
            IsAiming = true;
        if (Input.GetKeyUp(KeyCode.Mouse1) || czyIdzie)
            IsAiming = false;
        if (Input.GetKeyUp(KeyCode.Mouse0) && IsAiming)
        {
            animator.SetTrigger("Bang");
        }

        animator.SetBool("czyCeluje", IsAiming);
        animator.SetBool("czyIdzie", czyIdzie);

        // Sprint – niedostêpny podczas celowania
        if (Input.GetKeyDown(KeyCode.LeftShift) && !isSprinting && !IsAiming)
        {
            isSprinting = true;
            speed *= sprintMultiplier;
            animator.speed *= sprintMultiplier;
        }
        if (Input.GetKeyUp(KeyCode.LeftShift) && isSprinting)
        {
            isSprinting = false;
            speed /= sprintMultiplier;
            animator.speed /= sprintMultiplier;
        }

        // Ruch + grawitacja
        Vector3 move = moveInput * speed + Vector3.down * 9.81f;
        cc.Move(move * Time.deltaTime);
    }

    public bool IsMoving() => moveInput.sqrMagnitude > 0.01f;
    public Vector3 MoveDirection() => moveInput;
}