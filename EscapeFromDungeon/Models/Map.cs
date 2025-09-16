using EscapeFromDungeon.Constants;
using EscapeFromDungeon.Core;
using EscapeFromDungeon.Properties;
using System.IO;

namespace EscapeFromDungeon.Models
{
    internal class Map
    {
        enum WalkType
        {
            Passable = 0,
            Wall = 1,
            SemiWall = 2,
            Damage = 3,
            Enemy = 9
        }

        private static Map? _instance;

        public static Map? Instance
        {
            get => _instance;

            private set
            {
                _instance ??= value;
            }
        }

        private static readonly Brush _passableBrush = Brushes.LightGray;
        private static readonly Brush _blockedBrush = Brushes.DarkSlateGray;
        private static readonly Brush _transWallBrush = new SolidBrush(Color.FromArgb(255, 54, 83, 83));

        public static Point PlayerPos { get; set; } = new Point(6, 6); // マップ上の座標(map.csvの"SS"で変更)

        public static Point PlayerDispPos { get; private set; } = new Point(6, 6); //プレイヤー表示座標(map中央固定)

        public static bool IsVisionEnabled { get; private set; } = true; // true: 視界制限あり、false: 全体表示デバッグ用

        private static int _viewRadius = 10;// 歩数やアイテムで変化させる
        private static readonly int _minViewRadius = 2;

        public static readonly int _maxViewRadius = 10;
        public const int tileSize = 32;

        public static int Width { get; private set; }
        public static int Height { get; private set; }

#pragma warning disable CS8618 
        public static int[,] WalkMap { get; private set; }
        public static string[,] EventMap { get; private set; }
        public static Bitmap MapCanvas { get; private set; }
        public static Bitmap OverrayCanvas { get; private set; }
#pragma warning restore CS8618

        private static Dictionary<string, int> _walkMapCodes = new()
        {
            { "00", 0 },//通路
            { "XX", 3 },//ダメージ床
            { "11", 1 },//壁
            { "12", 2 }//半透明壁
        };

        private static string[] _readLines = default!;

        public Map(string path)
        {
            ReadData(path);
            StartUp();
        }

        public Map()
        {
            _readLines = Resources.map.Split(Const.separator, StringSplitOptions.None);
            StartUp();
        }

        public void ReadData(string path)
        {
            try
            {
                _readLines = File.ReadAllLines(path);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Map.csv読み込みエラー: {ex.Message}");
            }
        }

        public void StartUp()
        {
            if (_readLines.Last().Trim() == "") _readLines = _readLines.Take(_readLines.Length - 1).ToArray();//最終行が空行なら削除
            Height = _readLines.Length;
            Width = _readLines[0].Split(',').Length;

            WalkMap = new int[Width, Height];
            EventMap = new string[Width, Height];
            MapCanvas = new Bitmap(Width * tileSize, Height * tileSize);
            OverrayCanvas = new Bitmap(Width * tileSize, Height * tileSize);

            InitMap();
            SetIsVisionEnable(true);
        }

        public static void SetIsVisionEnable(bool value) => IsVisionEnabled = value;

        public static void InitMap()
        {
            for (int y = 0; y < Height; y++)
            {
                var cells = _readLines[y].Split(',');
                for (int x = 0; x < Width; x++)
                {
                    string code = cells[x].Trim();

                    if (code == "SS")//スタート地点
                    {
                        PlayerPos = new Point(x, y);
                        WalkMap[x, y] = 0;
                    }
                    else if (code == "GG")//ゴール地点
                    {
                        EventMap[x, y] = code;
                        WalkMap[x, y] = 0;
                    }
                    else if (code.StartsWith("E"))//敵シンボル
                    {
                        EventMap[x, y] = code;
                        WalkMap[x, y] = 9;
                    }
                    else if (_walkMapCodes.ContainsKey(code))//通路、壁、半透明壁、ダメージ床
                    {
                        WalkMap[x, y] = _walkMapCodes[code];
                    }
                    else//イベント
                    {
                        EventMap[x, y] = code;
                        WalkMap[x, y] = 0;
                    }
                }
            }
        }

        public static void AddViewRadius(int radius)
        {
            _viewRadius += radius;
            _viewRadius = Math.Clamp(_viewRadius, _minViewRadius, _maxViewRadius);
        }

        public static void Draw(PictureBox mapImage)
        {
            using (Graphics g = Graphics.FromImage(MapCanvas))
            {
                for (int y = 0; y < Height; y++)
                {
                    for (int x = 0; x < Width; x++)
                    {
                        Rectangle rect = new Rectangle(x * tileSize, y * tileSize, tileSize, tileSize);

                        if (WalkMap[x, y] == 0)//通路
                        {
                            g.FillRectangle(_passableBrush, rect);
                        }
                        else if (WalkMap[x, y] == 3)//ダメージ床
                        {
                            g.DrawImage(Resources.poisonTile, x * tileSize, y * tileSize);
                        }
                        else if (WalkMap[x, y] == 2)//半透明壁
                        {
                            g.FillRectangle(_transWallBrush, rect);
                        }
                        else if (WalkMap[x, y] == 9)//敵シンボル
                        {
                            g.FillRectangle(_passableBrush, rect);
                            g.DrawImage(Resources.EnemySimbol1, x * tileSize, y * tileSize);
                            WalkMap[x, y] = 0;
                        }
                        else//壁
                        {
                            g.FillRectangle(_blockedBrush, rect);
                        }

                        DrawWallLines(g, x, y);
                    }
                }
            }

            mapImage.Image = MapCanvas;
        }//Draw

        //敵シンボル位置を床で上書きする(壁ラインが消えるので、描画サイズは上下左右1ピクセルずつ減らす)
        public static void DelEnemySimbolDraw(int x, int y)
        {
            using (Graphics g = Graphics.FromImage(MapCanvas))
            {
                Rectangle rect = new Rectangle(x * tileSize + 1, y * tileSize + 1, tileSize - 2, tileSize - 2);
                g.FillRectangle(_passableBrush, rect);
            }
        }

        // 壁ライン描画
        private static void DrawWallLines(Graphics g, int x, int y)
        {
            int dx = x * tileSize, dy = y * tileSize;

            if (WalkMap[x, y] is (1 or 2))
            {
                // 上
                if (y == 0 || WalkMap[x, y - 1] != 1 && WalkMap[x, y - 1] != 2)
                    g.DrawLine(Pens.Black, dx, dy, dx + tileSize, dy);
                // 下
                if (y == Height - 1 || WalkMap[x, y + 1] != 1 && WalkMap[x, y + 1] != 2)
                    g.DrawLine(Pens.Black, dx, dy + tileSize - 1, dx + tileSize, dy + tileSize - 1);
                // 左
                if (x == 0 || WalkMap[x - 1, y] != 1 && WalkMap[x - 1, y] != 2)
                    g.DrawLine(Pens.Black, dx - 1, dy, dx - 1, dy + tileSize);
                // 右
                if (x == Width - 1 || WalkMap[x + 1, y] != 1 && WalkMap[x + 1, y] != 2)
                    g.DrawLine(Pens.Black, dx + tileSize - 1, dy, dx + tileSize - 1, dy + tileSize);
            }
        }//DrawWallLines

        public static void DrawBrightness()
        {
            using (Graphics g = Graphics.FromImage(OverrayCanvas))
            {
                int maxWidth = tileSize * Form1.tilesOfMapWidth;
                int maxHeight = tileSize * Form1.tilesOfMapHeight;

                for (int y = 0; y < maxHeight; y++)
                {
                    for (int x = 0; x < maxWidth; x++)
                    {
                        Rectangle rect = new Rectangle(x * tileSize, y * tileSize, tileSize, tileSize);

                        // 視界判定
                        int dx = x - PlayerDispPos.X;
                        int dy = y - PlayerDispPos.Y;

                        double distance = Math.Sqrt(dx * dx + dy * dy);
                        //  視界制限が無効かどうか　もしくは　視界内かどうか
                        bool isVisible = !IsVisionEnabled || distance / 2 <= _viewRadius;

                        if (isVisible)
                        {
                            if (IsVisionEnabled)
                            {
                                // ライト風の暗さを重ねる
                                double brightness = 1.0 - distance / _viewRadius;
                                int alpha = (int)((1.0 - brightness) * 255);
                                alpha = Math.Clamp(alpha, 0, 255);// アルファ値を0-255の範囲に制限
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

        public static void ClearBrightness(PictureBox overlayBox)
        {
            OverrayCanvas = new Bitmap(Width * tileSize, Height * tileSize);
            overlayBox.Image = OverrayCanvas;
        }

        public static bool CanMoveTo(int x, int y)
        {
            if (x < 0 || y < 0 || x >= Width || y >= Height) return false;

            if (WalkMap[x, y] is (0 or 2 or 3)) return true;
            return false;
        }

        public static void DeleteEvent(int x, int y)
        {
            EventMap[x, y] = null!;
        }

    }//class
}//namespace
