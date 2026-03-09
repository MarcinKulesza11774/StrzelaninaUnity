using UnityEngine;

public class JiggleLimit : MonoBehaviour
{
    public Transform anchor;

    [Header("Max local offset")]
    public Vector3 maxOffset = new Vector3(0.1f, 0.0f, 0.1f);

    [Header("Spring")]
    public float springStrength = 500f;
    public float damping = 15f;

    [Header("Motion influence")]
    public float movementInfluence = 0.06f;
    public float rotationInfluence = 2.0f;

    [Header("Velocity limits")]
    public float outwardVelocityLimit = 5f;
    public float inwardVelocityLimit = 10f;

    
    private float verticalBias = 0f;

    Vector3 restLocalPos;
    Vector3 velocity;

    Vector3 lastAnchorWorldPos;
    Quaternion lastAnchorRot;

    void Start()
    {
        restLocalPos = anchor.InverseTransformPoint(transform.position);
        restLocalPos.y -= maxOffset.y * verticalBias;

        lastAnchorWorldPos = anchor.position;
        lastAnchorRot = anchor.rotation;
    }

    void LateUpdate()
    {
        float dt = Time.deltaTime;
        if (dt <= 0f) return;

        Vector3 worldDelta = anchor.position - lastAnchorWorldPos;
        worldDelta.y = 0f;
        lastAnchorWorldPos = anchor.position;

        velocity -= worldDelta * movementInfluence;

        Quaternion deltaRot = anchor.rotation * Quaternion.Inverse(lastAnchorRot);
        lastAnchorRot = anchor.rotation;

        deltaRot.ToAngleAxis(out float angle, out Vector3 axis);
        if (angle > 180f) angle -= 360f;

        Vector3 rotationalVelocity = axis * angle * Mathf.Deg2Rad;
        Vector3 toBone = transform.position - anchor.position;
        velocity -= Vector3.Cross(rotationalVelocity, toBone) * rotationInfluence * dt;

        Vector3 targetWorldPos = anchor.TransformPoint(restLocalPos);
        Vector3 displacement = transform.position - targetWorldPos;

        if (displacement.magnitude > 0.001f)
        {
            float velComponentAway = Vector3.Dot(velocity, displacement.normalized);
            float limit = velComponentAway > 0f ? outwardVelocityLimit : inwardVelocityLimit;
            velocity = Vector3.ClampMagnitude(velocity, limit);
        }

        Vector3 displacement2 = transform.position - targetWorldPos;
        velocity -= displacement2 * springStrength * dt;

        velocity *= Mathf.Exp(-damping * dt);

        transform.position += velocity * dt;

        Vector3 localOffset = anchor.InverseTransformPoint(transform.position) - restLocalPos;

        localOffset.x = Mathf.Clamp(localOffset.x, -maxOffset.x, maxOffset.x);
        localOffset.y = Mathf.Clamp(localOffset.y, -maxOffset.y, 0f);
        localOffset.z = Mathf.Clamp(localOffset.z, -maxOffset.z, maxOffset.z);

        transform.position = anchor.TransformPoint(restLocalPos + localOffset);
    }
}
