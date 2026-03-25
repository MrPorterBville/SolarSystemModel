using UnityEngine;
using System;

/// <summary>
/// TimeController handles all time-related calculations needed for
/// astronomical calculations, such as Julian Day and days since J2000.
/// This is independent of observer position, which ensures a physically
/// accurate model of celestial positions.
/// </summary>
public class TimeController : MonoBehaviour
{
    [Header("Date & Time")]
    public int year = 2026;   // Year (e.g., 2026)
    public int month = 3;     // Month (1-12)
    public int day = 23;      // Day of the month
    public int hour = 12;     // Hour of day (0-23)
    public int minute = 0;    // Minute of hour (0-59)

    [HideInInspector] public double julianDay;        // Full Julian Day number
    [HideInInspector] public double daysSinceJ2000;   // Days since standard epoch Jan 1, 2000 12:00 UT
    [HideInInspector] public double UT;               // Universal Time in decimal hours

    void Update()
    {
        // Calculate Julian Day each frame (can optimize later to update only when time changes)
        CalculateJulianDay();
    }

    /// <summary>
    /// Calculates Julian Day, days since J2000, and decimal UT hours.
    /// </summary>
    public void CalculateJulianDay()
    {
        // Convert hour and minute into decimal hours (e.g., 14:30 -> 14.5)
        UT = hour + minute / 60.0;

        int Y = year;
        int M = month;
        int D = day;

        // If month is Jan or Feb, treat it as month 13/14 of previous year
        // This is part of the Julian Day calculation algorithm
        if (M <= 2)
        {
            Y -= 1;
            M += 12;
        }

        // Gregorian calendar correction
        // A = year / 100, B = 2 - A + floor(A / 4)
        // Adjusts for leap years and century years
        double A = Math.Floor(Y / 100.00);
        double B = 2 - A + Math.Floor(A / 4.0);

        // Julian Day calculation
        // Floor(365.25*(Y + 4716)) -> counts days in years including leap years
        // Floor(30.6001*(M + 1)) -> counts days in months
        // Add day and Gregorian correction B
        // Subtract 1524.5 to align with astronomical epoch
        // Add fraction of day from UT
        julianDay = Mathf.Floor(365.25f * (float)(Y + 4716)) +
                    Mathf.Floor(30.6001f * (float)(M + 1)) +
                    D + B - 1524.5 + UT / 24.0;

        // Days since J2000 (Jan 1, 2000, 12:00 UT)
        // This is the reference epoch commonly used in astronomy
        daysSinceJ2000 = julianDay - 2451545.0;
        Debug.Log($"Days since J2000: {daysSinceJ2000}");

        // Now julianDay, daysSinceJ2000, and UT are available to other scripts
        // like CelestialBodyController to compute Sun/Moon positions
    }
}