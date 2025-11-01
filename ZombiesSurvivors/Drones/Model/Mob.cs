using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;

namespace ZombieSurvivor.Model
{
    public partial class Mob
    {
        public float _x {  get; set; }
        public float _y { get; set; }

        public int Width { get; set; } = 32;
        public int Height { get; set; } = 32;

        public float _speed { get; set; } = 2f;
        public int _health { get; set; } = 50;

        public int _xpValue { get; set; } = 5;

        public Mob(int screenWidth, int screenHeight)
        {
            int side = GlobalHelpers.alea.Next(0, 4);

            switch (side)
            {
                case 0:
                    _x = GlobalHelpers.alea.Next(0, screenWidth - Width);
                    _y = -Height;
                    break;

                case 1:
                    _x = screenWidth;
                    _y = GlobalHelpers.alea.Next(0, screenHeight -  Height);
                    break;

                case 2:
                    _x = GlobalHelpers.alea.Next(0, screenWidth - Width);
                    _y = screenHeight;
                    break;

                case 3:
                    _x = -Width;
                    _y = GlobalHelpers.alea.Next(0, screenHeight - Height);
                    break;
            }
        }

        public void MoveTowards(float targetX, float targetY)
        {
            float dx = targetX - _x;
            float dy = targetY - _y;
            float length = (float)Math.Sqrt(dx * dx + dy * dy);

            if (length > 0)
            {
                dx /= length;
                dy /= length;
            }

            _x += dx * _speed;
            _y += dy * _speed;
        }

        public void TakeDamage(float damage)
        {
            _health -= (int)damage;
        }

        public void ClampToScreen(int screenWidth, int screenHeight)
        {
            if (_x < 0) _x = 0;
            if (_y < 0) _y = 0;
            if (_x + Width > screenWidth) _x = screenWidth - Width;
            if (_y + Height > screenHeight) _y = screenHeight - Height;
        }

        public void ClampAgainstObstacles(List<Obstacle> obstacles)
        {
            foreach (var obs in obstacles)
            {
                if (_x < obs._x + obs.Width &&
                    _x + Width > obs._x &&
                    _y < obs._y + obs.Height &&
                    _y + Height > obs._y)
                {
                    float overlapX = Math.Min(_x + Width - obs._x, obs._x + obs.Width - _x);
                    float overlapY = Math.Min(_y + Height - obs._y, obs._y + obs.Height - _y);

                    if (overlapX < overlapY)
                    {
                        if (_x + Width / 2 < obs._x + obs.Width / 2)
                            _x -= overlapX;
                        else
                            _x += overlapX;
                    }
                    else
                    {
                        if (_y + Height / 2 < obs._y + obs.Height / 2)
                            _y -= overlapY;
                        else
                            _y += overlapY;
                    }
                }
            }
        }
    }
}
