using System;

namespace Kuci.Core.Extensions
{
    public static class DoubleExtensions
    {
        // Converts degrees to radians
        public static double DegreesToRadians(this double degrees)
        {
            return degrees * Math.PI / 180.0;
        }

        // Adjusts radians to be within [-?, ?]
        public static double NormalizeRadians(this double radians)
        {
            while (radians > Math.PI)
                radians -= 2 * Math.PI;
            while (radians < -Math.PI)
                radians += 2 * Math.PI;
            return radians;
        }

        // Converts and normalizes an angle from degrees to radians in the range [-?, ?]
        public static double ConvertDegreesToNormalizedRadians(this double degrees)
        {
            double radians = DegreesToRadians(degrees);
            return NormalizeRadians(radians);
        }
        
        // Gets the percentage of a double value
        public static double GetPercentage(this double value, int percentage)
        {
            return (value / 100) * percentage;
        }
    }
}
