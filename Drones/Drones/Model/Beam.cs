using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZombieSurvivor.Model
{
    public class Beam
    {
        public float _startOffsetX, _startOffsetY; 
        public float _startX, _startY;
        public float _endX, _endY;
        public Color _color;
        public float _damage;
        public float _lifetime;

        public Beam(float startOffsetX, float startOffsetY, float endX, float endY, Color color, float damage, float lifetime)
        {
            _startOffsetX = startOffsetX;
            _startOffsetY = startOffsetY;
            _endX = endX;
            _endY = endY;
            _color = color;
            _damage = damage;
            _lifetime = lifetime;
        }

        public void UpdatePosition(float playerX, float playerY)
        {
            _startX = playerX + _startOffsetX;
            _startY = playerY + _startOffsetY;
        }
    }
}
