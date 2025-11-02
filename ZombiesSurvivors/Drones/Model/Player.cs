using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZombieSurvivor.Model
{
    public partial class Player
    {
        public float x { get; set; }
        public float y { get; set; }

        public int width { get; set; } = 32;
        public int height { get; set; } = 32;

        public float speed { get; set; } = 5f;
        public int health { get; set; }
        public int level { get; set; } = 1;
        public int xp { get; set; } = 0;
        public int xp_require { get; set; } = 100;

        private bool _isDashing = false;
        private float _dashSpeed = 15f;
        private float _dashDuration = 0.2f;
        private float _dashCooldown = 2f;

        private float _dashTimer = 0f;
        private float _cooldownTimer = 0f;

        private float _dashDirX = 0;
        private float _dashDirY = 0;

        public List<Weapon> Weapons = new List<Weapon>();
        public List<Bullet> Bullets = new List<Bullet>();
        public List<Beam> _beams = new List<Beam>();

        public float dirX { get; private set; } = 0f;
        public float dirY { get; private set; } = -1f;

        public Player(float x, float y, int health)
        {
            this.x = x;
            this.y = y;
            this.health = 100;
        }

        public void Move(float deltaX, float deltaY)
        {
            x += deltaX;
            y += deltaY;
        }

        public void Update(bool moveUp, bool moveDown, bool moveLeft, bool moveRight, bool dash, float deltaTime)
        {
            float deltaX = 0;
            float deltaY = 0;

            if (moveLeft) deltaX -= speed;
            if (moveRight) deltaX += speed;
            if (moveUp) deltaY -= speed;
            if (moveDown) deltaY += speed;

            if (deltaX != 0 || deltaY != 0)
            {
                float length = (float)Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
                dirX = deltaX / length;
                dirY = deltaY / length;
            }

            if (dash && !_isDashing && _cooldownTimer <= 0 && (deltaX != 0 || deltaY != 0))
            {
                _isDashing = true;
                _dashTimer = _dashDuration;
                _cooldownTimer = _dashCooldown;

                float length = (float)Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
                _dashDirX = deltaX / length;
                _dashDirY = deltaY / length;
            }

            if (_isDashing)
            {
                Move(_dashDirX * _dashSpeed, _dashDirY * _dashSpeed);
                _dashTimer -= deltaTime;

                if (_dashTimer <= 0)
                {
                    _isDashing = false;
                }
            }
            else
            {
                Move(deltaX, deltaY);
            }

            if (_cooldownTimer > 0)
            {
                _cooldownTimer -= deltaTime;
            }

            Shoot(deltaTime);

        }

        public void TakeDamage(int damage)
        {
            health -= damage;
            if (health < 0) health = 0;
        }

        public bool IsColliding(Mob mob)
        {
            return x < mob.x + mob.Width &&
                   x + width > mob.x &&
                   y < mob.y + mob.Height &&
                   y + height > mob.y;
        }

        public void Shoot(float deltaTime)
        {
            foreach (var weapon in Weapons)
                weapon.Shoot(x + width / 2, y + height / 2, Bullets, deltaTime, dirX, dirY, _beams);
        }

        public void LevelUp(int _xp_to_add)
        {
            xp += _xp_to_add;
            if (xp >= xp_require)
            {
                level++;
                xp -= xp_require;
                health += 20;

                foreach (var weapon in Weapons)
                {
                    weapon.damage += 5;
                }
            }
        }
        public void ClampToWorld(int worldWidth, int worldHeight)
        {
            if (x < 0) x = 0;
            if (y < 0) y = 0;
            if (x + width > worldWidth) x = worldWidth - width;
            if (y + height > worldHeight) y = worldHeight - height;
        }

        public void ClampAgainstObstacles(List<Obstacle> obstacles)
        {
            foreach (var obs in obstacles)
            {
                if (x < obs.x + obs.Width &&
                    x + width > obs.x &&
                    y < obs.y + obs.Height &&
                    y + height > obs.y)
                {
                    float overlapX = Math.Min(x + width - obs.x, obs.x + obs.Width - x);
                    float overlapY = Math.Min(y + height - obs.y, obs.y + obs.Height - y);

                    if (overlapX < overlapY)
                    {
                        if (x + width / 2 < obs.x + obs.Width / 2)
                            x -= overlapX;
                        else
                            x += overlapX;
                    }
                    else
                    {
                        if (y + height / 2 < obs.y + obs.Height / 2)
                            y -= overlapY;
                        else
                            y += overlapY;
                    }
                }
            }
        }

    }
}
