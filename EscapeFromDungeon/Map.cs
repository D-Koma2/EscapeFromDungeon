using EscapeFromDungeon.Properties;
using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EscapeFromDungeon
{
    internal class Map
    {
        //private static readonly Brush damageBrush = Brushes.Purple;//ダメージ床
        private static readonly Brush passableBrush = Brushes.LightGray;
        private static readonly Brush blockedBrush = Brushes.DarkSlateGray;
        private static readonly Color color = Color.FromArgb(255, 54, 83, 83);
        private static readonly Brush transWallBrush = new SolidBrush(color);

        public static Point playerPos = new Point(6, 6); // マップ上の座標(map.csvの"S"で変更)
        public static Point playerDispPos = new Point(6, 6); //プレイヤー表示座標(map中央固定)

        public static int viewRadius = 12;// 歩数やアイテムで変化させる

        public const int tileSize = 32;
        public int MapX { get; set; }
        public int MapY { get; set; }

        public int[,] WalkMap { get; private set; }

        public string[,] EventMap { get; private set; }

        public int Width { get; private set; }
        public int Height { get; private set; }
        public Bitmap MapCanvas { get; private set; }
        public Bitmap overrayCanvas { get; private set; }

        public Map(string path)
        {
            var lines = File.ReadAllLines(path);
            //var lines = Resources.map.Split(Const.separator, StringSplitOptions.None);
            if (lines.Last().Trim() == "") lines = lines.Take(lines.Length - 1).ToArray();//最終行が空行なら削除
            Height = lines.Length;
            Width = lines[0].Split(',').Length;
            WalkMap = new int[Width, Height];
            EventMap = new string[Width, Height];
            MapCanvas = new Bitmap(Width * tileSize, Height * tileSize);
            overrayCanvas = new Bitmap(Width * tileSize, Height * tileSize);

            for (int y = 0; y < Height; y++)
            {
                var cells = lines[y].Split(',');
                for (int x = 0; x < Width; x++)
                {
                    switch (cells[x].Trim())
                    {
                        case "SS"://スタート地点
                            playerPos = new Point(x, y);
                            WalkMap[x, y] = 0;
                            break;
                        case "GG"://ゴール
                            EventMap[x, y] = cells[x];
                            WalkMap[x, y] = 0;
                            break;
                        case "00"://通路
                            WalkMap[x, y] = 0;
                            break;
                        case "XX"://ダメージ床
                            WalkMap[x, y] = 3;
                            break;
                        case "11"://壁
                            WalkMap[x, y] = 1;
                            break;
                        case "12"://通れる壁
                            WalkMap[x, y] = 2;
                            break;
                        case "E1"://敵
                        case "E2":
                        case "E3":
                        case "E4":
                        case "E5":
                        case "E6":
                        case "E7":
                        case "E8":
                        case "E9":
                            EventMap[x, y] = cells[x];
                            WalkMap[x, y] = 9;
                            break;
                        default:
                            EventMap[x, y] = cells[x];
                            WalkMap[x, y] = 0;
                            break;
                    }
                }
            }
        }//LoadFromCsv

        public void Draw(PictureBox mapImage)
        {
            using (Graphics g = Graphics.FromImage(MapCanvas))
            {
                for (int y = 0; y < Height; y++)
                {
                    for (int x = 0; x < Width; x++)
                    {
                        Rectangle rect = new Rectangle(x * tileSize, y * tileSize, tileSize, tileSize);

                        if (WalkMap[x, y] == 0)
                        {
                            g.FillRectangle(passableBrush, rect);
                        }
                        else if (WalkMap[x, y] == 3)
                        {
                            g.DrawImage(Resources.poisonTile, x * tileSize, y * tileSize);
                        }
                        else if (WalkMap[x, y] == 2)
                        {
                            g.FillRectangle(transWallBrush, rect);
                        }
                        else if (WalkMap[x, y] == 9)
                        {
                            g.FillRectangle(passableBrush, rect);
                            g.DrawImage(Resources.EnemySimbol1, x * tileSize, y * tileSize);
                            WalkMap[x, y] = 0;
                        }
                        else
                        {
                            g.FillRectangle(blockedBrush, rect);
                        }

                        DrawWallLines(g, x, y);
                    }
                }
            }

            mapImage.Image = MapCanvas;
        }//Draw

        //敵シンボル位置を床で上書きする(壁ラインが消えるので、描画サイズは上下左右1ピクセルずつ減らす)
        public void DelEnemySimbolDraw(int x, int y)
        {
            using (Graphics g = Graphics.FromImage(MapCanvas))
            {
                Rectangle rect = new Rectangle(x * tileSize + 1, y * tileSize + 1, tileSize - 2, tileSize - 2);
                g.FillRectangle(passableBrush, rect);
            }
        }

        // 壁ライン描画
        private void DrawWallLines(Graphics g, int x, int y)
        {
            int dx = x * tileSize, dy = y * tileSize;

            if (WalkMap[x, y] == 1 || WalkMap[x, y] == 2)
            {
                // 上
                if (y == 0 || (WalkMap[x, y - 1] != 1 && WalkMap[x, y - 1] != 2))
                    g.DrawLine(Pens.Black, dx, dy, dx + tileSize, dy);
                // 下
                if (y == Height - 1 || (WalkMap[x, y + 1] != 1 && WalkMap[x, y + 1] != 2))
                    g.DrawLine(Pens.Black, dx, dy + tileSize - 1, dx + tileSize, dy + tileSize - 1);
                // 左
                if (x == 0 || (WalkMap[x - 1, y] != 1 && WalkMap[x - 1, y] != 2))
                    g.DrawLine(Pens.Black, dx - 1, dy, dx - 1, dy + tileSize);
                // 右
                if (x == Width - 1 || (WalkMap[x + 1, y] != 1 && WalkMap[x + 1, y] != 2))
                    g.DrawLine(Pens.Black, dx + tileSize - 1, dy, dx + tileSize - 1, dy + tileSize);
            }
        }//DrawWallLines

        public void DrawBrightness(PictureBox overlayBox)
        {
            using (Graphics g = Graphics.FromImage(overrayCanvas))
            {
                for (int y = 0; y < Height; y++)
                {
                    for (int x = 0; x < Width; x++)
                    {
                        Rectangle rect = new Rectangle(x * Map.tileSize, y * Map.tileSize, Map.tileSize, Map.tileSize);

                        // 視界判定
                        int dx = x - playerDispPos.X;
                        int dy = y - playerDispPos.Y;
                        double distance = Math.Sqrt(dx * dx + dy * dy);
                        //  視界制限が無効かどうか　もしくは　視界内かどうか
                        bool isVisible = !GameManager.IsVisionEnabled || distance / 2 <= viewRadius;

                        if (isVisible)
                        {
                            if (GameManager.IsVisionEnabled)
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

            }
        }//DrawBrightness

        public void ClearBrightness(PictureBox overlayBox)
        {
            overrayCanvas = new Bitmap(Width * tileSize, Height * tileSize);
            overlayBox.Image = overrayCanvas;
        }

        public bool CanMoveTo(int x, int y)
        {
            if (x < 0 || y < 0 || x >= Width || y >= Height) return false;

            if (WalkMap[x, y] == 0 || WalkMap[x, y] == 2 || WalkMap[x, y] == 3) return true;
            return false;
        }

        public void DeleteEvent(int x, int y)
        {
            EventMap[x, y] = null;
        }

    }//class
}//namespace
