using UnityEngine;
using Unity.Cinemachine;

public class DynamicFOV : MonoBehaviour
{
    public Rigidbody targetRb;

    [Header("FOV")]
    public float normalFOV = 70f;
    public float fastFOV = 95f;

    [Header("Speed")]
    public float maxSpeed = 80f;

    [Header("Smoothing")]
    public float smoothSpeed = 5f;

    [Header("Camera Height")]
    public float normalY = 3f;
    public float fastY = 1.8f;

    private CinemachineCamera cineCam;

    private CinemachineThirdPersonFollow follow;

    void Start()
    {
        cineCam = GetComponent<CinemachineCamera>();

        follow =
            GetComponent<CinemachineThirdPersonFollow>();
    }

    void Update()
    {
        float speed =
            targetRb.linearVelocity.magnitude;

        float t =
            Mathf.Clamp01(speed / maxSpeed);

        // DYNAMIC FOV
        float targetFOV =
            Mathf.Lerp(
                normalFOV,
                fastFOV,
                t
            );

        cineCam.Lens.FieldOfView =
            Mathf.Lerp(
                cineCam.Lens.FieldOfView,
                targetFOV,
                smoothSpeed * Time.deltaTime
            );

        // DYNAMIC CAMERA HEIGHT
        if (follow != null)
        {
            Vector3 offset = follow.ShoulderOffset;

            offset.y =
                Mathf.Lerp(
                    normalY,
                    fastY,
                    t
                );

            follow.ShoulderOffset = Vector3.Lerp(
                follow.ShoulderOffset,
                offset,
                smoothSpeed * Time.deltaTime
            );
        }
    }
}