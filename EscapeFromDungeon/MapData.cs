using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EscapeFromDungeon
{
    internal class MapData
    {
        private static readonly Brush passableBrush = Brushes.LightGray;
        private static readonly Brush blockedBrush = Brushes.DarkSlateGray;

        public static Point playerPos = new Point(6, 6); // マップ上の座標   

        public const int tileSize = 32;
        public int mapX { get; set; }
        public int mapY { get; set; }

        // プレイヤーから何マス見えるか
        public int viewRadius { get; set; } = 6;

        // true: 視界制限あり、false: 全体表示
        public bool isVisionEnabled { get; set; } = true;

        public int[,]? mapData { get; private set; }

        public int MapWidth { get; private set; }
        public int MapHeight { get; private set; }
        public Bitmap? mapCanvas { get; set; }
        public Bitmap? mapCanvas2 { get; set; }
        public MapData(string path)
        {
            LoadFromCsv(path);
        }

        public void LoadFromCsv(string path)
        {
            var lines = File.ReadAllLines(path);
            MapHeight = lines.Length;
            MapWidth = lines[0].Split(',').Length;
            mapData = new int[MapWidth, MapHeight];
            mapCanvas = new Bitmap(MapWidth * tileSize, MapHeight * tileSize);
            mapCanvas2 = new Bitmap(MapWidth * tileSize, MapHeight * tileSize);

            for (int y = 0; y < MapHeight; y++)
            {
                var cells = lines[y].Split(',');
                for (int x = 0; x < MapWidth; x++)
                {
                    switch (cells[x].Trim())
                    {
                        case "S":
                            //playerPos = new Point(x, y);
                            mapData[x, y] = 0; // 通行可能
                            break;
                        case "0":
                            mapData[x, y] = 0;
                            break;
                        case "1":
                            mapData[x, y] = 1; // 通行不可
                            break;
                        default:
                            mapData[x, y] = 0;
                            break;
                    }
                }
            }
        }//LoadFromCsv

        public void Draw(PictureBox mapImage)
        {
            using (Graphics g = Graphics.FromImage(mapCanvas))
            {
                for (int y = 0; y < MapHeight; y++)
                {
                    for (int x = 0; x < MapWidth; x++)
                    {
                        Rectangle rect = new Rectangle(x * tileSize, y * tileSize, tileSize, tileSize);
                        Brush brush = mapData[x, y] == 0 ? passableBrush : blockedBrush;
                        g.FillRectangle(brush, rect);
                        DrawWallLines(g, x, y);
                    }
                }
            }

            mapImage.Image = mapCanvas;
        }//Draw

        // 壁ライン描画
        private void DrawWallLines(Graphics g, int x, int y)
        {
            int dx = x * tileSize, dy = y * tileSize;

            if (mapData[x, y] == 1)
            {
                // 上
                if (y == 0 || mapData[x, y - 1] != 1)
                    g.DrawLine(Pens.Black, dx, dy, dx + tileSize, dy);
                // 下
                if (y == MapHeight - 1 || mapData[x, y + 1] != 1)
                    g.DrawLine(Pens.Black, dx, dy + tileSize - 1, dx + tileSize, dy + tileSize - 1);
                // 左
                if (x == 0 || mapData[x - 1, y] != 1)
                    g.DrawLine(Pens.Black, dx - 1, dy, dx - 1, dy + tileSize);
                // 右
                if (x == MapWidth - 1 || mapData[x + 1, y] != 1)
                    g.DrawLine(Pens.Black, dx + tileSize - 1, dy, dx + tileSize - 1, dy + tileSize);
            }
        }


        public void DrawBrightness(PictureBox overlayBox)
        {
            int size = 32;

            using (Graphics g = Graphics.FromImage(mapCanvas2))
            {
                for (int y = 0; y < MapHeight; y++)
                {
                    for (int x = 0; x < MapWidth; x++)
                    {
                        Rectangle rect = new Rectangle(x * size, y * size, size, size);

                        // 視界判定
                        int dx = x - playerPos.X;
                        int dy = y - playerPos.Y;
                        double distance = Math.Sqrt(dx * dx + dy * dy);
                        //  視界制限が無効かどうか　もしくは　視界内かどうか
                        bool isVisible = !isVisionEnabled || distance / 2 <= viewRadius;

                        if (isVisible)
                        {
                            if (isVisionEnabled)
                            {
                                // ライト風の暗さを重ねる
                                double brightness = 1.0 - (distance / viewRadius);
                                int alpha = (int)((1.0 - brightness) * 255);
                                alpha = Math.Min(255, Math.Max(0, alpha));// アルファ値を0-255の範囲に制限
                                Color overlayColor = Color.FromArgb(alpha, 10, 0, 10);
                                using (Brush overlayBrush = new SolidBrush(overlayColor))
                                {
                                    g.FillRectangle(overlayBrush, rect);
                                }
                            }
                        }
                        else
                        {
                            g.FillRectangle(Brushes.Black, rect); // 視界外は黒
                        }

                    }
                }

                // プレイヤー仮置
                g.FillRectangle(Brushes.Red, new Rectangle(playerPos.X * size, playerPos.Y * size, size, size));
            }
        }

    }//class
}//namespace
