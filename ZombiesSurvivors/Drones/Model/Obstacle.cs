using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZombieSurvivor.Model
{
    public class Obstacle
    {
        public float x;
        public float y;
        public int Width = 32;
        public int Height = 32;
        public int health = 80;

        public bool IsDestroyed => health <= 0;

        public Obstacle(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public void TakeDamage(int damage)
        {
            health -= damage;
        }
    }

    public class HealthPickup
    {
        public float x;
        public float y;
        public int size = 10;
        public int amount = 20; 

        public HealthPickup(float x, float y)
        {
            this.x = x;
            this.y = y;
        }
    }
}
