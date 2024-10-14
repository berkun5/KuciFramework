namespace Kuci.Core.Extensions
{
    public static class FloatExtensions
    {
        public static float NormalizeAngle(this float value)
        {
            if (value > 180f)
            {
                value -= 360f;
            }
            return value;
        }
    }
}