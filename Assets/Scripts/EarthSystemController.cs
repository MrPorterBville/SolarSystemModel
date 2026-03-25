using UnityEngine;
using System;

public class EarthSystemController : MonoBehaviour
{
    public TimeController timeController;

    [Header("Earth Parameters")]
    public float earthRadiusKm = 6371f;
    public float semiMajorAxisKm = 149_600_000f; // Earth orbit radius
    public float orbitalPeriodDays = 365.25f;

    [Header("Moon Parameters")]
    public GameObject moonSphere;
    public float moonDistanceKm = 384_400f;
    public float moonOrbitPeriodDays = 27.321f;

    [Header("Scaling")]
    public float scale = 1f / 6371f; // Earth = 1 Unity unit

    [HideInInspector] public Vector3 earthPosition;
    [HideInInspector] public Vector3 moonPosition;

    public GameObject earthSphere;

    void Update()
    {
        UpdateEarthOrbit();
        UpdateMoonOrbit();
    }

    void UpdateEarthOrbit()
    {
        double days = timeController.daysSinceJ2000;

        // 365.25636 is the precise Sidereal Year in days
        // 1.7202 rad (approx 98.6 degrees) aligns the orbit so 
        // June 21st matches your tilt direction.
        float orbitalAngle = (float)(days * (2 * Mathf.PI / 365.25636) + 1.7202f);

        float x = semiMajorAxisKm * Mathf.Cos(orbitalAngle);
        float z = semiMajorAxisKm * Mathf.Sin(orbitalAngle);

        earthPosition = new Vector3(x, 0, z) * scale;
        transform.position = earthPosition;
    }


    void UpdateMoonOrbit()
    {
        if (moonSphere == null) return;

        double days = timeController.daysSinceJ2000;
        float moonAngle = (float)((days % moonOrbitPeriodDays) / moonOrbitPeriodDays * 2 * Mathf.PI);

        // Moon position relative to EarthSystemController (local)
        Vector3 moonRel = new Vector3(
            moonDistanceKm * Mathf.Cos(moonAngle),
            0,
            moonDistanceKm * Mathf.Sin(moonAngle)
        ) * scale;

        moonSphere.transform.localPosition = moonRel;

        // Absolute world position
        moonPosition = transform.position + moonRel;
    }
}

