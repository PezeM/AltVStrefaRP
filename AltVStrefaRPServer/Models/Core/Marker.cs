using System.Drawing;
using AltV.Net.Data;
using AltVStrefaRPServer.Models.Interfaces;

namespace AltVStrefaRPServer.Models.Core
{
    public class Marker : IPosition
    {
        /// <summary>
        /// Marker type. https://wiki.rage.mp/index.php?title=Marker::Marker
        /// </summary>
        public int Type { get; set; }
        /// <summary>
        /// Range of the marker
        /// </summary>
        public int Range { get; set; }
        /// <summary>
        /// Dimension of the marker. 0 for every dimension
        /// </summary>
        public int Dimension { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public float ScaleX { get; set; }
        public float ScaleY { get; set; }
        public float ScaleZ { get; set; }
        public int Red { get; set; }
        public int Green { get; set; }
        public int Blue { get; set; }
        public int Alpha { get; set; }

        public Marker(int type, Position position, Color color, Position scale, int range = 20, int dimension = 0)
        {
            Type = type;
            X = position.X;
            Y = position.Y;
            Z = position.Z;
            Red = color.R;
            Green = color.G;
            Blue = color.B;
            Alpha = color.A;
            ScaleX = scale.X;
            ScaleY = scale.Y;
            ScaleZ = scale.Z;
            Range = range;
            Dimension = dimension;
        }

        public Position GetPosition()
        {
            return new Position(X,Y,Z);
        }
    }
}
