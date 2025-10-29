using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZombieSurvivor.Model
{
    public abstract class Weapon
    {
        public float _damage { get; set; }
        public float _fireRate { get; set; }
        protected float _cooldownTimer = 0f;

        public abstract void Shoot(float x, float y, List<Bullet> bullets, float deltaTime, float dirX, float dirY, List<Beam> beams = null);

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

        public override void Shoot(float x, float y, List<Bullet> bullets, float deltaTime, float dirX, float dirY, List<Beam> beams = null)
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

    public class DiagonalLaserBeamGun : Weapon
    {
        public float _beamLifetime { get; set; } = 0.3f;

        public DiagonalLaserBeamGun()
        {
            _damage = 8;
            _fireRate = 2f;
        }

        public override void Shoot(float x, float y, List<Bullet> bullets, float deltaTime, float dirX, float dirY, List<Beam> beams = null)
        {
            if (beams == null) return;

            UpdateCooldown(deltaTime);

            if (_cooldownTimer <= 0)
            {
                Color _laserColor = Color.Purple;
                float infiniteDistance = 2000f;
                float playerOffset = 16f; 

                beams.Add(new Beam(-playerOffset, -playerOffset, x - infiniteDistance, y - infiniteDistance, _laserColor, _damage, _beamLifetime));
                beams.Add(new Beam(playerOffset, -playerOffset, x + infiniteDistance, y - infiniteDistance, _laserColor, _damage, _beamLifetime));
                beams.Add(new Beam(-playerOffset, playerOffset, x - infiniteDistance, y + infiniteDistance, _laserColor, _damage, _beamLifetime));
                beams.Add(new Beam(playerOffset, playerOffset, x + infiniteDistance, y + infiniteDistance, _laserColor, _damage, _beamLifetime));

                _cooldownTimer = 1f / _fireRate;
            }
        }
    }
}
