using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZombieSurvivor.Model
{
    public class Bullet
    {
        public float _x { get; set; }
        public float _y { get; set; }
        public float _velX { get; set; }
        public float _velY { get; set; }
        public float _damage { get; set; }

        
        public int Radius { get; set; } = 4;

        public Bullet(float x, float y, float velX, float velY, float damage)
        {
            _x = x;
            _y = y;
            _velX = velX;
            _velY = velY;
            _damage = damage;
        }

        public void Update()
        {
            _x += _velX;
            _y += _velY;
        }

        public float _width => Radius * 2;
        public float _height => Radius * 2;
    }
}
