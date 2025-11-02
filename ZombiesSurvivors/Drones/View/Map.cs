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

        private int _worldWidth = 2000;
        private int _worldHeight = 2000;
        private float _cameraX = 0;
        private float _cameraY = 0;

        private int _maxMobs = 100;
        private float _mobSpawnTimer = 0f;
        private float _mobSpawnInterval = 2f;

        private float _damageCooldown = 0f;
        private float _damageInterval = 0.5f;

        private float _gameTime = 0f;
        private bool _gameOver = false;
        private int _score = 0;

        private bool _keyW = false;
        private bool _keyS = false;
        private bool _keyA = false;
        private bool _keyD = false;
        private bool _dash = false;

        public Map()
        {
            InitializeComponent();
            InitializeGame();
        }

        private void InitializeGame()
        {
            gameTimer?.Stop();
            gameTimer?.Dispose();
            mobs.Clear();
            enemyBullets.Clear();
            obstacles.Clear();
            pickups.Clear();

            _gameOver = false;
            _gameTime = 0f;
            _score = 0;
            _mobSpawnTimer = 0f;
            _damageCooldown = 0f;

            player = new Player(_worldWidth / 2, _worldHeight / 2, 100);
            player.Weapons.Clear();
            player.Weapons.Add(new Gun());
            //player.Weapons.Add(new DiagonalLaserBeamGun());
            player.Bullets.Clear();
            player._beams.Clear();

            Random rand = new Random();
            for (int i = 0; i < 5; i++)
            {
                float ox = rand.Next(0, _worldWidth - 64);
                float oy = rand.Next(0, _worldHeight - 64);
                obstacles.Add(new Obstacle(ox, oy));
            }

            gameTimer = new System.Windows.Forms.Timer();
            gameTimer.Interval = 16;
            gameTimer.Tick -= GameTimer_Tick;
            gameTimer.Tick += GameTimer_Tick;
            gameTimer.Start();
        }

        private void GameTimer_Tick(object sender, EventArgs e)
        {
            float deltaTime = gameTimer.Interval / 1000f;

            if (_gameOver) return;
            _gameTime += deltaTime;

            _damageCooldown -= deltaTime;

            player.Update(_keyW, _keyS, _keyA, _keyD, _dash, deltaTime);
            player.ClampToWorld(_worldWidth, _worldHeight);
            player.ClampAgainstObstacles(obstacles);

            _mobSpawnTimer += deltaTime;
            if (_mobSpawnTimer >= _mobSpawnInterval && mobs.Count < _maxMobs)
            {
                int roll = GlobalHelpers.alea.Next(0, 3);
                if (roll < 2)
                    mobs.Add(new Mob(_worldWidth, _worldHeight));
                else
                    mobs.Add(new RangedMob(_worldWidth, _worldHeight));
                _mobSpawnTimer = 0f;
            }

            foreach (var mob in mobs.ToList())
            {
                if (mob is RangedMob ranged)
                {
                    ranged.Update(player, enemyBullets, deltaTime);
                    ranged.ClampToScreen(_worldWidth, _worldHeight);
                }
                else
                {
                    mob.MoveTowards(player.x, player.y);
                    mob.ClampToScreen(_worldWidth, _worldHeight);
                    mob.ClampAgainstObstacles(obstacles);
                }

                if (player.IsColliding(mob) && _damageCooldown <= 0f)
                {
                    player.TakeDamage(10);
                    _damageCooldown = _damageInterval;
                }
            }

            _dash = false;

            foreach (var bullet in player.Bullets)
                bullet.Update();

            player.Bullets.RemoveAll(b =>
                b.x < -b.width || b.y < -b.height || b.x > _worldWidth + b.width || b.y > _worldHeight + b.height
            );

            foreach (var bullet in enemyBullets)
                bullet.Update();

            enemyBullets.RemoveAll(b =>
                b.x < -b.width || b.y < -b.height || b.x > _worldWidth + b.width || b.y > _worldHeight + b.height
            );

            foreach (var bullet in enemyBullets.ToList())
            {
                if (player.x < bullet.x + bullet.width &&
                    player.x + player.width > bullet.x &&
                    player.y < bullet.y + bullet.height &&
                    player.y + player.height > bullet.y)
                {
                    player.TakeDamage((int)bullet.damage);
                    enemyBullets.Remove(bullet);
                }
            }

            foreach (var beam in player._beams)
            {
                beam.UpdatePosition(player.x + player.width / 2, player.y + player.height / 2);
                beam.lifetime -= deltaTime;
            }

            player._beams.RemoveAll(b => b.lifetime <= 0f);

            foreach (var bullet in player.Bullets.ToList())
            {
                foreach (var mob in mobs.ToList())
                {
                    if (bullet.x < mob.x + mob.Width &&
                        bullet.x + bullet.width > mob.x &&
                        bullet.y < mob.y + mob.Height &&
                        bullet.y + bullet.height > mob.y)
                    {
                        mob.TakeDamage(bullet.damage);
                        player.Bullets.Remove(bullet);

                        if (mob.health <= 0)
                        {
                            mobs.Remove(mob);
                            player.LevelUp(mob.xpValue);
                            _score++;
                        }
                        break;
                    }
                }

                foreach (var obs in obstacles.ToList())
                {
                    if (bullet.x < obs.x + obs.Width &&
                        bullet.x + bullet.width > obs.x &&
                        bullet.y < obs.y + obs.Height &&
                        bullet.y + bullet.height > obs.y)
                    {
                        obs.TakeDamage((int)bullet.damage);
                        player.Bullets.Remove(bullet);

                        if (obs.IsDestroyed)
                        {
                            pickups.Add(new HealthPickup(obs.x + obs.Width / 2 - 5, obs.y + obs.Height / 2 - 5));
                            obstacles.Remove(obs);
                        }
                        break;
                    }
                }
            }

            if (player.health <= 0)
            {
                _gameOver = true;
                gameTimer.Stop();
                Invalidate();
                return;
            }

            foreach (var p in pickups.ToList())
            {
                if (player.x < p.x + p.size &&
                    player.x + player.width > p.x &&
                    player.y < p.y + p.size &&
                    player.y + player.height > p.y)
                {
                    player.health += p.amount;
                    if (player.health > 100) player.health = 100;
                    pickups.Remove(p);
                }
            }


            UpdateCamera();
            this.Invalidate();
        }

        private void UpdateCamera()
        {
            _cameraX = player.x + player.width / 2 - this.ClientSize.Width / 2;
            _cameraY = player.y + player.height / 2 - this.ClientSize.Height / 2;

            _cameraX = Math.Max(0, Math.Min(_cameraX, _worldWidth - this.ClientSize.Width));
            _cameraY = Math.Max(0, Math.Min(_cameraY, _worldHeight - this.ClientSize.Height));
        }

        private void Map_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            g.Clear(Color.DarkBlue);

            foreach (var mob in mobs)
            {
                if (mob is RangedMob)
                    g.FillRectangle(Brushes.Purple, mob.x - _cameraX, mob.y - _cameraY, mob.Width, mob.Height);
                else
                    g.FillRectangle(Brushes.Red, mob.x - _cameraX, mob.y - _cameraY, mob.Width, mob.Height);
            }

            g.FillRectangle(Brushes.White, player.x - _cameraX, player.y - _cameraY, player.width, player.height);

            foreach (var bullet in player.Bullets)
                g.FillEllipse(Brushes.Blue, bullet.x - bullet.radius - _cameraX, bullet.y - bullet.radius - _cameraY, bullet.width, bullet.height);

            foreach (var bullet in enemyBullets)
                g.FillEllipse(Brushes.Red, bullet.x - bullet.radius - _cameraX, bullet.y - bullet.radius - _cameraY, bullet.width, bullet.height);

            foreach (var beam in player._beams)
            {
                using (Pen p = new Pen(beam.color, 3))
                    g.DrawLine(p, beam.startX - _cameraX, beam.startY - _cameraY, beam.endX - _cameraX, beam.endY - _cameraY);
            }

            using (Brush obsBrush = new SolidBrush(Color.Yellow))
            {
                foreach (var obs in obstacles)
                    g.FillRectangle(obsBrush, obs.x - _cameraX, obs.y - _cameraY, obs.Width, obs.Height);
            }

            using (Brush pickupBrush = new SolidBrush(Color.Green))
            {
                foreach (var p in pickups)
                    g.FillRectangle(pickupBrush, p.x - _cameraX, p.y - _cameraY, p.size, p.size);
            }

            int barWidth = 200;
            int barHeight = 20;
            int barX = 10;
            int healthBarY = 40;
            int xpBarY = 60;

            float healthPercent = (float)player.health / 100f;
            int healthWidth = (int)(barWidth * healthPercent);

            float xpPercent = (float)player.xp / 100f;
            int xpWidth = (int)(barWidth * xpPercent);

            g.FillRectangle(Brushes.Red, barX, healthBarY, barWidth, barHeight);
            g.FillRectangle(Brushes.Green, barX, healthBarY, healthWidth, barHeight);
            g.DrawRectangle(Pens.Black, barX, healthBarY, barWidth, barHeight);

            g.FillRectangle(Brushes.Black, barX, xpBarY, barWidth, barHeight);
            g.FillRectangle(Brushes.LightBlue, barX, xpBarY, xpWidth, barHeight);
            g.DrawRectangle(Pens.Black, barX, xpBarY, barWidth, barHeight);

            g.DrawString("Contrôles: WASD pour se déplacer et Espace pour le dash", this.Font, Brushes.Yellow, 10, 10);

            string topRightTimeText = $"Time: {Math.Floor(_gameTime)}s";
            string topRightScoreText = $"Score: {_score}";
            SizeF timeSize = g.MeasureString(topRightTimeText, this.Font);
            SizeF scoreSize = g.MeasureString(topRightScoreText, this.Font);
            float margin = 10f;
            float x = ClientSize.Width - Math.Max(timeSize.Width, scoreSize.Width) - margin;

            g.DrawString(topRightTimeText, this.Font, Brushes.White, x, 10);
            g.DrawString(topRightScoreText, this.Font, Brushes.White, x, 30);

            if (player.health <= 0)
            {
                string lostText = "YOU LOST";
                string lostTimeText = $"Time: {Math.Floor(_gameTime)}s";
                string lostScoreText = $"Score: {_score}";
                string restartText = "Press R to Restart";

                Font bigFont = new Font(this.Font.FontFamily, 48, FontStyle.Bold);
                Font mediumFont = new Font(this.Font.FontFamily, 22, FontStyle.Bold);
                Font smallFont = new Font(this.Font.FontFamily, 14, FontStyle.Regular);

                SizeF lostSize = g.MeasureString(lostText, bigFont);
                SizeF lostTimeSize = g.MeasureString(lostTimeText, mediumFont);
                SizeF lostScoreSize = g.MeasureString(lostScoreText, mediumFont);
                SizeF lostRestartSize = g.MeasureString(restartText, smallFont);

                float centerX = ClientSize.Width / 2f;
                float centerY = ClientSize.Height / 2f;

                g.FillRectangle(new SolidBrush(Color.FromArgb(180, 0, 0, 0)), 0, 0, ClientSize.Width, ClientSize.Height);

                using (Brush redBrush = new SolidBrush(Color.Red))
                using (Brush whiteBrush = new SolidBrush(Color.White))
                using (Brush yellowBrush = new SolidBrush(Color.Yellow))
                {
                    g.DrawString(lostText, bigFont, redBrush, centerX - lostSize.Width / 2, centerY - lostSize.Height - 60);
                    g.DrawString(lostScoreText, mediumFont, whiteBrush, centerX - lostScoreSize.Width / 2, centerY - lostScoreSize.Height / 2);
                    g.DrawString(lostTimeText, mediumFont, whiteBrush, centerX - lostTimeSize.Width / 2, centerY + lostScoreSize.Height / 2 + 10);
                    g.DrawString(restartText, smallFont, yellowBrush, centerX - lostRestartSize.Width / 2, centerY + lostTimeSize.Height + 60);
                }

                bigFont.Dispose();
                mediumFont.Dispose();
                smallFont.Dispose();
                return;
            }
        }


        private void Map_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.W:
                    _keyW = true;
                    break;
                case Keys.S:
                    _keyS = true;
                    break;
                case Keys.A:
                    _keyA = true;
                    break;
                case Keys.D:
                    _keyD = true;
                    break;
                case Keys.Space:
                    _dash = true;
                    break;
                case Keys.R:
                    if (_gameOver)
                        InitializeGame();
                    break;
            }
        }

        private void Map_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.W:
                    _keyW = false;
                    break;
                case Keys.S:
                    _keyS = false;
                    break;
                case Keys.A:
                    _keyA = false;
                    break;
                case Keys.D:
                    _keyD = false;
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
