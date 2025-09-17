using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZombieSurvivor.Model
{
    public partial class Player
    {
        public float _x {  get; set; }
        public float _y { get; set; }

        public int Width { get; set; } = 32;
        public int Height { get; set; } = 32;

        public float _speed { get; set; } = 5f;
        public int _health { get; set; }

        public Player(float x, float y, int health)
        {
            _x = x;
            _y = y;
            _health = 100;
        }

        public void Move(float deltaX, float deltaY ) 
        {
            _x += deltaX;
            _y += deltaY;
        }

        public void Update(bool moveUp, bool moveDown, bool moveLeft, bool moveRight)
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

            Move(deltaX, deltaY);
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
