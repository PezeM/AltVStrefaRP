﻿using AltV.Net;
using AltV.Net.Data;
using AltVStrefaRPServer.Modules.Core;

namespace AltVStrefaRPServer.Models.Core
{
    public class BlipWrapper : IBlipWrapper
    {
        private string _name;
        private int _sprite;
        private int _color;
        private float _scale;
        private Position _position;

        public int Id { get; set; }

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                BlipManager.Instance.UpdateBlipName(this, value);
            }
        }

        public int Sprite
        {
            get { return _sprite; }
            set
            {
                _sprite = value;
                BlipManager.Instance.UpdateBlipSprite(this, value);
            }
        }

        public int Color
        {
            get { return _color; }
            set
            {
                _color = value;
                BlipManager.Instance.UpdateBlipColor(this, value);
            }
        }

        public float Scale
        {
            get { return _scale; }
            set
            {
                _scale = value;
                BlipManager.Instance.UpdateBlipScale(this, value);
            }
        }

        public Position Position
        {
            get { return _position; }
            set
            {
                _position = value;
                BlipManager.Instance.UpdateBlipPosition(this, value);
            }
        }

        public BlipWrapper(int id, string name, int sprite, int color, Position position, float scale = 1) : this(name, sprite, color, position, scale)
        {
            Id = id;
        }

        public BlipWrapper(string name, int sprite, int color, Position position, float scale = 1)
        {
            Name = name;
            Sprite = sprite;
            Color = color;
            Scale = scale;
            Position = position;
        }

        public void DestroyBlip()
        {
            BlipManager.Instance.Remove(this);
        }

        public IMValueBaseAdapter GetAdapter()
        {
            return BlipManager.Instance.BlipAdapter;
        }
    }
}
