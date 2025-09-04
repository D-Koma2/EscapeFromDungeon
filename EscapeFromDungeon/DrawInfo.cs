using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace EscapeFromDungeon
{
    internal class DrawInfo
    {
        private const int barWidth = 200;
        private const int barHeight = 20;
        private const int barX = 10;
        private const int barY = 10;
        private const int limitBarWidth = 36;
        private const int limitBarHeight = 11 * Map.tileSize + 10;
        private const int limitBarX = 5;
        private const int limitBarY = 50;

        public void DrawStatus(Graphics g, Player player)
        {
            float hpRatio = (float)player.Hp / player.MaxHp; // HP割合

            // HPの割合で色を決定
            Brush hpBrush;
            if (hpRatio > 0.6f) hpBrush = Brushes.Green;
            else if (hpRatio > 0.3f) hpBrush = Brushes.DarkOrange;
            else hpBrush = Brushes.Red;

            // 背景（最大HP）
            g.FillRectangle(Brushes.DarkGray, barX, barY, barWidth, barHeight);

            // 現在のHPバー
            int currentWidth = (int)(hpRatio * barWidth);
            g.FillRectangle(hpBrush, barX, barY, currentWidth, barHeight);

            // 枠線
            g.DrawRectangle(Pens.White, barX, barY, barWidth, barHeight);

            // 数値表示
            string hpText = $"HP: {player.Hp} / {player.MaxHp}";
            string inventoryTitle = "所持品:";
            string potionCount = Const.potion + " x " + player.Inventry.Count(item => item.Name == Const.potion).ToString();
            string curePoisonCount = Const.curePoison + " x " + player.Inventry.Count(item => item.Name == Const.curePoison).ToString();
            string torchCount = Const.torch + " x " + player.Inventry.Count(item => item.Name == Const.torch).ToString();

            using (Font font = new("Arial", 10))
            {
                g.DrawString(hpText, font, Brushes.White, barX + 10, barY + 2);
                if (player.Status == Status.Poison)
                {
                    g.DrawString("状態：毒", font, Brushes.Yellow, barX + 30, barY + 26);
                }
                else
                {
                    g.DrawString("状態：通常", font, Brushes.White, barX + 30, barY + 26);
                }

                g.DrawString(inventoryTitle, font, Brushes.White, barX + 30, barY + 48);

                g.DrawString(potionCount, font, Brushes.White, barX + 80, barY + 74);
                g.DrawString(curePoisonCount, font, Brushes.White, barX + 80, barY + 104);
                g.DrawString(torchCount, font, Brushes.White, barX + 80, barY + 134);

                int num = barY + 134;
                Font font2 = font;

                foreach (var item in player.Inventry)
                {
                    if (item.Name == Const.potion || item.Name == Const.curePoison || item.Name == Const.torch) continue;
                    num += 30;
                    g.DrawString(item.Name, font2, Brushes.White, barX + 80, num);
                }
            }

        }

        public void DrawLimitBar(Graphics g, Player player)
        {
            float limitRatio = (float)player.Limit / 999; // Limit割合

            // 背景（最大Limit）
            g.FillRectangle(Brushes.Red, limitBarX, limitBarY, limitBarWidth, limitBarHeight);

            // 現在のLimitバー
            int currentHight = (int)(limitRatio * limitBarHeight);
            g.FillRectangle(Brushes.Wheat, limitBarX, limitBarY, limitBarWidth, currentHight);

            // 枠線
            g.DrawRectangle(Pens.White, limitBarX, limitBarY, limitBarWidth, limitBarHeight);

            using (Font font = new("Arial", 10))
            {
                g.DrawString(player.Limit.ToString(), font, Brushes.Wheat, 6, 20);
                g.DrawString("Limit", font, Brushes.Red, 2, 2);
            }
        }
    }
}
