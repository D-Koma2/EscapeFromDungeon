namespace EscapeFromDungeon
{
    public partial class Form1 : Form
    {
        private string file = "map.csv";
        private Map map;
        private PictureBox mapImage, overlayBox, playerImg;
        private GameManager gameManager;
        private System.Timers.Timer scrollTimer;

        public Form1()
        {
            InitializeComponent();
            init();
        }

        private void init()
        {
            map = new Map(file);
            gameManager = new GameManager();

            mapDrawBox.Size = new Size(Map.tileSize * 13, Map.tileSize * 13);

            mapImage = new PictureBox
            {
                Size = new Size(map.Width * Map.tileSize, map.Height * Map.tileSize),
                Location = new Point(0, 0),
                Image = map.MapCanvas,
                SizeMode = PictureBoxSizeMode.Normal
            };

            mapDrawBox.Controls.Add(mapImage);

            // ライト用オーバーレイ
            overlayBox = new PictureBox
            {
                Size = mapDrawBox.Size,
                Location = new Point(0, 0),
                BackColor = Color.Transparent,
                Image = map.overrayCanvas,
                Parent = mapImage // mapImageの上に重ねる
            };

            overlayBox.BringToFront(); // mapImageの上に表示

            playerImg = new PictureBox
            {
                Size = new(Map.tileSize, Map.tileSize),
                Location = new Point(Map.playerPos.X * Map.tileSize, Map.playerPos.Y * Map.tileSize),
                BackColor = Color.Transparent,
                Image = gameManager.player.playerImage,
                Parent = overlayBox
            };

            overlayBox.Controls.Add(playerImg);
            playerImg.BringToFront(); // mapImageの上に表示

            map.Draw(mapImage);
            map.DrawBrightness(overlayBox);

            gameManager.SetMapPos(mapImage, overlayBox, playerImg);
            DispPoint();
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            gameManager.KeyInput(e.KeyCode, mapImage, overlayBox, map);
            playerImg.Image = gameManager.player.playerImage;
            overlayBox.Invalidate();
            DispPoint();
        }

        private void DispPoint()
        {
            label1.Text = $"({Map.playerPos.X}, {Map.playerPos.Y}) mI:({mapImage.Location.X},{mapImage.Location.Y}) " +
                $"ob:({overlayBox.Location.X},{overlayBox.Location.Y}) pi:({playerImg.Location.X},{playerImg.Location.Y})";
        }

    }//class
}//namespace
