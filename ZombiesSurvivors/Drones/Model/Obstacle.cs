using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZombieSurvivor.Model
{
    public class Obstacle
    {
        public float _x;
        public float _y;
        public int Width = 32;
        public int Height = 32;
        public int _health = 20;

        public bool IsDestroyed => _health <= 0;

        public Obstacle(float x, float y)
        {
            _x = x;
            _y = y;
        }

        public void TakeDamage(int damage)
        {
            _health -= damage;
        }
    }

    public class HealthPickup
    {
        public float _x;
        public float _y;
        public int _size = 10;
        public int _amount = 20; 

        public HealthPickup(float x, float y)
        {
            _x = x;
            _y = y;
        }
    }
}
