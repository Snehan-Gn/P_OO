using ZombieSurvivor.Model;

namespace ZombieSurvivor
{
    public partial class Map : Form
    {
        private Player player;
        private List<Mob> mobs = new List<Mob>();
        private List<Bullet> enemyBullets = new List<Bullet>();
        private List<Obstacle> obstacles = new List<Obstacle>();
        private List<HealthPickup> pickups = new List<HealthPickup>();

        private System.Windows.Forms.Timer gameTimer;

        private int worldWidth = 2000;
        private int worldHeight = 2000;
        private float cameraX = 0;
        private float cameraY = 0;

        private int maxMobs = 100;
        private float mobSpawnTimer = 0f;
        private float mobSpawnInterval = 2f;

        private float damageCooldown = 0f;
        private float damageInterval = 0.5f;

        private bool keyW = false;
        private bool keyS = false;
        private bool keyA = false;
        private bool keyD = false;
        private bool dash = false;

        public Map()
        {
            InitializeComponent();
            InitializeGame();
        }

        private void InitializeGame()
        {
            player = new Player(worldWidth / 2, worldHeight / 2, 100);
            player.Weapons.Add(new Gun());
            player.Weapons.Add(new DiagonalLaserBeamGun());

            Random rand = new Random();
            for (int i = 0; i < 5; i++)
            {
                float ox = rand.Next(0, 2000); 
                float oy = rand.Next(0, 2000); 
                obstacles.Add(new Obstacle(ox, oy));
            }

            gameTimer = new System.Windows.Forms.Timer();
            gameTimer.Interval = 16;
            gameTimer.Tick += GameTimer_Tick;
            gameTimer.Start();
        }

        private void GameTimer_Tick(object sender, EventArgs e)
        {
            float deltaTime = gameTimer.Interval / 1000f;

            damageCooldown -= deltaTime;

            player.Update(keyW, keyS, keyA, keyD, dash, deltaTime);
            player.ClampToWorld(worldWidth, worldHeight);

            mobSpawnTimer += deltaTime;
            if (mobSpawnTimer >= mobSpawnInterval && mobs.Count < maxMobs)
            {
                int roll = GlobalHelpers.alea.Next(0, 3);
                if (roll < 2)
                    mobs.Add(new Mob(worldWidth, worldHeight));
                else
                    mobs.Add(new RangedMob(worldWidth, worldHeight));
                mobSpawnTimer = 0f;
            }

            foreach (var mob in mobs.ToList())
            {
                if (mob is RangedMob ranged)
                {
                    ranged.Update(player, enemyBullets, deltaTime);
                    ranged.ClampToScreen(worldWidth, worldHeight);
                }
                else
                {
                    mob.MoveTowards(player._x, player._y);
                    mob.ClampToScreen(worldWidth, worldHeight);
                }

                if (player.IsColliding(mob) && damageCooldown <= 0f)
                {
                    player.TakeDamage(10);
                    damageCooldown = damageInterval;
                }
            }

            dash = false;

            foreach (var bullet in player.Bullets)
                bullet.Update();

            player.Bullets.RemoveAll(b =>
                b._x < -b._width || b._y < -b._height || b._x > worldWidth + b._width || b._y > worldHeight + b._height
            );

            foreach (var bullet in enemyBullets)
                bullet.Update();

            enemyBullets.RemoveAll(b =>
                b._x < -b._width || b._y < -b._height || b._x > worldWidth + b._width || b._y > worldHeight + b._height
            );

            foreach (var bullet in enemyBullets.ToList())
            {
                if (player._x < bullet._x + bullet._width &&
                    player._x + player._width > bullet._x &&
                    player._y < bullet._y + bullet._height &&
                    player._y + player._height > bullet._y)
                {
                    player.TakeDamage((int)bullet._damage);
                    enemyBullets.Remove(bullet);
                }
            }

            foreach (var beam in player._beams)
            {
                beam.UpdatePosition(player._x + player._width / 2, player._y + player._height / 2);
                beam._lifetime -= deltaTime;
            }

            player._beams.RemoveAll(b => b._lifetime <= 0f);

            foreach (var bullet in player.Bullets.ToList())
            {
                foreach (var mob in mobs.ToList())
                {
                    if (bullet._x < mob._x + mob.Width &&
                        bullet._x + bullet._width > mob._x &&
                        bullet._y < mob._y + mob.Height &&
                        bullet._y + bullet._height > mob._y)
                    {
                        mob.TakeDamage(bullet._damage);
                        player.Bullets.Remove(bullet);

                        if (mob._health <= 0)
                        {
                            mobs.Remove(mob);
                            player.LevelUp(mob._xpValue);
                        }
                        break;
                    }
                }

                foreach (var obs in obstacles.ToList())
                {
                    if (bullet._x < obs._x + obs.Width &&
                        bullet._x + bullet._width > obs._x &&
                        bullet._y < obs._y + obs.Height &&
                        bullet._y + bullet._height > obs._y)
                    {
                        obs.TakeDamage((int)bullet._damage);
                        player.Bullets.Remove(bullet);

                        if (obs.IsDestroyed)
                        {
                            pickups.Add(new HealthPickup(obs._x + obs.Width / 2 - 5, obs._y + obs.Height / 2 - 5));
                            obstacles.Remove(obs);
                        }
                        break;
                    }
                }
            }

            foreach (var p in pickups.ToList())
            {
                if (player._x < p._x + p._size &&
                    player._x + player._width > p._x &&
                    player._y < p._y + p._size &&
                    player._y + player._height > p._y)
                {
                    player._health += p._amount;
                    if (player._health > 100) player._health = 100;
                    pickups.Remove(p);
                }
            }

            UpdateCamera();
            this.Invalidate();
        }

        private void UpdateCamera()
        {
            cameraX = player._x + player._width / 2 - this.ClientSize.Width / 2;
            cameraY = player._y + player._height / 2 - this.ClientSize.Height / 2;

            cameraX = Math.Max(0, Math.Min(cameraX, worldWidth - this.ClientSize.Width));
            cameraY = Math.Max(0, Math.Min(cameraY, worldHeight - this.ClientSize.Height));
        }

        private void Map_Paint(object sender, PaintEventArgs e)
        {
            int barWidth = 200;
            int barHeight = 20;
            int barX = 10;
            int healthBarY = 40;
            int xpBarY = 60;

            float healthPercent = (float)player._health / 100f;
            int healthWidth = (int)(barWidth * healthPercent);

            float xpPercent = (float)player._xp / 100f;
            int xpWidth = (int)(barWidth * xpPercent);

            Graphics g = e.Graphics;
            g.Clear(Color.DarkBlue);

            foreach (var mob in mobs)
            {
                if (mob is RangedMob)
                    g.FillRectangle(Brushes.OrangeRed, mob._x - cameraX, mob._y - cameraY, mob.Width, mob.Height);
                else
                    g.FillRectangle(Brushes.Red, mob._x - cameraX, mob._y - cameraY, mob.Width, mob.Height);
            }

            g.FillRectangle(Brushes.White, player._x - cameraX, player._y - cameraY, player._width, player._height);

            foreach (var bullet in player.Bullets)
                g.FillEllipse(Brushes.Blue, bullet._x - bullet._radius - cameraX, bullet._y - bullet._radius - cameraY, bullet._width, bullet._height);

            foreach (var bullet in enemyBullets)
                g.FillEllipse(Brushes.Red, bullet._x - bullet._radius - cameraX, bullet._y - bullet._radius - cameraY, bullet._width, bullet._height);

            foreach (var beam in player._beams)
            {
                using (Pen p = new Pen(beam._color, 3))
                    g.DrawLine(p, beam._startX - cameraX, beam._startY - cameraY, beam._endX - cameraX, beam._endY - cameraY);
            }

            g.FillRectangle(Brushes.Red, barX, healthBarY, barWidth, barHeight);
            g.FillRectangle(Brushes.Green, barX, healthBarY, healthWidth, barHeight);
            g.DrawRectangle(Pens.Black, barX, healthBarY, barWidth, barHeight);

            g.FillRectangle(Brushes.Black, barX, xpBarY, barWidth, barHeight);
            g.FillRectangle(Brushes.LightBlue, barX, xpBarY, xpWidth, barHeight);
            g.DrawRectangle(Pens.Black, barX, xpBarY, barWidth, barHeight);

            g.DrawString("Contrôles: WASD pour se déplacer", this.Font, Brushes.Yellow, 10, 10);

            using (Brush obsBrush = new SolidBrush(Color.Brown))
            {
                foreach (var obs in obstacles)
                    g.FillRectangle(obsBrush, obs._x - cameraX, obs._y - cameraY, obs.Width, obs.Height);
            }

            using (Brush pickupBrush = new SolidBrush(Color.Green))
            {
                foreach (var p in pickups)
                    g.FillRectangle(pickupBrush, p._x - cameraX, p._y - cameraY, p._size, p._size);
            }
        }

        private void Map_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.W:
                    keyW = true;
                    break;
                case Keys.S:
                    keyS = true;
                    break;
                case Keys.A:
                    keyA = true;
                    break;
                case Keys.D:
                    keyD = true;
                    break;
                case Keys.Space:
                    dash = true;
                    break;
            }
        }

        private void Map_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.W:
                    keyW = false;
                    break;
                case Keys.S:
                    keyS = false;
                    break;
                case Keys.A:
                    keyA = false;
                    break;
                case Keys.D:
                    keyD = false;
                    break;
            }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            gameTimer?.Stop();
            gameTimer?.Dispose();
            base.OnFormClosed(e);
        }
    }
}
