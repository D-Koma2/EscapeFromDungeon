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
        private const int barX = 20;
        private const int barY = 20;

        public void DrawHPBar(PictureBox infoBox, Player player)
        {
            float hpRatio = (float)player.Hp / player.MaxHp; // HP割合

            // HPの割合で色を決定
            Brush hpBrush;
            if (hpRatio > 0.6f) hpBrush = Brushes.Green;
            else if (hpRatio > 0.3f) hpBrush = Brushes.Yellow;
            else hpBrush = Brushes.Red;

            Graphics g = Graphics.FromImage(infoBox.Image);
            // 背景（最大HP）
            g.FillRectangle(Brushes.DarkGray, barX, barY, barWidth, barHeight);

            // 現在のHPバー
            int currentWidth = (int)(hpRatio * barWidth);
            g.FillRectangle(hpBrush, barX, barY, currentWidth, barHeight);

            // 枠線
            g.DrawRectangle(Pens.White, barX, barY, barWidth, barHeight);

            // 数値表示
            string hpText = $"HP: {player.Hp} / {player.MaxHp}";
            using (Font font = new Font("Arial", 10))
            {
                g.DrawString(hpText, font, Brushes.White, barX + 120, barY + 2);
            }
        }
    }
}
