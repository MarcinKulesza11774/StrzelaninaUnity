using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    public float jumpForce = 5f;
    public Transform cameraTransform;

    private Rigidbody rb;
    private Vector3 moveInput;

    public Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        Animator animator;
    }

    void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;

        forward.y = 0;
        right.y = 0;

        forward.Normalize();
        right.Normalize();

        moveInput = (forward * v + right * h).normalized;

        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        //}

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            speed *= 2;
            animator.speed *= 2;
        }

        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            speed /= 2;
            animator.speed /= 2;
        }


        bool czyIdzie = Mathf.Abs(h) > 0.01f || Mathf.Abs(v) > 0.01f;

        animator.SetBool("czyIdzie", czyIdzie);
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + moveInput * speed * Time.fixedDeltaTime);
    }

    public bool IsMoving()
    {
        return moveInput.sqrMagnitude > 0.01f;
    }

    public Vector3 MoveDirection()
    {
        return moveInput;
    }
}
