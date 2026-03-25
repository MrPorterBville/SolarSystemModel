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

        // Use low-order solar system terms referenced to J2000.
        // This keeps the orbital phase tied to the equinox/perihelion geometry,
        // instead of relying on a hand-tuned phase constant.
        double T = days / 36525.0;

        // Mean anomaly of Earth's orbit (degrees)
        double meanAnomalyDeg = 357.52911 + 35999.05029 * T - 0.0001537 * T * T;
        float meanAnomalyRad = Mathf.Deg2Rad * (float)meanAnomalyDeg;

        // Equation of center (degrees)
        double equationOfCenterDeg =
            (1.914602 - 0.004817 * T - 0.000014 * T * T) * Math.Sin(meanAnomalyRad) +
            (0.019993 - 0.000101 * T) * Math.Sin(2.0 * meanAnomalyRad) +
            0.000289 * Math.Sin(3.0 * meanAnomalyRad);

        // Earth heliocentric true longitude (degrees), referenced to J2000 equinox.
        // 102.9372 deg is Earth's longitude of perihelion.
        double earthLongitudeDeg = meanAnomalyDeg + equationOfCenterDeg + 102.9372;
        float earthLongitudeRad = Mathf.Deg2Rad * (float)earthLongitudeDeg;

        // Earth-Sun distance in AU with low-order eccentricity terms.
        double radiusAu = 1.00014
                        - 0.01671 * Math.Cos(meanAnomalyRad)
                        - 0.00014 * Math.Cos(2.0 * meanAnomalyRad);

        float radiusKm = semiMajorAxisKm * (float)radiusAu;
        float x = radiusKm * Mathf.Cos(earthLongitudeRad);
        float z = radiusKm * Mathf.Sin(earthLongitudeRad);

        earthPosition = new Vector3(x, 0, z) * scale;
        transform.position = earthPosition;
    }


    void UpdateMoonOrbit()
    {
        if (moonSphere == null) return;

        double days = timeController.daysSinceJ2000;

        // Low-order lunar ephemeris terms (J2000-based).
        // Includes:
        // - elliptical radius variation (perigee/apogee),
        // - ecliptic longitude perturbation,
        // - ecliptic latitude variation.
        double meanLongitudeDeg = 218.316 + 13.176396 * days; // L'
        double meanAnomalyDeg = 134.963 + 13.064993 * days;   // M'
        double argumentOfLatitudeDeg = 93.272 + 13.229350 * days; // F

        float meanAnomalyRad = Mathf.Deg2Rad * (float)meanAnomalyDeg;

        float eclipticLongitudeRad = Mathf.Deg2Rad * (float)(meanLongitudeDeg + 6.289 * Math.Sin(meanAnomalyRad));
        float eclipticLatitudeRad = Mathf.Deg2Rad * (float)(5.128 * Math.Sin(Mathf.Deg2Rad * (float)argumentOfLatitudeDeg));
        float radiusKm = (float)(385001.0 - 20905.0 * Math.Cos(meanAnomalyRad));

        // Ecliptic-frame Cartesian position relative to Earth (km)
        float cosLat = Mathf.Cos(eclipticLatitudeRad);
        float x = radiusKm * cosLat * Mathf.Cos(eclipticLongitudeRad);
        float y = radiusKm * Mathf.Sin(eclipticLatitudeRad);
        float z = radiusKm * cosLat * Mathf.Sin(eclipticLongitudeRad);

        Vector3 moonRel = new Vector3(x, y, z) * scale;

        moonSphere.transform.localPosition = moonRel;

        // Absolute world position
        moonPosition = transform.position + moonRel;
    }
}
