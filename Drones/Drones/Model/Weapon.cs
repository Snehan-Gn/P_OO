using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZombieSurvivor.Model
{
    public abstract class Weapon
    {
        public float _damage {  get; set; }
        public float _fireRate { get; set; }
        protected float _cooldownTimer = 0f;

        public abstract void Shoot(float x, float y, List<Bullet> bullets, float deltaTime, float dirX, float dirY);

        public void UpdateCooldown(float deltaTime)
        {
            if (_cooldownTimer > 0)
                _cooldownTimer -= deltaTime;
        }
    }
    public class Gun : Weapon
    {
        public float BulletSpeed { get; set; } = 10f;

        public Gun()
        {
            _damage = 10;
            _fireRate = 5f;
        }

        public override void Shoot(float x, float y, List<Bullet> bullets, float deltaTime, float dirX, float dirY)
        {
            UpdateCooldown(deltaTime);

            if (_cooldownTimer <= 0 && (dirX != 0 || dirY != 0))
            {
                float spawnOffset = 20f;
                float bulletX = x + dirX * spawnOffset;
                float bulletY = y + dirY * spawnOffset;

                bullets.Add(new Bullet(bulletX, bulletY, dirX * BulletSpeed, dirY * BulletSpeed, _damage));
                _cooldownTimer = 1f / _fireRate;
            }
        }

    }
}
