using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZombieSurvivor.Model
{
    public partial class Player
    {
        public float _x { get; set; }
        public float _y { get; set; }

        public int _width { get; set; } = 32;
        public int _height { get; set; } = 32;

        public float _speed { get; set; } = 5f;
        public int _health { get; set; }

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

        public float _dirX { get; private set; } = 0f;
        public float _dirY { get; private set; } = -1f;

        public Player(float x, float y, int health)
        {
            _x = x;
            _y = y;
            _health = 100;
        }

        public void Move(float deltaX, float deltaY)
        {
            _x += deltaX;
            _y += deltaY;
        }

        public void Update(bool moveUp, bool moveDown, bool moveLeft, bool moveRight, bool dash, float deltaTime)
        {
            float deltaX = 0;
            float deltaY = 0;

            if (moveLeft) deltaX -= _speed;
            if (moveRight) deltaX += _speed;
            if (moveUp) deltaY -= _speed;
            if (moveDown) deltaY += _speed;

            if (deltaX != 0 || deltaY != 0)
            {
                float length = (float)Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
                _dirX = deltaX / length;
                _dirY = deltaY / length;
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
            _health -= damage;
            if (_health < 0) _health = 0;
        }

        public bool IsColliding(Mob mob)
        {
            return _x < mob._x + mob.Width &&
                   _x + _width > mob._x &&
                   _y < mob._y + mob.Height &&
                   _y + _height > mob._y;
        }

        public void Shoot(float deltaTime)
        {
            foreach (var weapon in Weapons)
                weapon.Shoot(_x + _width / 2, _y + _height / 2, Bullets, deltaTime, _dirX, _dirY, _beams);
        }


        public void ClampToScreen(int screenWidth, int screenHeight)
        {
            if (_x < 0) _x = 0;
            if (_y < 0) _y = 0;
            if (_x + _width > screenWidth) _x = screenWidth - _width;
            if (_y + _height > screenHeight) _y = screenHeight - _height;
        }
    }
}
