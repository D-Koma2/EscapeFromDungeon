using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace EscapeFromDungeon.resources
{
    internal class MapLoader
    {
        private const int tileSize = 32;

        public int mapX { get; set; }
        public int mapY { get; set; }
        public int[,] mapData { get; private set; }
        private int mapWidth, mapHeight;
        public Bitmap mapCanvas { get; set; }

        public void LoadMap(string path)
        {
            if (File.Exists(path))
            {
                var rows = File.ReadAllLines(path);

                mapWidth = rows.Length;
                mapHeight = rows[0].Split(',').Length;
                mapCanvas = new Bitmap(mapWidth, mapHeight);
                mapData = new int[mapWidth, mapHeight];

                for (int i = 0; i < rows.Length; i++)
                {
                    var cols = rows[i].Split(',');

                    for (int j = 0; j < cols.Length; j++)
                    {
                        switch (cols[i])
                        {
                            case "0":
                                mapData[j, i] = 0;
                                break;
                            case "1":
                                mapData[j, i] = 1;
                                break;
                            case "S":
                                mapData[j, i] = 0;
                                break;
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("ファイルが見つかりません");
            }
        }

        public void DispMap()
        {
            using (Graphics g = Graphics.FromImage(mapCanvas))
            {
                for (int i = 0; i < mapWidth; i++)
                {
                    for (int j = 0; j < mapHeight; j++)
                    {
                        Color col = (mapData[i, j] == 0 ? Color.Cyan : Color.DarkMagenta);
                        SolidBrush brush = new SolidBrush(col);
                        g.FillRectangle(brush, new Rectangle(j, i, j * tileSize + tileSize, i * tileSize + tileSize));
                    }
                }
            }

        }
    }
}
