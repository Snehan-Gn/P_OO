using System;
using System.Collections.Generic;
using System.Drawing;
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

        public Color _color { get; set; }


        public int _radius { get; set; } = 4;

        public Bullet(float x, float y, float velX, float velY, float damage, Color? color = null)
        {
            _x = x;
            _y = y;
            _velX = velX;
            _velY = velY;
            _damage = damage;
            _color = color ?? Color.DeepSkyBlue;
        }

        public void Update()
        {
            _x += _velX;
            _y += _velY;
        }

        public float _width => _radius * 2;
        public float _height => _radius * 2;
    }
}
