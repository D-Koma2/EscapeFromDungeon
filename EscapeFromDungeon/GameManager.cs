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
        public Player player {  get; private set; }

        // true: 視界制限あり、false: 全体表示
        public static bool IsVisionEnabled { get; set; } = true;

        private bool isMoving = false;

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

        public void KeyInput(Keys keyCode, PictureBox mapImage, PictureBox overlayBox, Map map)
        {
            //if (isMoving) return; // 移動中は入力を無視

            if (keyCode == Keys.V)
            {
                IsVisionEnabled = !IsVisionEnabled;
            }

            Point dir = Point.Empty;
            int moveAmount = 32; // 移動量（ピクセル）

            Point current1 = mapImage.Location;
            Point current2 = overlayBox.Location;

            if (keyCode == Keys.Up)
            {
                current1.Y += moveAmount;
                current2.Y -= moveAmount;
                dir = new Point(0, -1);
                player.Dir = Player.Direction.Up;
            }
            else if (keyCode == Keys.Down)
            {
                current1.Y -= moveAmount;
                current2.Y += moveAmount;
                dir = new Point(0, 1);
                player.Dir = Player.Direction.Down;
            }
            else if (keyCode == Keys.Left)
            {
                current1.X += moveAmount;
                current2.X -= moveAmount;
                dir = new Point(-1, 0);
                player.Dir = Player.Direction.Left;
            }
            else if (keyCode == Keys.Right)
            {
                current1.X -= moveAmount;
                current2.X += moveAmount;
                dir = new Point(1, 0);
                player.Dir = Player.Direction.Right;
            }

            player.GetPlayerImage(player.Dir);
            Point newPos = new Point(Map.playerPos.X + dir.X, Map.playerPos.Y + dir.Y);

            if (map.CanMoveTo(newPos.X, newPos.Y))
            {
                Map.playerPos = newPos;
                mapImage.Location = current1;
                overlayBox.Location = current2;
            }

            Console.WriteLine($"x:{Map.playerPos.X} y:{Map.playerPos.Y}");
        }

        public void SetMapPos(PictureBox mapImage, PictureBox overlayBox, PictureBox playerImage)
        {
            int moveX = Map.tileSize * (Map.playerPos.X - 6);
            int moveY = Map.tileSize * (Map.playerPos.Y - 6);

            mapImage.Location = new Point(-moveX, -moveY);
            overlayBox.Location = new Point(moveX, moveY);
            playerImage.Location = new Point(Map.tileSize * 6, Map.tileSize * 6);
        }

    }//class
}//namespace
