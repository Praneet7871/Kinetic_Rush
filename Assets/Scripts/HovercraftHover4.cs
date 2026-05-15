using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class StableHovercraft : MonoBehaviour
{
    [System.Serializable]
    public class HoverPoint
    {
        public Transform point;

        [HideInInspector]
        public float lastDistance;
    }

    public HoverPoint[] hoverPoints;

    public LayerMask groundMask;

    [Header("Hover")]
    public float hoverHeight = 2f;
    public float rayLength = 4f;
    public float hoverForce = 140f;
    public float hoverDamping = 25f;

    [Header("Movement")]
    public float moveForce = 400f;
    public float turnTorque = 150f;
    public float maxSpeed = 80f;

    [Header("Stability")]
    public float uprightStrength = 25f;
    public float uprightDamping = 19f;

    [Header("Air Physics")]
    public float extraGravity = 20f;

    private Rigidbody rb;

    private bool grounded;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        rb.centerOfMass = new Vector3(0f, -0.8f, 0f);

        rb.linearDamping = 0.3f;
        rb.angularDamping = 2f;

        rb.interpolation = RigidbodyInterpolation.Interpolate;

        rb.collisionDetectionMode =
            CollisionDetectionMode.Continuous;
    }

    void FixedUpdate()
    {
        HoverPhysics();

        if (grounded)
        {
            StabilizeRotation();
        }

        Movement();

        // EXTRA ARCADE GRAVITY
        rb.AddForce(
            Vector3.down * extraGravity,
            ForceMode.Acceleration
        );
    }

    void HoverPhysics()
    {
        grounded = false;

        foreach (HoverPoint hp in hoverPoints)
        {
            RaycastHit hit;

            Vector3 origin = hp.point.position;

            Vector3 direction = -transform.up;

            Debug.DrawRay(
                origin,
                direction * rayLength,
                Color.red
            );

            if (Physics.Raycast(
                origin,
                direction,
                out hit,
                rayLength,
                groundMask))
            {
                grounded = true;

                float compression =
                    hoverHeight - hit.distance;

                float velocity =
                    (hp.lastDistance - hit.distance)
                    / Time.fixedDeltaTime;

                float force =
                    (compression * hoverForce)
                    + (velocity * hoverDamping);

                force = Mathf.Max(0f, force);

                rb.AddForceAtPosition(
                    transform.up * force,
                    hp.point.position,
                    ForceMode.Force
                );

                hp.lastDistance = hit.distance;
            }
        }
    }

    void StabilizeRotation()
    {
        Quaternion desiredRotation =
            Quaternion.FromToRotation(
                transform.up,
                Vector3.up
            ) * rb.rotation;

        Quaternion rotationDifference =
            desiredRotation *
            Quaternion.Inverse(rb.rotation);

        rotationDifference.ToAngleAxis(
            out float angle,
            out Vector3 axis
        );

        if (angle > 180f)
        {
            angle -= 360f;
        }

        Vector3 torque =
            axis * (angle * uprightStrength)
            - rb.angularVelocity * uprightDamping;

        rb.AddTorque(torque);
    }

    void Movement()
    {
        float move =
            Input.GetAxis("Vertical");

        float turn =
            Input.GetAxis("Horizontal");

        Vector3 localVelocity =
            transform.InverseTransformDirection(
                rb.linearVelocity
            );

        // ACCELERATION
        rb.AddForce(
            transform.forward * move * moveForce,
            ForceMode.Acceleration
        );

        // TURNING
        rb.AddRelativeTorque(
            Vector3.up * turn * turnTorque,
            ForceMode.Acceleration
        );

        // DRIFT CONTROL
        localVelocity.x *= 0.92f;

        // STRONGER BRAKING
        if (move < 0)
        {
            localVelocity.z *= 0.82f;
        }

        rb.linearVelocity =
            transform.TransformDirection(localVelocity);

        // SPEED LIMIT
        rb.linearVelocity =
            Vector3.ClampMagnitude(
                rb.linearVelocity,
                maxSpeed
            );
    }
}