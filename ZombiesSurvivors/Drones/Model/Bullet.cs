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
        public float x { get; set; }
        public float y { get; set; }
        public float velX { get; set; }
        public float velY { get; set; }
        public float damage { get; set; }

        public Color color { get; set; }


        public int radius { get; set; } = 4;

        public Bullet(float x, float y, float velX, float velY, float damage, Color? color = null)
        {
            this.x = x;
            this.y = y;
            this.velX = velX;
            this.velY = velY;
            this.damage = damage;
            this.color = color ?? Color.DeepSkyBlue;
        }

        public void Update()
        {
            x += velX;
            y += velY;
        }

        public float width => radius * 2;
        public float height => radius * 2;
    }
}
