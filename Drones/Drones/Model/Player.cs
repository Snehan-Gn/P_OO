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

        public int Width { get; set; } = 32;
        public int Height { get; set; } = 32;

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

            if (deltaX != 0 && deltaY != 0)
            {
                float diagonalSpeed = _speed * 0.707f;
                deltaX = deltaX > 0 ? diagonalSpeed : -diagonalSpeed;
                deltaY = deltaY > 0 ? diagonalSpeed : -diagonalSpeed;
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
