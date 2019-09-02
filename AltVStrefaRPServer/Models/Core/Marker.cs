using System.Drawing;
using AltV.Net.Data;

namespace AltVStrefaRPServer.Models.Core
{
    public class Marker : IMarker
    {
        public int Id { get; set; }
        public int Type { get; set; }
        public int Range { get; set; }
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

        public Marker(int id, int type, Position position, Color color, Position scale, int range = 20, int dimension = 0)
        {
            Id = id;
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
