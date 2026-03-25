using UnityEngine;

public class EarthRotationController : MonoBehaviour
{
    public TimeController timeController;
    public float axialTiltDeg = 23.44f;
    [Tooltip("Adjust this until London faces the Sun at 12:00 UTC")]
    public float textureCalibration = 180f;

    private float lastRotationAngle = 0f;

    void Start()
    {
        // Tilt around Z so the lean is along the X-axis
        transform.localRotation = Quaternion.Euler(0, 0, axialTiltDeg);
        lastRotationAngle = GetCurrentTotalAngle();
    }

    void Update()
    {
        if (timeController == null) return;

        float currentAngle = GetCurrentTotalAngle();

        // Calculate the CHANGE (Delta)
        // Mathf.DeltaAngle handles the 359 -> 0 degree wrap-around correctly!
        float deltaAngle = Mathf.DeltaAngle(lastRotationAngle, currentAngle);

        // Apply rotation around the TILTED Y-axis
        transform.Rotate(Vector3.up, deltaAngle, Space.Self);

        // Store for next frame
        lastRotationAngle = currentAngle;
    }

    // Helper to keep Update() clean
    private float GetCurrentTotalAngle()
    {
        double d = timeController.daysSinceJ2000;
        double T = d / 36525.0;

        double gmst = 280.46061837 + 360.98564736629 * d + 0.000387933 * T * T - (T * T * T / 38710000.0);

        // Keep angle normalized to [0, 360) to avoid large-number drift.
        double normalizedGmst = (gmst % 360.0 + 360.0) % 360.0;
        double angle = normalizedGmst + textureCalibration;

        return (float)((angle % 360.0 + 360.0) % 360.0);
    }
}
