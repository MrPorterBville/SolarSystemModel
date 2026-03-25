using UnityEngine;
using System;

public class CelestialBodyController : MonoBehaviour
{
    public TimeController timeController;   // Reference to global time
    //public EarthController earthController; // Reference to moving Earth

    [Header("Scaling")]
    public float scale = 1f / 6371f; // Converts km -> Unity units

    //[Header("Moon Parameters")]
    //public float moonDistanceKm = 384_400f; // Average km from Earth
    //public float moonOrbitPeriodDays = 27.321f; // Lunar orbit period
    //public GameObject moonSphere; // Moon mesh, must NOT be a child of EarthController

    [Header("Sun Parameters")]
    public GameObject sunSphere;  // Sun mesh, must NOT be a child of any moving object
    public Light sunLight;

    [Header("Earth System Parameters")]
    public GameObject earthController;

    //[HideInInspector] public Vector3 moonPosition; // Absolute world position
    [HideInInspector] public Vector3 sunPosition;  // Absolute world position


    void Update()
    {
        // Get Earths Location in space 
        Vector3 earthPos = earthController.transform.position; // Earth’s center in world space
        Vector3 sunToEarth = (earthPos - Vector3.zero).normalized; // Direction from Sun to Earth

        // Set Sunlight Rotation to equal earths location 
        if (sunLight != null)
        {
            sunLight.transform.rotation = Quaternion.LookRotation(sunToEarth);
        }

        //if (timeController == null || earthController == null) return;

        // -------------------------------
        // Sun - absolute position
        // Technically this is uneeded
        // -------------------------------
        sunPosition = Vector3.zero; // Origin in world space
        if (sunSphere != null)
        {
            sunSphere.transform.position = sunPosition * scale; // Apply Unity scale
        }

    }
}