using System;

namespace Kuci.Core.Extensions
{
    public static class TimeSpanExtensions
    {
        public static string ToSecondAndMillisecond(this TimeSpan timeSpan)
        {
            return $"{timeSpan.TotalSeconds:0}.{timeSpan.Milliseconds / 10:00}";
        }
    }
}