using System;
using System.Collections.Generic;
using System.Drawing;

namespace ZombieSurvivor.Model
{
    public class RangedMob : Mob
    {
        private float shootingCooldown = 0f;

        public float fireRate = 1f;          
        public float minDistance = 150f;     
        public float shootingRange = 800f;   
        public float bulletSpeed = 8f;       

        public RangedMob(int worldWidth, int worldHeight) : base(worldWidth, worldHeight)
        {
            health = 30;
            speed = 1.5f;
            xpValue = 8;
        }

        public void Update(Player player, List<Bullet> bullets, float deltaTime)
        {
            float dx = player.x + player.width / 2 - (x + Width / 2);
            float dy = player.y + player.height / 2 - (y + Height / 2);
            float distance = (float)Math.Sqrt(dx * dx + dy * dy);

            if (distance > minDistance)
            {
                float moveX = dx / distance;
                float moveY = dy / distance;
                x += moveX * speed;
                y += moveY * speed;
            }

            shootingCooldown -= deltaTime;

            if (distance >= minDistance && distance <= shootingRange && shootingCooldown <= 0f)
            {
                float dirX = dx / distance;
                float dirY = dy / distance;

                bullets.Add(new Bullet(
                    x + Width / 2,
                    y + Height / 2,
                    dirX * bulletSpeed,
                    dirY * bulletSpeed,
                    8,
                    Color.Red
                ));

                shootingCooldown = 1f / fireRate;
            }
        }
    }
}
