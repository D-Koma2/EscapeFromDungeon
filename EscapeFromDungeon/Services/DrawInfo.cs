using EscapeFromDungeon.Constants;
using EscapeFromDungeon.Models;

namespace EscapeFromDungeon.Services
{
    internal static class DrawInfo
    {
        private const int _barWidth = 200;
        private const int _barHeight = 20;
        private const int _barX = 10;
        private const int _barY = 10;
        private const int _limitBarWidth = 36;
        private const int _limitBarHeight = 11 * Map.tileSize + 10;
        private const int _limitBarX = 5;
        private const int _limitBarY = 50;

        public static void DrawStatus(Graphics g, Player player)
        {
            float hpRatio = (float)player.Hp / player.MaxHp; // HP割合

            // HPの割合で色を決定
            Brush hpBrush;
            if (hpRatio > 0.6f) hpBrush = Brushes.Green;
            else if (hpRatio > 0.3f) hpBrush = Brushes.DarkOrange;
            else hpBrush = Brushes.Red;

            // 背景（最大HP）
            g.FillRectangle(Brushes.DarkGray, _barX, _barY, _barWidth, _barHeight);

            // 現在のHPバー
            int currentWidth = (int)(hpRatio * _barWidth);
            g.FillRectangle(hpBrush, _barX, _barY, currentWidth, _barHeight);

            // 枠線
            g.DrawRectangle(Pens.White, _barX, _barY, _barWidth, _barHeight);

            // 数値表示
            string hpText = $"HP: {player.Hp.ToString("D3")} / {player.MaxHp.ToString("D3")}";
            string inventoryTitle = "所持品:";
            string potionCount = Const.potion + " x " + player.Inventry.Count(item => item.Name == Const.potion).ToString();
            string curePoisonCount = Const.curePoison + " x " + player.Inventry.Count(item => item.Name == Const.curePoison).ToString();
            string torchCount = Const.torch + " x " + player.Inventry.Count(item => item.Name == Const.torch).ToString();

            using (Font font = new("Arial", 10))
            {
                g.DrawString(hpText, font, Brushes.White, _barX + 10, _barY + 2);
                if (player.Status is Status.Poison)
                {
                    g.DrawString("状態：毒", font, Brushes.Yellow, _barX + 30, _barY + 26);
                }
                else
                {
                    g.DrawString("状態：通常", font, Brushes.White, _barX + 30, _barY + 26);
                }

                g.DrawString(inventoryTitle, font, Brushes.White, _barX + 30, _barY + 48);

                g.DrawString(potionCount, font, Brushes.White, _barX + 80, _barY + 74);
                g.DrawString(curePoisonCount, font, Brushes.White, _barX + 80, _barY + 104);
                g.DrawString(torchCount, font, Brushes.White, _barX + 80, _barY + 134);

                int dispY = _barY + 134;
 
                foreach (var item in player.Inventry)
                {
                    if (item.Name is (Const.potion or Const.curePoison or Const.torch)) continue;
                    dispY += 30;
                    g.DrawString(item.Name, font, Brushes.White, _barX + 80, dispY);
                }
            }

        }

        public static void DrawLimitBar(Graphics g, int limit, int limitMax)
        {
            float limitRatio = (float)limit / limitMax; // Limit割合

            // 背景（最大Limit）
            g.FillRectangle(Brushes.Red, _limitBarX, _limitBarY, _limitBarWidth, _limitBarHeight);

            // 現在のLimitバー
            int currentHight = (int)(limitRatio * _limitBarHeight);
            g.FillRectangle(Brushes.Wheat, _limitBarX, _limitBarY, _limitBarWidth, currentHight);

            // 枠線
            g.DrawRectangle(Pens.White, _limitBarX, _limitBarY, _limitBarWidth, _limitBarHeight);

            using (Font font = new("Arial", 10))
            {
                g.DrawString(limit.ToString("D3"), font, Brushes.Wheat, 6, 20);
                g.DrawString("Limit", font, Brushes.Red, 2, 2);
            }
        }
    }
}
