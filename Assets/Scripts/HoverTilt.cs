using UnityEngine;

public class HoverTilt : MonoBehaviour
{
    public float tiltAmount = 30f;
    public float tiltSpeed = 8f;

    public float pitchAmount = 10f;

    private Quaternion initialRotation;

    void Start()
    {
        initialRotation = transform.localRotation;
    }

    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        float roll = -horizontal * tiltAmount;

        float pitch = vertical * pitchAmount;

        Quaternion targetRotation =
            initialRotation *
            Quaternion.Euler(pitch, 0f, roll);

        transform.localRotation =
            Quaternion.Lerp(
                transform.localRotation,
                targetRotation,
                tiltSpeed * Time.deltaTime
            );
    }
}