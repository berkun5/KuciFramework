using System;

namespace Kuci.Core.Extensions
{
    public static class DateTimeExtensions
    {
        private static readonly DateTime PivotDatetime = new(1970, 1, 1, 0, 0, 0);
        
        public static double ToUnixTimestamp(this DateTime d)
        {
            var epoch = d - PivotDatetime;
            return epoch.TotalSeconds;
        }
    }
}