using System.Drawing;
using AltV.Net.Data;
using AltV.Net.NetworkingEntity.Elements.Entities;
using AltVStrefaRPServer.Modules.Core;

namespace AltVStrefaRPServer.Models.Core
{
    public class Marker : IMarker
    {
        private int _red;
        private int _green;
        private int _blue;
        private int _alpha;
        private float _scaleX;
        private float _scaleY;
        private float _scaleZ;
        public int Id { get; set; }
        public INetworkingEntity NetworkingEntity { get; set; }
        public int Type { get; set; }
        public int Range { get; set; }
        public int Dimension { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public float ScaleX
        {
            get => _scaleX;
            set
            {
                _scaleX = value;
                UpdateNetworkingEntity("scaleX", _scaleX);
            }
        }

        public float ScaleY
        {
            get => _scaleY;
            set
            {
                _scaleY = value;
                UpdateNetworkingEntity("scaleY", _scaleY);
            }
        }

        public float ScaleZ
        {
            get => _scaleZ;
            set
            {
                _scaleZ = value;
                UpdateNetworkingEntity("scaleZ", _scaleZ);
            }
        }

        public int Red
        {
            get => _red;
            set
            {
                _red = value;
                UpdateNetworkingEntity("red", _red);
            }
        }

        public int Green
        {
            get => _green;
            set
            {
                _green = value;
                UpdateNetworkingEntity("green", _green);
            } 
        }

        public int Blue
        {
            get => _blue;
            set
            {
                _blue = value;
                UpdateNetworkingEntity("blue", _blue);
            }
        }

        public int Alpha
        {
            get => _alpha;
            set
            {
                _alpha = value;
                UpdateNetworkingEntity("alpha", _alpha);
            }
        }

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
        public bool DestroyMarker()
        {
            return MarkerManager.Instance.RemoveMarker(this);
        }
        private void UpdateNetworkingEntity(string dataName, object newValue)
        {
            if (NetworkingEntity == null) return;
            if (NetworkingEntity.GetData(dataName, out long _))
            {
                NetworkingEntity.SetData(dataName, newValue);
            }
        }
    }
}
