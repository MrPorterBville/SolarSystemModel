using UnityEngine;

public class EarthRotationController : MonoBehaviour
{
    public TimeController timeController;
    public float axialTiltDeg = 23.44f;
    [Tooltip("Offset so mesh longitude 0 aligns with astronomical 0° (GMST reference). Set for your mesh seam/orientation.")]
    public float textureCalibration = 90f;

    private float lastRotationAngle = 0f;

    void Start()
    {
        // Tilt around X so the spin axis leans along +Z.
        // This matches Earth's seasonal orientation in this project's orbital frame:
        // - March/September equinox: axis ~perpendicular to Sun direction
        // - June solstice: north pole tilted toward the Sun
        transform.localRotation = Quaternion.Euler(axialTiltDeg, 0, 0);
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
