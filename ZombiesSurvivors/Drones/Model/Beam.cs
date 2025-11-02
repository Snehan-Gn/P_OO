using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZombieSurvivor.Model
{
    public class Beam
    {
        public float startOffsetX, startOffsetY; 
        public float startX, startY;
        public float endX, endY;
        public Color color;
        public float damage;
        public float lifetime;

        public Beam(float startOffsetX, float startOffsetY, float endX, float endY, Color color, float damage, float lifetime)
        {
            this.startOffsetX = startOffsetX;
            this.startOffsetY = startOffsetY;
            this.endX = endX;
            this.endY = endY;
            this.color = color;
            this.damage = damage;
            this.lifetime = lifetime;
        }

        public void UpdatePosition(float playerX, float playerY)
        {
            startX = playerX + startOffsetX;
            startY = playerY + startOffsetY;
        }
    }
}
