using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZombieSurvivor.Model
{
    public abstract class Weapon
    {
        public float damage { get; set; }
        public float fireRate { get; set; }
        protected float _cooldownTimer = 0f;

        public abstract void Shoot(float x, float y, List<Bullet> bullets, float deltaTime, float dirX, float dirY, List<Beam> beams = null);

        public void UpdateCooldown(float deltaTime)
        {
            if (_cooldownTimer > 0)
                _cooldownTimer -= deltaTime;
        }
    }
    
    
    /*public class DiagonalLaserBeamGun : Weapon
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
    }*/
}
