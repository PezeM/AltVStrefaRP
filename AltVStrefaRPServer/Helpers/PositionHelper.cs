using System;
using AltV.Net.Data;

namespace AltVStrefaRPServer.Helpers
{
    public static class PositionHelper
    {
        /// <summary>
        /// Calculates position in front of
        /// </summary>
        /// <param name="position"></param>
        /// <param name="heading"></param>
        /// <param name="distance"></param>
        /// <returns>Returns <see cref="Vector3"/> with position in front</returns>
        public static Position GetPositionInFrontOf(Position position, float heading, float distance)
        {
            float x = (float)(distance * Math.Sin(-(heading * Math.PI / 180)));
            float y = (float)(distance * Math.Cos(-(heading * Math.PI / 180)));
            return new Position(position.X + x, position.Y + y, position.Z);
        }
    }
}
