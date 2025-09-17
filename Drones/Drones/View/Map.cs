using ZombieSurvivor.Model;

namespace ZombieSurvivor
{

    public partial class Map : Form
    {
        private Player player;
        private System.Windows.Forms.Timer gameTimer;

        private bool keyW = false;  // Up
        private bool keyS = false;  // Down
        private bool keyA = false;  // Left
        private bool keyD = false;  // Right

        public Map()
        {
            InitializeComponent();
            InitializeGame();
        }

        private void InitializeGame()
        {
            player = new Player(this.ClientSize.Width / 2, this.ClientSize.Height / 2, 100);

            gameTimer = new System.Windows.Forms.Timer();
            gameTimer.Interval = 16; 
            gameTimer.Tick += GameTimer_Tick;
            gameTimer.Start();
        }

        private void GameTimer_Tick(object sender, EventArgs e)
        { 
            player.Update(keyW, keyS, keyA, keyD);

            player.ClampToScreen(this.ClientSize.Width, this.ClientSize.Height);

            this.Invalidate();
        }

        private void Map_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            g.Clear(Color.DarkBlue);

            Brush playerBrush = new SolidBrush(Color.White);
            g.FillRectangle(playerBrush, player._x, player._y, player.Width, player.Height);

            string controlsText = "Contrôles: WASD pour se déplacer";
            g.DrawString(controlsText, this.Font, Brushes.Yellow, 10, 10);

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