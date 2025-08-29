
namespace EscapeFromDungeon
{
    public partial class Form1 : Form
    {
        private PictureBox mapImage, overlayImg, playerImg;
        private GameManager gameManager;
        private DrawInfo drawInfo;
        private System.Windows.Forms.Timer timer;//画面表示更新用
        private const int timerInterval = 16; // 約60FPS

        public Form1()
        {
            InitializeComponent();
            init();
        }

        private void init()
        {
            gameManager = new GameManager();
            drawInfo = new DrawInfo();

            InitPictureBoxes();
            MsgBox.Location = new Point(10, 440);

            gameManager.Map.Draw(mapImage);
            gameManager.Map.DrawBrightness(overlayImg);
            gameManager.SetMapPos(mapImage, overlayImg, playerImg);
            DispPoint();
            timer = new System.Windows.Forms.Timer();
            timer.Interval = timerInterval;
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            gameManager.Map.ClearBrightness(overlayImg);
            gameManager.Map.DrawBrightness(overlayImg);
            playerImg.Image = gameManager.player.playerImage;
            gameManager.PlayerVisible(playerImg);
            overlayImg.Invalidate();
            StateBox.Invalidate();
            MsgBox.Invalidate();
            DispPoint();
        }

        private void InitPictureBoxes()
        {
            //マップ画像の枠　入れ子の親
            mapDrawBox.Size = new Size(Map.tileSize * 13, Map.tileSize * 13);

            // マップ画像　入れ子の子
            mapImage = new PictureBox
            {
                Size = new Size(gameManager.Map.Width * Map.tileSize, gameManager.Map.Height * Map.tileSize),
                Location = new Point(0, 0),
                Image = gameManager.Map.MapCanvas,
                SizeMode = PictureBoxSizeMode.Normal
            };

            mapDrawBox.Controls.Add(mapImage);

            // オーバーレイ画像
            overlayImg = new PictureBox
            {
                Size = mapDrawBox.Size,
                Location = new Point(0, 0),
                BackColor = Color.Transparent,
                Image = gameManager.Map.overrayCanvas,
                Parent = mapImage // mapImageの上に重ねる
            };

            overlayImg.BringToFront(); // mapImageの上に表示

            // プレイヤー画像
            playerImg = new PictureBox
            {
                Size = new(Map.tileSize, Map.tileSize),
                Location = new Point(Map.playerPos.X * Map.tileSize, Map.playerPos.Y * Map.tileSize),
                BackColor = Color.Transparent,
                Image = gameManager.player.playerImage,
                Parent = overlayImg
            };

            overlayImg.Controls.Add(playerImg);
            playerImg.BringToFront(); // mapImageの上に表示
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            gameManager.KeyInput(e.KeyCode, mapImage, overlayImg);
        }

        private void StateBox_Paint(object sender, PaintEventArgs e)
        {
            drawInfo.DrawStatus(e.Graphics, gameManager.player);
        }

        private void MsgBox_Paint(object sender, PaintEventArgs e)
        {
            gameManager.Message.Draw(e.Graphics);
        }

        // デバッグ用
        private void DispPoint()
        {
            label1.Text = $"XY:({Map.playerPos.X}, {Map.playerPos.Y}) limit:{gameManager.player.Limit} " +
                $"mi:({mapImage.Location.X},{mapImage.Location.Y}) " +
                $"oi:({overlayImg.Location.X},{overlayImg.Location.Y}) pi:({playerImg.Location.X},{playerImg.Location.Y})";
        }

    }//class
}//namespace
