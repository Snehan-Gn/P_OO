using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZombieSurvivor.Model
{
    public class Gun : Weapon
    {
        public float BulletSpeed { get; set; } = 10f;

        public Gun()
        {
            damage = 10;
            fireRate = 5f;
        }

        public override void Shoot(float x, float y, List<Bullet> bullets, float deltaTime, float dirX, float dirY, List<Beam> beams = null)
        {
            UpdateCooldown(deltaTime);

            if (_cooldownTimer <= 0 && (dirX != 0 || dirY != 0))
            {
                float spawnOffset = 20f;
                float bulletX = x + dirX * spawnOffset;
                float bulletY = y + dirY * spawnOffset;

                bullets.Add(new Bullet(bulletX, bulletY, dirX * BulletSpeed, dirY * BulletSpeed, damage));
                _cooldownTimer = 1f / fireRate;
            }
        }
    }
}
