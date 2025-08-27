using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace EscapeFromDungeon
{
    internal class GameManager
    {
        private Player player;                                   
        private int viewRadius = 6; // プレイヤーから何マス見えるか
        private bool isMoving = false;
        private bool isVisionEnabled = true; // true: 視界制限あり、false: 全体表示

        public GameManager()
        {
            player = new Player
            {
                Name = "Hero",
                Hp = 100,
                Attack = 20,
                Defence = 10,
                Speed = 5,
                Status = Status.Normal,
            };
        }

        public void KeyInput(object sender, KeyEventArgs e)
        {
            if (isMoving) return; // 移動中は入力を無視

            if (e.KeyCode == Keys.V)
            {
                isVisionEnabled = !isVisionEnabled;
                // 再描画
            }

            Point dir = Point.Empty;

            if (e.KeyCode == Keys.Up)
            {
                dir = new Point(0, -1);
                player.Dir = Player.Direction.Up;
            }
            else if (e.KeyCode == Keys.Down)
            {
                dir = new Point(0, 1);
                player.Dir = Player.Direction.Down;
            }
            else if (e.KeyCode == Keys.Left)
            {
                dir = new Point(-1, 0);
                player.Dir = Player.Direction.Left;
            }
            else if (e.KeyCode == Keys.Right)
            {
                dir = new Point(1, 0);
                player.Dir = Player.Direction.Right;
            }

            Point newPos = new Point(MapData.playerPos.X + dir.X, MapData.playerPos.Y + dir.Y);

            if (CanMoveTo(newPos.X, newPos.Y))
            {
                MapData.playerPos = newPos;
                //isMoving = true;
                //ScrollMapToPlayer();
            }
            else
            {
                // 向きだけ変えて移動しない（壁に向かっても向きは変わる）
                //player.DirectionImage();
            }

            Console.WriteLine($"x:{MapData.playerPos.X} y:{MapData.playerPos.Y}");
        }

        private bool CanMoveTo(int x, int y)
        {
            //if (x < 0 || y < 0 || x >= width || y >= height) return false;
            //return mapData[x, y] == 0; // 0 = 通行可能
            return true; // 仮実装
        }

    }
}
