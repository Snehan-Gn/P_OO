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
    }
}
