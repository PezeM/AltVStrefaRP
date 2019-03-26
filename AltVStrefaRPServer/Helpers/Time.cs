using System;

namespace AltVStrefaRPServer.Helpers
{
    public class Time
    {
        /// <summary>
        /// Get current unix date in seconds
        /// </summary>
        /// <returns></returns>
        public static double GetTimestamp()
        {
            return DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        }

        /// <summary>
        /// Get current unix date in ms
        /// </summary>
        /// <returns></returns>
        public static double GetTimestampMs()
        {
            return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }
    }
}
