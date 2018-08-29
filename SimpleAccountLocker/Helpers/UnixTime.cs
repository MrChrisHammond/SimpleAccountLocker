using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleAccountLocker.Helpers
{
    /// <summary>
    /// A class with methods to convert time to and from a unix timestamp.
    /// </summary>
    public static class UnixTime
    {
        /// <summary>
        /// Converts a given unix timestamp to DateTime.
        /// </summary>
        /// <param name="unixTimeStamp">Date and time represented as a unix timestamp.</param>
        /// <returns>DateTime converted from unix timestamp.</returns>
        public static DateTime UnixTimestampToDateTime(double unixTimeStamp)
        {
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }
        /// <summary>
        /// Converts a given DateTime to unix timestamp.
        /// </summary>
        /// <param name="dateTime">A date and time as DateTime.</param>
        /// <returns>unix timestamp as a double, converted from DateTime.</returns>
        public static double DateTimeToUnixTimestamp(DateTime dateTime)
        {
            return (TimeZoneInfo.ConvertTimeToUtc(dateTime) -
                   new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc)).TotalSeconds;
        }
    }
}
