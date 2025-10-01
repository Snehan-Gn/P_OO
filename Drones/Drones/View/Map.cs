using ZombieSurvivor.Model;

namespace ZombieSurvivor
{

    public partial class Map : Form
    {
        private Player player;
        private System.Windows.Forms.Timer gameTimer;

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

            gameTimer = new System.Windows.Forms.Timer();
            gameTimer.Interval = 16; 
            gameTimer.Tick += GameTimer_Tick;
            gameTimer.Start();
        }

        private void GameTimer_Tick(object sender, EventArgs e)
        {
            float deltaTime = gameTimer.Interval / 1000f;

            player.Update(keyW, keyS, keyA, keyD, dash,deltaTime);

            player.ClampToScreen(this.ClientSize.Width, this.ClientSize.Height);

            dash = false;

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