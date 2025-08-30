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
            string state = $"状態： {player.Status.ToString()}";
            string limitText = $"リミット: {player.Limit}";
            string inventoryTitle = "所持品:";
            using (Font font = new("Arial", 10))
            {
                g.DrawString(hpText, font, Brushes.White, barX + 10, barY + 2);
                g.DrawString(state, font, Brushes.White, barX + 10, barY + 26);
                g.DrawString(limitText, font, Brushes.White, barX + 10, barY + 48);
                g.DrawString(inventoryTitle, font, Brushes.White, barX + 10, barY + 76);

                int num = barY + 52;
                Font font2 = font;

                foreach (var item in player.Inventry)
                {
                    num += 24;
                    g.DrawString(item.Name, font2, Brushes.White, barX + 80, num);
                }
            }

        }
    }
}
