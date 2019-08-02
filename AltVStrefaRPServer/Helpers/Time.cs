using System;

namespace AltVStrefaRPServer.Helpers
{
    public static class Time
    {
        /// <summary>
        /// Get current unix date in seconds
        /// </summary>
        /// <returns></returns>
        public static double GetTimestamp() => DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        /// <summary>
        /// Get current unix date in ms
        /// </summary>
        /// <returns></returns>
        public static double GetTimestampMs() => DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        public static double GetElapsedTime(double startTime) => DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - startTime;
    }
}
