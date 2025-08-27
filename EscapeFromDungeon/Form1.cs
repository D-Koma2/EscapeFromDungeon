namespace EscapeFromDungeon
{
    public partial class Form1 : Form
    {
        private string file = "map.csv";
        private MapData mapData;
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
            mapData = new MapData(file);

            mapDrawBox.Size = new Size(MapData.tileSize * 13, MapData.tileSize * 13);

            mapImage = new PictureBox
            {
                Size = new Size(mapData.MapWidth * MapData.tileSize, mapData.MapHeight * MapData.tileSize),
                Location = new Point(0, 0),
                Image = mapData.mapCanvas,
                SizeMode = PictureBoxSizeMode.Normal
            };

            mapDrawBox.Controls.Add(mapImage);

            // ライト用オーバーレイ
            overlayBox = new PictureBox
            {
                Size = mapDrawBox.Size,
                Location = new Point(0, 0),
                BackColor = Color.Transparent,
                Image = mapData.mapCanvas2,
                Parent = mapImage // mapImageの上に重ねる
            };

            overlayBox.BringToFront(); // mapImageの上に表示

            gameManager = new GameManager();
            mapData.Draw(mapImage);

            mapData.DrawBrightness(overlayBox);
        }


        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            //gameManager.KeyInput(sender, e);

            int moveAmount = 32; // 移動量（ピクセル）

            Point current1 = mapImage.Location;
            Point current2 = overlayBox.Location;

            switch (e.KeyCode)
            {
                case Keys.Down:
                    current1.Y -= moveAmount;
                    current2.Y += moveAmount;
                    break;
                case Keys.Up:
                    current1.Y += moveAmount;
                    current2.Y -= moveAmount;
                    break;
                case Keys.Right:
                    current1.X -= moveAmount;
                    current2.X += moveAmount;
                    break;
                case Keys.Left:
                    current1.X += moveAmount;
                    current2.X -= moveAmount;
                    break;
            }

            mapImage.Location = current1;
            overlayBox.Location = current2;
        }
    }//class
}//namespace
