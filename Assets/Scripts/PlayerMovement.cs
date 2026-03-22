using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Ruch")]
    public float speed = 5f;
    public float sprintMultiplier = 2f;

    [Header("Referencje")]
    public Transform cameraTransform;
    public Animator animator;
    public GameObject crosshair;

    private CharacterController cc;
    private Vector3 moveInput;
    private bool isMoving;
    private bool isAiming;
    private bool isSprinting;
    public bool IsSprinting => isSprinting;

    public bool IsAiming => isAiming;

    void Start()
    {
        cc = GetComponent<CharacterController>();
    }

    void Update()
    {
        HandleMovement();
        HandleAiming();
        HandleSprint();
        HandleAnimator();

        cc.Move((moveInput * speed + Vector3.down * 9.81f) * Time.deltaTime);
    }

    void HandleMovement()
    {
        if (isAiming)
        {
            moveInput = Vector3.zero;
            isMoving = false;
            return;
        }

        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;
        forward.y = 0; forward.Normalize();
        right.y = 0; right.Normalize();

        Vector3 dir = Vector3.zero;
        if (Input.GetKey(KeyCode.W)) dir += forward;
        if (Input.GetKey(KeyCode.S)) dir -= forward;
        if (Input.GetKey(KeyCode.D)) dir += right;
        if (Input.GetKey(KeyCode.A)) dir -= right;

        moveInput = dir.normalized;
        isMoving = moveInput.sqrMagnitude > 0.01f;
    }

    void HandleAiming()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1)) isAiming = true;
        if (Input.GetKeyUp(KeyCode.Mouse1)) isAiming = false;
        crosshair.SetActive(isAiming);

        if (Input.GetKeyUp(KeyCode.Mouse0) && isAiming)
            animator.SetTrigger("Bang");
    }

    void HandleSprint()
    {
        if (isAiming) return;

        if (Input.GetKeyDown(KeyCode.LeftShift) && !isSprinting)
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
    }

    void HandleAnimator()
    {
        animator.SetBool("czyCeluje", isAiming);
        animator.SetBool("czyIdzie", isMoving);
    }

    public bool IsMoving() => isMoving;
    public Vector3 MoveDirection() => moveInput;
}