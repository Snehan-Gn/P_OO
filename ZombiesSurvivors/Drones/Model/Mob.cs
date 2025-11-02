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
        public float x {  get; set; }
        public float y { get; set; }

        public int Width { get; set; } = 32;
        public int Height { get; set; } = 32;

        public float speed { get; set; } = 2f;
        public int health { get; set; } = 50;

        public int xpValue { get; set; } = 5;

        public Mob(int screenWidth, int screenHeight)
        {
            int side = GlobalHelpers.alea.Next(0, 4);

            switch (side)
            {
                case 0:
                    x = GlobalHelpers.alea.Next(0, screenWidth - Width);
                    y = -Height;
                    break;

                case 1:
                    x = screenWidth;
                    y = GlobalHelpers.alea.Next(0, screenHeight -  Height);
                    break;

                case 2:
                    x = GlobalHelpers.alea.Next(0, screenWidth - Width);
                    y = screenHeight;
                    break;

                case 3:
                    x = -Width;
                    y = GlobalHelpers.alea.Next(0, screenHeight - Height);
                    break;
            }
        }

        public void MoveTowards(float targetX, float targetY)
        {
            float dx = targetX - x;
            float dy = targetY - y;
            float length = (float)Math.Sqrt(dx * dx + dy * dy);

            if (length > 0)
            {
                dx /= length;
                dy /= length;
            }

            x += dx * speed;
            y += dy * speed;
        }

        public void TakeDamage(float damage)
        {
            health -= (int)damage;
        }

        public void ClampToScreen(int screenWidth, int screenHeight)
        {
            if (x < 0) x = 0;
            if (y < 0) y = 0;
            if (x + Width > screenWidth) x = screenWidth - Width;
            if (y + Height > screenHeight) y = screenHeight - Height;
        }

        public void ClampAgainstObstacles(List<Obstacle> obstacles)
        {
            foreach (var obs in obstacles)
            {
                if (x < obs.x + obs.Width &&
                    x + Width > obs.x &&
                    y < obs.y + obs.Height &&
                    y + Height > obs.y)
                {
                    float overlapX = Math.Min(x + Width - obs.x, obs.x + obs.Width - x);
                    float overlapY = Math.Min(y + Height - obs.y, obs.y + obs.Height - y);

                    if (overlapX < overlapY)
                    {
                        if (x + Width / 2 < obs.x + obs.Width / 2)
                            x -= overlapX;
                        else
                            x += overlapX;
                    }
                    else
                    {
                        if (y + Height / 2 < obs.y + obs.Height / 2)
                            y -= overlapY;
                        else
                            y += overlapY;
                    }
                }
            }
        }
    }
}
