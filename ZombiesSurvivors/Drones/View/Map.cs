using ZombieSurvivor.Model;

namespace ZombieSurvivor
{

    public partial class Map : Form
    {
        private Player player;
        private List<Mob> mobs= new List<Mob>();
        private System.Windows.Forms.Timer gameTimer;

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
            player = new Player(this.ClientSize.Width / 2, this.ClientSize.Height / 2, 100);
            player.Weapons.Add(new Gun());
            player.Weapons.Add(new DiagonalLaserBeamGun());

            gameTimer = new System.Windows.Forms.Timer();
            gameTimer.Interval = 16; 
            gameTimer.Tick += GameTimer_Tick;
            gameTimer.Start();
        }

        private void GameTimer_Tick(object sender, EventArgs e)
        {
            float deltaTime = gameTimer.Interval / 1000f;

            damageCooldown -= deltaTime;

            player.Update(keyW, keyS, keyA, keyD, dash,deltaTime);

            player.ClampToScreen(this.ClientSize.Width, this.ClientSize.Height);

            mobSpawnTimer += deltaTime;
            if (mobSpawnTimer >= mobSpawnInterval && mobs.Count < maxMobs)
            {
                mobs.Add(new Mob(this.ClientSize.Width, this.ClientSize.Height));
                mobSpawnTimer = 0f;
            }

            foreach (var mob in mobs)
            {
                mob.MoveTowards(player._x, player._y);
                mob.ClampToScreen(this.ClientSize.Width, this.ClientSize.Height);

                if (player.IsColliding(mob) && damageCooldown <= 0f)
                {
                    player.TakeDamage(10);
                    damageCooldown = damageInterval;
                }
            }

            dash = false;

            foreach (var bullet in player.Bullets)
            {
                bullet.Update(); 
            }

            player.Bullets.RemoveAll(b =>
                b._x < -b._width || b._y < -b._height || b._x > this.ClientSize.Width + b._width || b._y > this.ClientSize.Height + b._height
            );

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
                            mobs.Remove(mob); 
                        break; 
                    }
                }
            }

            this.Invalidate();
        }

        private void Map_Paint(object sender, PaintEventArgs e)
        {
            int barWidth = 200;
            int barHeight = 20;
            int barX = 10;
            int barY = 40;

            float healthPercent = (float)player._health / 100f;
            int healthWidth = (int)(barWidth * healthPercent);

            Graphics g = e.Graphics;

            g.Clear(Color.DarkBlue);

            Brush playerBrush = new SolidBrush(Color.White);

            g.FillRectangle(playerBrush, player._x, player._y, player._width, player._height);

            using (Brush mobBrush = new SolidBrush(Color.Red))
            
            {
                foreach (var mob in mobs)
                    g.FillRectangle(mobBrush, mob._x, mob._y, mob.Width, mob.Height);
            }


            g.FillRectangle(Brushes.Red, barX, barY, barWidth, barHeight);
            
            g.FillRectangle(Brushes.Green, barX, barY, healthWidth, barHeight);

            g.DrawRectangle(Pens.Black, barX, barY, barWidth, barHeight);

            string controlsText = "Contrôles: WASD pour se déplacer";
            g.DrawString(controlsText, this.Font, Brushes.Yellow, 10, 10);

            foreach (var bullet in player.Bullets)
            {
                g.FillEllipse(Brushes.Blue, bullet._x - 2, bullet._y - 2, 4, 4);
            }


            foreach (var beam in player._beams)
            {
                using (Pen p = new Pen(beam._color, 3))
                {
                    g.DrawLine(p, beam._startX, beam._startY, beam._endX, beam._endY);
                }
            }

            using (Pen glowPen = new Pen(Color.LightBlue, 1.5f))
            {
                foreach (var bullet in player.Bullets)
                {
                    g.DrawEllipse(glowPen, bullet._x - bullet._radius, bullet._y - bullet._radius, bullet._width, bullet._height);
                }
            }


            playerBrush.Dispose();
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