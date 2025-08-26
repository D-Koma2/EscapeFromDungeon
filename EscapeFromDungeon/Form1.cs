using EscapeFromDungeon.resources;

namespace EscapeFromDungeon
{
    public partial class Form1 : Form
    {
        private MapLoader mapLoader;
        private PictureBox mapPicture;

        public Form1()
        {
            InitializeComponent();
            init();
        }

        private void init()
        {
            mapLoader = new MapLoader();
            var file = "map.csv";
            mapLoader.LoadMap(file);
            mainPicture.Size = new Size(640, 480);

            mapPicture = new PictureBox();
            mapPicture.Size = new Size(640, 480);
            mapPicture.Location = new Point(64, 64);
            mainPicture.Controls.Add(mapPicture);
            mapPicture.Image = mapLoader.mapCanvas;
            mapLoader.DispMap();
        }

        private void MapPicture_Paint(object? sender, PaintEventArgs e)
        {
            mapLoader.DispMap();

        }

        private void mainPicture_Paint(object sender, PaintEventArgs e)
        {

        }
    }//class
}//namespace
